using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;

[System.Serializable]
public class FurnitureColor
{
    public List<Color> colorList = new List<Color>();
}

[System.Serializable]
public class MyRoomData
{
    public string wallPaper; //����
    public string floor1; //�ٴ���
    public string floor2;
    public string door;
    public string fence;
    public string windowFrame;
    public List<FurnitureData> furnitureList = new List<FurnitureData>(); //���� ����
    public List<Vector3> furniturePosList = new List<Vector3>(); //���� ��ġ
    public List<Quaternion> furnitureRotList = new List<Quaternion>(); //���� ȸ����
    public List<Vector3> furnitureScaleList = new List<Vector3>(); //���� ũ��
    public List<FurnitureColor> furnitureColorList = new List<FurnitureColor>(); //���� ����
}

public class M_MyRoomManager : MonoBehaviour
{
    //�� ���� ����
    public MyRoomData myRoomData = new MyRoomData();
    public GameObject myRoomInventory;
    public GameObject roomObject;
    public static M_MyRoomManager instance;
    public List<Furniture> myRoomFurniture = new List<Furniture>();
    public GameObject curWallpaper;
    public GameObject curFloor1;
    public GameObject curFloor2;
    public GameObject curDoor;
    public GameObject curFence;
    public GameObject curWindowFrame;
    public GameObject saveImage; //����Ϸ� �̹���. Player > CenterEye > SaveCanvas
    public Text saveText;
    public GameObject resetPanel;

    string auth;
    DatabaseReference reference;

    private void Awake()
    {
        instance = this;
        // ���̾�̽�
        auth = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainScene" ) StartCoroutine(IELoadFirebaseRoom());
        resetPanel.SetActive(false);
        saveText.text = "�����ϴ� ��";
        
    }

    IEnumerator IeStartDelay()
    {
        //������� �ð����� ���� ���� �ӽù����
        yield return new WaitForSeconds(1f);
        LoadRoom();
    }

    //���̷� ����Ʈ�� ���� �߰�
    public void AddFurniture(GameObject furniture)
    {             
        Furniture f = furniture.GetComponent<Furniture>();
        if(!myRoomFurniture.Contains(f))
        {
            myRoomFurniture.Add(furniture.GetComponent<Furniture>());
            furniture.transform.SetParent(roomObject.transform);
        }
    }

    //���� ��ü
    public void ChangeWallpaper(GameObject wallpaper)
    {
        if (curWallpaper == wallpaper)
            return;
        wallpaper.transform.parent = null;
        wallpaper.transform.position = curWallpaper.transform.position;
        wallpaper.transform.rotation = curWallpaper.transform.rotation;
        wallpaper.transform.localScale = curWallpaper.transform.localScale;
        Destroy(curWallpaper);
        curWallpaper = wallpaper;
        M_MyRoomInventory.instance.TypeSort();
    }

    //�ٴ�1 ��ü
    public void ChangeFloor1(GameObject floor1)
    {
        if (curFloor1 == floor1)
            return;
        floor1.transform.parent = null;
        floor1.transform.position = curFloor1.transform.position;
        floor1.transform.rotation = curFloor1.transform.rotation;
        floor1.transform.localScale = curFloor1.transform.localScale;
        Destroy(curFloor1);
        curFloor1 = floor1;
        M_MyRoomInventory.instance.TypeSort();
    }

    //�ٴ�2 ��ü
    public void ChangeFloor2(GameObject floor2)
    {
        if (curFloor2 == floor2)
            return;
        floor2.transform.parent = null;
        floor2.transform.position = curFloor2.transform.position;
        floor2.transform.rotation = curFloor2.transform.rotation;
        floor2.transform.localScale = curFloor2.transform.localScale;
        Destroy(curFloor2);
        curFloor2 = floor2;
        M_MyRoomInventory.instance.TypeSort();
    }

    //�� ��ü
    public void ChangeDoor(GameObject door)
    {
        if (curDoor == door)
            return;
        door.transform.parent = null;
        door.transform.position = curDoor.transform.position;
        door.transform.rotation = curDoor.transform.rotation;
        door.transform.localScale = curDoor.transform.localScale;
        Destroy(curDoor);
        curDoor = door;
        M_MyRoomInventory.instance.TypeSort();
    }

    //âƲ ��ü
    public void ChangeWindowFrame(GameObject windowFrame)
    {
        if (curWindowFrame == windowFrame)
            return;
        windowFrame.transform.parent = null;
        windowFrame.transform.position = curWindowFrame.transform.position;
        windowFrame.transform.rotation = curWindowFrame.transform.rotation;
        windowFrame.transform.localScale = curWindowFrame.transform.localScale;
        Destroy(curWindowFrame);
        curWindowFrame = windowFrame;
        M_MyRoomInventory.instance.TypeSort();
    }

    //��� ��ü
    public void ChangeFence(GameObject fence)
    {
        if (curFence == fence)
            return;
        fence.transform.parent = null;
        fence.transform.position = curFence.transform.position;
        fence.transform.rotation = curFence.transform.rotation;
        fence.transform.localScale = curFence.transform.localScale;
        Destroy(curFence);
        curFence = fence;
        M_MyRoomInventory.instance.TypeSort();
    }
    
