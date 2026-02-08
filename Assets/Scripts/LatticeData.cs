using System;
using UnityEngine;

[Serializable]
public class LatticeJson
{
    public int Size = 1;
    public float[] A1;
    public float[] A2;
    public float[] A3;
    public BasisAtom[] BasisAtoms;
}

[Serializable]
public class BasisAtom
{
    public float[] RelativePos;   // fractional (u,v,w)
    public float[] Colour;        // RGB 0~1
    public string Element;        // optional
    public float Radius = 0.12f;  // optional default
}

