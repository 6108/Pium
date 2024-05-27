using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnitureData
{
    public string type;
    public string subType;
    public string name;
    public string info;
}

public class Furniture : MonoBehaviour
{
    public FurnitureData furnitureData;
}
