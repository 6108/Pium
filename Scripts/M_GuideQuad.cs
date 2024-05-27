using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_GuideQuad : MonoBehaviour
{
    public static M_GuideQuad instance;

    private void Awake()
    {
        instance = this;
    }

    public bool isOK;
    private void OnTriggerEnter(Collider other)
    {
        isOK = false;
    }

    private void OnTriggerExit(Collider other)
    {
        isOK = true;
        GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.5f, 1f, 0.7f);
    }
}
