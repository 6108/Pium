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
    public string wallPaper; //벽지
    public string floor1; //바닥재
    public string floor2;
    public string door;
    public string fence;
    public string windowFrame;
    public List<FurnitureData> furnitureList = new List<FurnitureData>(); //가구 정보
    public List<Vector3> furniturePosList = new List<Vector3>(); //가구 위치
    public List<Quaternion> furnitureRotList = new List<Quaternion>(); //가구 회전값
    public List<Vector3> furnitureScaleList = new List<Vector3>(); //가구 크기
    public List<FurnitureColor> furnitureColorList = new List<FurnitureColor>(); //가구 색깔
}

public class M_MyRoomManager : MonoBehaviour
{
    //방 정보 저장
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
    public GameObject saveImage; //저장완료 이미지. Player > CenterEye > SaveCanvas
    public Text saveText;
    public GameObject resetPanel;

    string auth;
    DatabaseReference reference;

    private void Awake()
    {
        instance = this;
        // 파이어베이스
        auth = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainScene" ) StartCoroutine(IELoadFirebaseRoom());
        resetPanel.SetActive(false);
        saveText.text = "저장하는 중";
        
    }

    IEnumerator IeStartDelay()
    {
        //서버통신 시간차로 인한 에러 임시방편용
        yield return new WaitForSeconds(1f);
        LoadRoom();
    }

    //마이룸 리스트에 가구 추가
    public void AddFurniture(GameObject furniture)
    {             
        Furniture f = furniture.GetComponent<Furniture>();
        if(!myRoomFurniture.Contains(f))
        {
            myRoomFurniture.Add(furniture.GetComponent<Furniture>());
            furniture.transform.SetParent(roomObject.transform);
        }
    }

    //벽지 교체
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

    //바닥1 교체
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

    //바닥2 교체
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

    //문 교체
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

    //창틀 교체
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

    //계단 교체
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
    
    //가구 삭제
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
    //현재 방 상태 저장
    public void SaveRoom()
    {
        StartCoroutine(IeSaveCheck());
        isSave = false;
        saveImage.SetActive(true);
        saveText.text = "저장하는 중";
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
                //MeshRenderer 있으면 바로 가져오고, 없으면 자식 1번째꺼 갖고옴(화분이 자식한테 달려있어서 추가)
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
            print("기본 데이터 저장 완료");
            isSave = true;
        }
        else
        {
            print("통신 실패");
        }
    }

    //저장 된 방 상태 불러오기
    public void LoadRoom()
    {
        print("LoadRoom");
        // 뉴비일 때 리턴
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

                //화분 소환 가능해지면 살려
                print("화분 이름: " + int.Parse(myRoomData.furnitureList[i].name));
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
            // MyRoom에 있는 데이터 불러오기
            DataSnapshot snapshot = task.Result;
            firebaseRoom = (string)snapshot.Value;
            if (firebaseRoom != null) LoadRoom();
            else yield break;
            print("로드 완료");
        }
        else
        {
            print("통신 실패");
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
        saveText.text = "저장 성공";
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
        saveText.text = "다시 시도해주세요";
        M_PlaceManager.instance.StartDecoration();
        yield return new WaitForSeconds(2f);
        saveImage.SetActive(false);
        
    }

    //저장 된 가구 제외하고 리스트에서 제거, 삭제
    public void ReturnRoomExit()
    {
        //if (M_ColorPickerManager.instance.colorPickerCanvas.S)
        for (int i = myRoomData.furnitureList.Count; i < myRoomFurniture.Count; i++)
            myRoomFurniture.RemoveAt(myRoomFurniture.Count - 1);
        for (int i = myRoomData.furnitureList.Count; i < roomObject.transform.childCount; i++)
            Destroy(roomObject.transform.GetChild(i).gameObject);
        M_PlaceManager.instance.StopDecoration();
    }

    //기본 가구만 남기고 다 인벤토리에 넣기
    public void ResetRoom()
    {
        //배치된 가구 모두 삭제하고 저장
        for (int i = 0; i < roomObject.transform.childCount; i++)
            Destroy(roomObject.transform.GetChild(i).gameObject);
        //리스트도 초기화
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
            // MyRoom에 있는 데이터 불러오기
            DataSnapshot snapshot = task.Result;
            reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoom").RemoveValueAsync();
            print("로드 완료");
        }
        else
        {
            print("통신 실패");
        }
    }
}
