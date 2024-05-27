using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_Inventory : MonoBehaviour
{
    //�κ��丮 �⺻ ���
    public List<GameObject> inventoryItemList = new List<GameObject>(); //�κ��丮 �������� ���� ����Ʈ
    public GameObject inventory; //�κ��丮 �ֻ��� �θ�
    public float distance; //�κ��丮 ���� �� �÷��̾���� �Ÿ�
    bool isInventoryOn; //�κ��丮�� �����°�?

    //������ ����
    public Text itemName; //������ �̸�

    //������ ����
    public GameObject curItem; //���� ������
    float inputTime;

    //I�� ������ �κ��丮�� ����
    //�÷��̾� �߽����� ���� �׸����� �κ��丮 ������ ����
    //�κ��丮�� �¿�� �巡���ϸ� �������� �÷��̾� �߽����� ���ư�

    void Start()
    {
        for (int i = 0; i < 18; i++)
        {
            GameObject emptyObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            emptyObject.layer = LayerMask.NameToLayer("InventoryItem");
            emptyObject.transform.SetParent(inventory.transform);
            emptyObject.transform.position = transform.position + transform.forward * (5 + inventoryItemList.Count * distance);
            emptyObject.transform.forward = transform.forward;
            inventory.transform.Rotate(0, 20, 0);
        }
        InventoryOff();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (curItem)
                InteractiveItem();
        }
        if (Input.GetMouseButton(0))
        {
            if (curItem)
                MoveItem();
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                //������ �κ��丮�� �ֱ�
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Item"))
                {
                    ItemPutIn(hit.transform.gameObject);
                }
            }
            //�κ��丮 ������ �̸� ����
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("InventoryItem"))
            {
                itemName.enabled = true;
                Vector3 textPos = hit.transform.position;
                textPos.y += 1;
                itemName.transform.position = textPos;
                itemName.text = hit.transform.gameObject.name;
                itemName.transform.forward = transform.position;
            }
            //�κ��丮 ������ �̸� ����
            else if (itemName.isActiveAndEnabled)
                itemName.enabled = false;
        }
        //�κ��丮 ������ �̸� ����
        else if (itemName.isActiveAndEnabled)
            itemName.enabled = false;

        //�κ��丮�� ���������� ȸ��, ����
        if (isInventoryOn)
        {
            InventoryRotate();
            ItemPutOut();
        }
        //�κ��丮 ���� �ݱ�
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOn)
            {
                //�κ��丮 ����
                InventoryOff();
            }
            else
            {
                //�κ��丮 �ѱ�
                InventoryOn();
            }
        }

        //�κ��丮 �巡��
        if (Input.GetMouseButtonDown(0))
        {
            if (isInventoryOn)
                InventoryClick();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isInventoryOn)
                InventoryClickOff();
        }
    }

    //������ �巡�� �̵�
    void MoveItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        curItem.layer = LayerMask.NameToLayer("CurObject");
        int layerMask = 1 << LayerMask.NameToLayer("CurObject");
        if (Physics.Raycast(ray, out hit, 100, ~layerMask))
        {
            Vector3 hitPos = hit.point;
           /* hitPos.x = (int)hitPos.x;
            hitPos.y += 1;// + boundSize.y;
            hitPos.z = (int)hitPos.z;*/
            curItem.transform.position = hitPos;
        }
    }

    //������ ��ȣ�ۿ�
    void InteractiveItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("��ȣ�ۿ밡���� �����̸�"))
            {
                //�� ������ �´� �ڵ� ����
                return;
            }
        }
        ItemPutIn(curItem, 0);
        curItem = null;
        InventoryOn();
    }

    //�������� �κ��丮�� ����
    public void ItemPutIn(GameObject item, int num = -1)
    {
        if (!isInventoryOn)
        {
            if (num == -1)
                inventoryItemList.Add(item);
            else
                inventoryItemList.Insert(num, item);
            //inventoryItemList.Add(item);
            item.transform.SetParent(inventory.transform);
            item.SetActive(false);
            item.layer = LayerMask.NameToLayer("InventoryItem");
        }
    }

    //�κ��丮�� ������ ���̰�
    public void InventoryOn()
    {
        isInventoryOn = true;
        inventory.SetActive(true);
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            inventoryItemList[i].SetActive(true);
            inventoryItemList[i].transform.position = transform.position + transform.forward * (5 + inventoryItemList.Count * distance);
            inventoryItemList[i].transform.forward = transform.forward;
            inventory.transform.Rotate(0, -20, 0);
        }
        
    }

    //�κ��丮 ����
    public void InventoryOff()
    {
        inventory.transform.localRotation = new Quaternion(0, 0, 0, 0);
        inventory.SetActive(false);
        isInventoryOn = false;
    }

    public void ItemPutOut()
    {
        if (Input.GetMouseButton(0))
        {
            inputTime += Time.deltaTime;
            if (inputTime > 1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EmptyItem"))
                        return;
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("InventoryItem"))
                    {
                        curItem = hit.transform.gameObject;
                        curItem.transform.parent = null;
                        curItem.layer = LayerMask.NameToLayer("Item");
                        for (int i = 0; i < inventoryItemList.Count; i++)
                        {
                            if (curItem.name == inventoryItemList[i].name)
                            {
                                inventoryItemList.RemoveAt(i);
                            }
                        }
                        InventoryOff();
                    }
                }
                inputTime = 0;
            }
        }
    }
    //�κ��丮 Ŭ������ ��, �κ��丮 Ŭ�� ������ ���� ���콺 ��ġ�� �� ��ŭ ȸ��
    public Vector2 startMousePos;
    public Vector2 endMousePos;
    public void InventoryClick()
    {
        startMousePos = Input.mousePosition;
    }

    float h;
    //�κ��丮 ���ư���
    public void InventoryRotate()
    {
        if (h > -0.1 && h < 0.1)
        {

            return;
        }

        if (h > 0.1f)
        {
            h = Mathf.Lerp(0, h, 0.9f);
        }
        else if (h < -0.1f)
        {
            h = Mathf.Lerp(0, -h, 0.9f) * -1;
        }
        inventory.transform.RotateAround(transform.position, Vector3.up, h * Time.deltaTime);
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            Vector3 rot = inventoryItemList[i].transform.eulerAngles;
            if ((h < 1 && h > 0) || (h > -1 && h < 0))
                rot.z = h * 5;
            else
                rot.z = h / 5;
            inventoryItemList[i].transform.eulerAngles = rot;
        }
    }

    public void InventoryClickOff()
    {
        endMousePos = Input.mousePosition;
        h = startMousePos.x - endMousePos.x;
    }
}
