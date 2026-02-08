using UnityEngine;

public class LatticeBuilder : MonoBehaviour
{
    public TextAsset jsonFile;
    public GameObject atomPrefab;
    public Transform latticeRoot;

    void Start()
    {
        Build();
    }

    public void Build()
    {
        if (jsonFile == null || atomPrefab == null || latticeRoot == null)
        {
            Debug.LogError("Missing references: jsonFile / atomPrefab / latticeRoot");
            return;
        }

        var data = JsonUtility.FromJson<LatticeJson>(jsonFile.text);
        if (data == null || data.BasisAtoms == null)
        {
            Debug.LogError("Failed to parse JSON or BasisAtoms is null.");
            return;
        }

        Vector3 A1 = ToV3(data.A1);
        Vector3 A2 = ToV3(data.A2);
        Vector3 A3 = ToV3(data.A3);

        int n = Mathf.Max(1, data.Size);

        // Clear old atoms
        for (int i = latticeRoot.childCount - 1; i >= 0; i--)
            Destroy(latticeRoot.GetChild(i).gameObject);

        // Build
        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
        for (int k = 0; k < n; k++)
        {
            foreach (var b in data.BasisAtoms)
            {
                Vector3 f = ToV3(b.RelativePos);               // (u,v,w)
                Vector3 fCell = new Vector3(f.x + i, f.y + j, f.z + k);

                // r = (u+i)A1 + (v+j)A2 + (w+k)A3
                Vector3 r = fCell.x * A1 + fCell.y * A2 + fCell.z * A3;

                GameObject atom = Instantiate(atomPrefab, latticeRoot);
                atom.transform.localPosition = r;
                atom.transform.localRotation = Quaternion.identity;

                float radius = (b.Radius > 0f) ? b.Radius : 0.12f;
                atom.transform.localScale = Vector3.one * radius;

                // Fill meta
                var meta = atom.GetComponent<AtomMeta>();
                if (meta != null)
                {
                    meta.Element = string.IsNullOrEmpty(b.Element) ? "X" : b.Element;
                    meta.Fractional = fCell;
                    meta.Cartesian = r;
                    meta.CellIndex = new Vector3Int(i, j, k);
                }

                // Color
                if (b.Colour != null && b.Colour.Length >= 3)
                {
                    Color c = new Color(b.Colour[0], b.Colour[1], b.Colour[2], 1f);
                    var renderer = atom.GetComponent<Renderer>();
                    if (renderer != null) renderer.material.color = c;
                }
            }
        }
    }

    private Vector3 ToV3(float[] a)
    {
        if (a == null || a.Length < 3) return Vector3.zero;
        return new Vector3(a[0], a[1], a[2]);
    }
}

