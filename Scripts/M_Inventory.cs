using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_Inventory : MonoBehaviour
{
    //인벤토리 기본 기능
    public List<GameObject> inventoryItemList = new List<GameObject>(); //인벤토리 아이템을 담은 리스트
    public GameObject inventory; //인벤토리 최상위 부모
    public float distance; //인벤토리 켰을 때 플레이어와의 거리
    bool isInventoryOn; //인벤토리가 켜졌는가?

    //아이템 설명
    public Text itemName; //아이템 이름

    //아이템 빼기
    public GameObject curItem; //현재 아이템
    float inputTime;

    //I를 누르면 인벤토리가 켜짐
    //플레이어 중심으로 원을 그리도록 인벤토리 아이템 셋팅
    //인벤토리를 좌우로 드래그하면 아이템이 플레이어 중심으로 돌아감

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
                //아이템 인벤토리에 넣기
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Item"))
                {
                    ItemPutIn(hit.transform.gameObject);
                }
            }
            //인벤토리 아이템 이름 띄우기
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("InventoryItem"))
            {
                itemName.enabled = true;
                Vector3 textPos = hit.transform.position;
                textPos.y += 1;
                itemName.transform.position = textPos;
                itemName.text = hit.transform.gameObject.name;
                itemName.transform.forward = transform.position;
            }
            //인벤토리 아이템 이름 끄기
            else if (itemName.isActiveAndEnabled)
                itemName.enabled = false;
        }
        //인벤토리 아이템 이름 끄기
        else if (itemName.isActiveAndEnabled)
            itemName.enabled = false;

        //인벤토리가 열려있으면 회전, 가능
        if (isInventoryOn)
        {
            InventoryRotate();
            ItemPutOut();
        }
        //인벤토리 열고 닫기
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOn)
            {
                //인벤토리 끄기
                InventoryOff();
            }
            else
            {
                //인벤토리 켜기
                InventoryOn();
            }
        }

        //인벤토리 드래그
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

    //아이템 드래그 이동
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

    //아이템 상호작용
    void InteractiveItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("상호작용가능한 가구이면"))
            {
                //그 가구에 맞는 코드 실행
                return;
            }
        }
        ItemPutIn(curItem, 0);
        curItem = null;
        InventoryOn();
    }

    //아이템을 인벤토리에 넣음
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

    //인벤토리의 아이템 보이게
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

    //인벤토리 끄기
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
    //인벤토리 클릭했을 때, 인벤토리 클릭 끝났을 때의 마우스 위치를 뺀 만큼 회전
    public Vector2 startMousePos;
    public Vector2 endMousePos;
    public void InventoryClick()
    {
        startMousePos = Input.mousePosition;
    }

    float h;
    //인벤토리 돌아가게
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