    //���� ����
    public void DeleteFurniture(GameObject furniture)
    {
        for (int i = 0; i < roomObject.transform.childCount; i++)
        {
            if (roomObject.transform.GetChild(i).gameObject == furniture)
            {
                myRoomFurniture.RemoveAt(i);
                break;
            }
        }
        print("22539rig3i17");
        Destroy(furniture.gameObject);
        M_ColorPickerManager.instance.ColorPickerOff();
    }

    bool isSave;
    //���� �� ���� ����
    public void SaveRoom()
    {
        StartCoroutine(IeSaveCheck());
        isSave = false;
        saveImage.SetActive(true);
        saveText.text = "�����ϴ� ��";
        myRoomData = new MyRoomData();
        myRoomData.wallPaper = curWallpaper.name.Split('(')[0];
        myRoomData.floor1 = curFloor1.name.Split('(')[0];
        myRoomData.floor2 = curFloor2.name.Split('(')[0];
        myRoomData.door = curDoor.name.Split('(')[0];
        myRoomData.fence = curFence.name.Split('(')[0];
        myRoomData.windowFrame = curWindowFrame.name.Split('(')[0];
        if (myRoomFurniture.Count > 0)
        {
            for (int i = 0; i < roomObject.transform.childCount; i++)
            {
                myRoomData.furnitureList.Add(myRoomFurniture[i].furnitureData);
                myRoomData.furniturePosList.Add(myRoomFurniture[i].transform.localPosition);
                myRoomData.furnitureRotList.Add(myRoomFurniture[i].transform.localRotation);
                myRoomData.furnitureScaleList.Add(myRoomFurniture[i].transform.localScale);
                FurnitureColor furnitureColor = new FurnitureColor();
                //MeshRenderer ������ �ٷ� ��������, ������ �ڽ� 1��°�� �����(ȭ���� �ڽ����� �޷��־ �߰�)
                if (myRoomFurniture[i].GetComponent<MeshRenderer>())
                {
                    for (int j = 0; j < myRoomFurniture[i].GetComponent<MeshRenderer>().materials.Length; j++)
                        furnitureColor.colorList.Add(myRoomFurniture[i].GetComponent<MeshRenderer>().materials[j].color);
                }
                else
                {
                    for (int j = 0; j < myRoomFurniture[i].transform.GetChild(0).GetComponent<MeshRenderer>().materials.Length; j++)
                        furnitureColor.colorList.Add(myRoomFurniture[i].transform.GetChild(0).GetComponent<MeshRenderer>().materials[j].color);
                }
                myRoomData.furnitureColorList.Add(furnitureColor);
            }
        }
        string json = JsonUtility.ToJson(myRoomData);
        runningCoroutine = StartCoroutine(IESaveRoom(json));
        // SaveRoom(json);
    }

