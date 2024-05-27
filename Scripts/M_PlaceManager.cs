using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class M_PlaceManager : MonoBehaviour
{
    public GameObject curFurniture; //���� ���� 
    public int layerNum; //���� ������ ���� ���̾� ��ȣ
    Vector3 startPos; //������ ���� �ִ� ��ġ
    public GameObject guideQuad; //���̵���ο�
    public GameObject myRoomInventory;
    public static M_PlaceManager instance;
    public GameObject saveCanvas;
    public GameObject UIHelper;

    //��������
    public GameObject trashCanvas;
    public bool isSmall;
    public Vector3 beforeScale;

    //�� �ٹ̴� ���� �� �ٸ� ������Ʈ ��ȣ�ۿ� ����
    public GameObject postBox;

    bool isDecorateTime; //�����ٹ̴����ΰ�?
    bool isChangeColorTime; //���� �ٲٴ� ���ΰ�?

    float rotTime;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        myRoomInventory.SetActive(false);
        guideQuad.SetActive(false);
        trashCanvas.SetActive(false);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (!curFurniture)
                ShootRay();
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (curFurniture)
                MoveFurniture();
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (curFurniture)
                PlaceFurniture();
        }

        if (curFurniture)
        {
            trashCanvas.SetActive(true);
            Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            rotTime += Time.deltaTime;
            if (rotTime > 0.2f)
            {
                if (input.x > 0)
                    RotateFurniture(1);
                else if (input.x < 0)
                    RotateFurniture(-1);
                rotTime = 0;
            }
        }
    }

    void DeleteFurniture()
    {
        print("11111111111");
        if (!curFurniture)
            return;
        if (M_Tutorial.instance)
            M_Tutorial.instance.isSixNextDone = true;
        print("222222222");
        M_MyRoomManager.instance.DeleteFurniture(curFurniture);
        print("33333333333");
        curFurniture = null;
        guideQuad.SetActive(false);
        trashCanvas.SetActive(false);
        myRoomInventory.SetActive(true);
        
    }

    //���� ���� ��� ��Ҵ��� üũ
    void ShootRay()
    {
        if (isChangeColorTime)
            return;
        if (!GameObject.Find("UIHelpers") || !GameObject.Find("UIHelpers").activeSelf)
            return;
        RaycastHit hit;
        if (Physics.Raycast(GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._startPoint, GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._forward * 0.5f, out hit))
        {
            if (hit.transform.name == "Bird" && !isDecorateTime)
                StartDecoration();
            if (!isDecorateTime)
                return;
            //���߿� ~���� �±װ� �ƴϸ�~���� �ٲٱ�
            if (hit.transform.name == "Floor" || hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
                return;
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("InventoryFurniture"))
            {
                //�κ��丮 ���� ���� ����
                FurnitureData funitureData = hit.transform.GetComponent<Furniture>().furnitureData;
                string furnitureName = funitureData.type + "/" + funitureData.subType + "/" + funitureData.name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/" + furnitureName));
                if (!furniture.GetComponent<Furniture>())
                    furniture.AddComponent<Furniture>();
                furniture.GetComponent<Furniture>().furnitureData = hit.transform.GetComponent<Furniture>().furnitureData;
                furniture.transform.position = hit.transform.position;
                curFurniture = furniture;
                curFurniture.layer = LayerMask.NameToLayer("CurObject");
                layerNum = curFurniture.layer;
                myRoomInventory.SetActive(false);
                GuideQuadOn();
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Furniture"))
            {
                //Ŭ���� ������Ʈ 
                startPos = hit.transform.position;
                curFurniture = hit.transform.gameObject;
                layerNum = curFurniture.layer;
                curFurniture.layer = LayerMask.NameToLayer("CurObject");
                myRoomInventory.SetActive(false);
                trashCanvas.SetActive(true);
                
                GuideQuadOn();
                Vector3 guideQuadRot = curFurniture.transform.eulerAngles;
                guideQuadRot.x += 90;
                guideQuad.transform.eulerAngles = guideQuadRot;
                M_ColorPickerManager.instance.ColorPickerOff();
            }
            else if (hit.transform.gameObject.name.Contains("Wall"))
            {
                if (hit.transform.gameObject == M_MyRoomManager.instance.curWallpaper)
                    return;
                string furnitureName = hit.transform.GetComponent<Furniture>().furnitureData.name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Wall/" + furnitureName));
                M_MyRoomManager.instance.ChangeWallpaper(furniture);
                Destroy(hit.transform.gameObject);
            }
            else if (hit.transform.gameObject.name.Contains("Door"))
            {
                if (hit.transform.gameObject == M_MyRoomManager.instance.curDoor)
                    return;
                string furnitureName = hit.transform.GetComponent<Furniture>().furnitureData.name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Door/" + furnitureName));
                M_MyRoomManager.instance.ChangeDoor(furniture);
                Destroy(hit.transform.gameObject);
            }
            else if (hit.transform.gameObject.name.Contains("Floor1"))
            {
                if (hit.transform.gameObject == M_MyRoomManager.instance.curFloor1)
                    return;
                string furnitureName = hit.transform.GetComponent<Furniture>().furnitureData.name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Floor1/" + furnitureName));
                M_MyRoomManager.instance.ChangeFloor1(furniture);
                Destroy(hit.transform.gameObject);
            }
            else if (hit.transform.gameObject.name.Contains("Floor2"))
            {
                if (hit.transform.gameObject == M_MyRoomManager.instance.curFloor2)
                    return;
                string furnitureName = hit.transform.GetComponent<Furniture>().furnitureData.name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Floor2/" + furnitureName));
                M_MyRoomManager.instance.ChangeFloor2(furniture);
                Destroy(hit.transform.gameObject);
            }
            else if (hit.transform.gameObject.name.Contains("Fence"))
            {
                if (hit.transform.gameObject == M_MyRoomManager.instance.curFence)
                    return;
                string furnitureName = hit.transform.GetComponent<Furniture>().furnitureData.name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Fence/" + furnitureName));
                M_MyRoomManager.instance.ChangeFence(furniture);
                Destroy(hit.transform.gameObject);
            }
            else if (hit.transform.gameObject.name.Contains("WindowFrame"))
            {
                if (hit.transform.gameObject == M_MyRoomManager.instance.curWindowFrame)
                    return;
                string furnitureName = hit.transform.GetComponent<Furniture>().furnitureData.name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/WindowFrame/" + furnitureName));
                M_MyRoomManager.instance.ChangeWindowFrame(furniture);
                Destroy(hit.transform.gameObject);
            }
            else if (hit.transform.gameObject.name.Contains("Vase"))
            {
                GameObject furniture = KSR_InsVase.instance.Ins(int.Parse(hit.transform.GetComponent<Furniture>().furnitureData.name));
                if (!furniture.GetComponent<Furniture>())
                {
                    furniture.AddComponent<Furniture>();
                    furniture.GetComponent<Furniture>().furnitureData = hit.transform.GetComponent<Furniture>().furnitureData;
                }
                //�ݶ��̴� �� �۾Ƽ� �ӽ÷� �ٲ�
                furniture.GetComponent<BoxCollider>().size = furniture.GetComponent<Renderer>().bounds.size;
                furniture.GetComponent<BoxCollider>().center = furniture.GetComponent<Renderer>().bounds.size/2;

                furniture.transform.position = hit.transform.position;
                curFurniture = furniture;
                curFurniture.layer = LayerMask.NameToLayer("CurObject");
                layerNum = curFurniture.layer;
                myRoomInventory.SetActive(false);
                GuideQuadOn();
            }
            else if (hit.transform.gameObject.name.Contains("FlowerPot"))
            {
                GameObject furniture = CreateFlowerPot.instance.CreatePot(int.Parse(hit.transform.GetComponent<Furniture>().furnitureData.name));
                if (!furniture.GetComponent<Furniture>())
                {
                    furniture.AddComponent<Furniture>();
                    furniture.GetComponent<Furniture>().furnitureData = hit.transform.GetComponent<Furniture>().furnitureData;
                }
                furniture.transform.rotation = Quaternion.Euler(0, 0, 0);
                //furniture.transform.localScale /= 3;
                furniture.transform.position = hit.transform.position;
                curFurniture = furniture;
                curFurniture.layer = LayerMask.NameToLayer("CurObject");
                layerNum = curFurniture.layer;
                myRoomInventory.SetActive(false);
                GuideQuadOn();
            }
        }
    }

    public void StartDecoration()
    {
        saveCanvas.SetActive(true);
        isDecorateTime = true;
        myRoomInventory.SetActive(true);
        M_MyRoomInventory.instance.ResetPage();
        M_MyRoomInventory.instance.TypeSort();
        //��ü�� ��ȣ�ۿ� �ȵǰ� �ݶ��̴� ���ֱ�
        postBox.GetComponent<BoxCollider>().enabled = false;
    }

    public void StopDecoration()
    {
        saveCanvas.SetActive(false);
        isDecorateTime = false;
        myRoomInventory.SetActive(false);
        //��ü�� ��ȣ�ۿ� �ǰ� �ݶ��̴� ���ֱ�
        postBox.GetComponent<BoxCollider>().enabled = true;
    }

    //���� �̵���Ű��
    void MoveFurniture()
    {
        if (!isDecorateTime || isChangeColorTime )
            return;
        //�̵���ų �� ��ġ Ȯ��
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("CurObject");

        if (Physics.Raycast(GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._startPoint, GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._forward * 0.5f, out hit, 100, ~layerMask))
        {
            Vector3 hitPos = hit.point;
            if (hit.transform.name == "TrashCan")
            {
                if (!isSmall)
                {
                    beforeScale = curFurniture.transform.localScale;

                    Vector3 furnitureSize = curFurniture.transform.localScale;
                    float maxSize = curFurniture.GetComponent<Renderer>().bounds.size.x;
                    if (maxSize < curFurniture.GetComponent<Renderer>().bounds.size.y)
                        maxSize = curFurniture.GetComponent<Renderer>().bounds.size.y;
                    if (maxSize < curFurniture.GetComponent<Renderer>().bounds.size.z)
                        maxSize = curFurniture.GetComponent<Renderer>().bounds.size.z;
                    furnitureSize.x /= maxSize;
                    furnitureSize.y /= maxSize;
                    furnitureSize.z /= maxSize;
                    curFurniture.transform.localScale = furnitureSize / 8;
                    isSmall = true;
                }
                curFurniture.GetComponent<BoxCollider>().enabled = false;
                guideQuad.SetActive(false);
                curFurniture.transform.position = hitPos;
                return;
            }
            else
            {
                if (isSmall)
                {
                    curFurniture.transform.localScale = beforeScale;
                    isSmall = false;
                }
                curFurniture.GetComponent<BoxCollider>().enabled = true;
                guideQuad.SetActive(true);
            }
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                guideQuad.SetActive(false);
            }
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Item"))
                    return;
                guideQuad.SetActive(true);
                hitPos.y -= 0.5f;
                print("Quad: " + guideQuad.transform.position);
                print(hitPos);
                if (guideQuad.transform.position != hitPos)
                    guideQuad.GetComponent<Rigidbody>().velocity = (hitPos - guideQuad.transform.position) * 20f;
            }
            hitPos = guideQuad.transform.position;
            hitPos.y += 0.99f;
            if (hitPos.y > 7)
                hitPos.y = 7;
            if (!curFurniture.GetComponent<Rigidbody>())
            {
                curFurniture.AddComponent<Rigidbody>();
                curFurniture.GetComponent<Rigidbody>().freezeRotation = true;
            }
            else
            {
                curFurniture.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                curFurniture.GetComponent<Rigidbody>().freezeRotation = true;
                curFurniture.GetComponent<Rigidbody>().isKinematic = false;
            }
            if (curFurniture.transform.position != hitPos)
                curFurniture.GetComponent<Rigidbody>().velocity = (hitPos - curFurniture.transform.position) * 20f;
        }
    }

    void GuideQuadOn()
    {
        //���� ������ 
        Quaternion curFurnitureRot = curFurniture.transform.rotation;
        curFurniture.transform.rotation = Quaternion.Euler(0, 0, 0);
        Vector3 boundSize;
        if (curFurniture.GetComponent<Renderer>())
            boundSize = curFurniture.GetComponent<Renderer>().bounds.size;
        else
            boundSize = curFurniture.transform.localScale;
        curFurniture.transform.rotation = curFurnitureRot;
        //���̵� ����
        //����� x, y�� �־ z�� ���̸� y�� �־���� ��
        guideQuad.SetActive(true);
        guideQuad.transform.localScale = new Vector3(boundSize.x, boundSize.z, 1);
    }

    //���� ��ġ�� ��������
    void PlaceFurniture()
    {
        if (!isDecorateTime || isChangeColorTime)
            return;
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("CurObject");
        if (Physics.Raycast(GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._startPoint, GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._forward * 0.5f, out hit, 100, ~layerMask))
        {
            //���� �������� �̹������ ����
            if (hit.transform.name == "TrashCan")
            {
                DeleteFurniture();
                return;
            }
            if (curFurniture.GetComponent<Rigidbody>())
            {
                curFurniture.GetComponent<Rigidbody>().useGravity = true;
                curFurniture.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                curFurniture.GetComponent<Rigidbody>().freezeRotation = true;
            }
        }
        guideQuad.transform.rotation = Quaternion.Euler(90, 0, 0);
        guideQuad.SetActive(false);
        curFurniture.layer = LayerMask.NameToLayer("Furniture");
        Vector3 placePos = curFurniture.transform.position;
        //�������� ����
        trashCanvas.SetActive(false);
        //�� �ٲٱ�
        print(curFurniture);
        M_ColorPickerManager.instance.transform.gameObject.SetActive(true);
        M_ColorPickerManager.instance.SetRectList(curFurniture);
        curFurniture = null;
    }

    public void SaveFurnitureData(GameObject furniture)
    {
        //�� ���� ������ �ֱ�
        print(furniture);
        isChangeColorTime = false;
        M_MyRoomManager.instance.AddFurniture(furniture);
        myRoomInventory.SetActive(true);
    }

    void RotateFurniture(int i)
    {
        if (i > 0)
        {
            curFurniture.transform.Rotate(0, 15, 0);
            Vector3 guideQuadRot = curFurniture.transform.eulerAngles;
            guideQuadRot.x += 90;
            guideQuad.transform.eulerAngles = guideQuadRot;
        }
            
        else if (i < 0)
        {
            curFurniture.transform.Rotate(0, -15, 0);
            Vector3 guideQuadRot = curFurniture.transform.eulerAngles;
            guideQuadRot.x += 90;
            guideQuad.transform.eulerAngles = guideQuadRot;
        }
        else
            return;
    }
}
