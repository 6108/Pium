using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_InventorySlotRotate : MonoBehaviour
{
    //슬롯 천천히 회전
    void Update()
    {
        transform.Rotate(0, 0.4f, 0);
    }
}