    IEnumerator IeSaveCheck()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            SaveFail();
        }
    }
    Coroutine runningCoroutine;

    IEnumerator IESaveRoom(string s)
    {
        var task = reference.Child("users").Child(auth.Replace(".", "!")).GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.IsCompleted)
        {
            reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoom").SetValueAsync(s);
            SaveSuccess();
            print("�⺻ ������ ���� �Ϸ�");
            isSave = true;
        }
        else
        {
            print("��� ����");
        }
    }

    //���� �� �� ���� �ҷ�����
    public void LoadRoom()
    {
        print("LoadRoom");
        // ������ �� ����
        if (firebaseRoom == null)
            return;
        myRoomData = JsonUtility.FromJson<MyRoomData>(firebaseRoom);
        GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Door/" + myRoomData.door));
        ChangeDoor(furniture);
        furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Floor1/" + myRoomData.floor1));
        ChangeFloor1(furniture);
        furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Floor2/" + myRoomData.floor2));
        ChangeFloor2(furniture);
        furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/WindowFrame/" + myRoomData.windowFrame));
        ChangeWindowFrame(furniture);
        furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Wall/" + myRoomData.wallPaper));
        ChangeWallpaper(furniture);
        furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/Background/Fence/" + myRoomData.fence));
        ChangeFence(furniture);
        for (int i = 0; i < roomObject.transform.childCount; i++)
        {
            Destroy(roomObject.transform.GetChild(i).gameObject);
        }
        myRoomFurniture.Clear();
        for (int i = 0; i < myRoomData.furnitureList.Count; i++)
        {
            print(myRoomData.furnitureList[i].name);
            if (myRoomData.furnitureList[i].subType == "Vase")
            {
                furniture = KSR_InsVase.instance.Ins(int.Parse(myRoomData.furnitureList[i].name));
                if (!furniture.GetComponent<Furniture>())
                    furniture.AddComponent<Furniture>();
                furniture.GetComponent<Furniture>().furnitureData = myRoomData.furnitureList[i];
                furniture.GetComponent<BoxCollider>().size = furniture.GetComponent<Renderer>().bounds.size;
                furniture.GetComponent<BoxCollider>().center = furniture.GetComponent<Renderer>().bounds.size / 2;
            }
            else if (myRoomData.furnitureList[i].subType == "FlowerPot")
            {

                //ȭ�� ��ȯ ���������� ���
                print("ȭ�� �̸�: " + int.Parse(myRoomData.furnitureList[i].name));
                furniture = CreateFlowerPot.instance.CreatePot(int.Parse(myRoomData.furnitureList[i].name));
                if (!furniture.GetComponent<BoxCollider>())
                {
                    furniture.AddComponent<BoxCollider>();
                    furniture.GetComponent<BoxCollider>().size = furniture.transform.lossyScale;
                }
                
                if (!furniture.GetComponent<Furniture>())
                    furniture.AddComponent<Furniture>();
                furniture.GetComponent<Furniture>().furnitureData = myRoomData.furnitureList[i];
            }
            else
            {
                string furnitureName = myRoomData.furnitureList[i].type + "/" + myRoomData.furnitureList[i].subType + "/" + myRoomData.furnitureList[i].name;
                furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/" + furnitureName));
                if (!furniture.GetComponent<Furniture>())
                {
                    furniture.AddComponent<Furniture>();
                }
                furniture.GetComponent<Furniture>().furnitureData = myRoomData.furnitureList[i];
            }

            furniture.transform.localPosition = myRoomData.furniturePosList[i];
            furniture.transform.localRotation = myRoomData.furnitureRotList[i];
            furniture.transform.localScale = myRoomData.furnitureScaleList[i];
            for (int j = 0; j < furniture.GetComponent<MeshRenderer>().materials.Length; j++)
            {
                print(myRoomData.furnitureColorList[i].colorList[j]);
                furniture.GetComponent<MeshRenderer>().materials[j].color = myRoomData.furnitureColorList[i].colorList[j];
            }
            furniture.transform.SetParent(roomObject.transform);
            furniture.layer = LayerMask.NameToLayer("Furniture");
            myRoomFurniture.Add(furniture.GetComponent<Furniture>());
        }
    }

    string firebaseRoom;
    IEnumerator IELoadFirebaseRoom()
    {
        yield return new WaitForSeconds(0.5f);
        var task = reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoom").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted)
        {
            // MyRoom�� �ִ� ������ �ҷ�����
            DataSnapshot snapshot = task.Result;
            firebaseRoom = (string)snapshot.Value;
            if (firebaseRoom != null) LoadRoom();
            else yield break;
            print("�ε� �Ϸ�");
        }
        else
        {
            print("��� ����");
        }
    }

    public void SaveRoomExit()
    {
        SaveRoom();
        M_PlaceManager.instance.StopDecoration();
    }

    void SaveSuccess()
    {
        runningCoroutine = null;
        StartCoroutine(IeSaveSuccess());
    }

    IEnumerator IeSaveSuccess()
    {
        saveImage.SetActive(true);
        saveText.text = "���� ����";
        yield return new WaitForSeconds(1f);
        saveImage.SetActive(false);
    }

    void SaveFail()
    {
        StartCoroutine(IeSaveFail());
    }

    IEnumerator IeSaveFail()
    {
        saveImage.SetActive(true);
        saveText.text = "�ٽ� �õ����ּ���";
        M_PlaceManager.instance.StartDecoration();
        yield return new WaitForSeconds(2f);
        saveImage.SetActive(false);
        
    }

    //���� �� ���� �����ϰ� ����Ʈ���� ����, ����
    public void ReturnRoomExit()
    {
        //if (M_ColorPickerManager.instance.colorPickerCanvas.S)
        for (int i = myRoomData.furnitureList.Count; i < myRoomFurniture.Count; i++)
            myRoomFurniture.RemoveAt(myRoomFurniture.Count - 1);
        for (int i = myRoomData.furnitureList.Count; i < roomObject.transform.childCount; i++)
            Destroy(roomObject.transform.GetChild(i).gameObject);
        M_PlaceManager.instance.StopDecoration();
    }

    //�⺻ ������ ����� �� �κ��丮�� �ֱ�
    public void ResetRoom()
    {
        //��ġ�� ���� ��� �����ϰ� ����
        for (int i = 0; i < roomObject.transform.childCount; i++)
            Destroy(roomObject.transform.GetChild(i).gameObject);
        //����Ʈ�� �ʱ�ȭ
        myRoomFurniture.Clear();
        StartCoroutine(IEDeleteRoomData());
    }

    public void ResetPanelOn()
    {
        resetPanel.SetActive(true);
    }

    public void ResetPanelOff()
    {
        resetPanel.SetActive(false);
    }

    IEnumerator IEDeleteRoomData()
    {
        var task = reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoom").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted)
        {
            // MyRoom�� �ִ� ������ �ҷ�����
            DataSnapshot snapshot = task.Result;
            reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoom").RemoveValueAsync();
            print("�ε� �Ϸ�");
        }
        else
        {
            print("��� ����");
        }
    }
}
