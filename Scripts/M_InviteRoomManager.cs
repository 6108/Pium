using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class M_InviteRoomManager : MonoBehaviourPun
{
    public MyRoomData myRoomData = new MyRoomData(); //파이어 베이스에서 가져온 정보를 담을 클래스
    public GameObject flowerPot;
    public GameObject inVase;
    public GameObject curWallpaper;
    public GameObject curFloor1;
    public GameObject curFloor2;
    public GameObject curDoor;
    public GameObject curFence;
    public GameObject curWindowFrame;

    string firebaseRoom;
    string auth; //플레이어 구분용
    DatabaseReference reference;

    public GameObject roomObject;

    private void Awake()
    {
        // 파이어베이스
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        if (PhotonNetwork.IsMasterClient)
            auth = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        else
            auth = PhotonNetwork.CurrentRoom.Name;
    }

    void Start()
    {
        //플레이어의 가구 모두 불러오기
        StartCoroutine(IELoadFirebaseRoom(auth));
    }

    private void Update()
    {
        if (firebaseRoom == null)
            StartCoroutine(IELoadFirebaseRoom(auth));
        else
            LoadRoom();
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
        // M_MyRoomInventory.instance.TypeSort();
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
        // M_MyRoomInventory.instance.TypeSort();
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
    }

    void LoadRoom()
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
            Destroy(roomObject.transform.GetChild(i).gameObject);
        for (int i = 0; i < myRoomData.furnitureList.Count; i++)
        {
            print(myRoomData.furnitureList[i].name);
            if (myRoomData.furnitureList[i].subType == "Vase")
            {
                furniture = KSR_InsVase.instance.Ins(int.Parse(myRoomData.furnitureList[i].name));
                if (!furniture.GetComponent<Furniture>())
                    furniture.AddComponent<Furniture>();
                furniture.GetComponent<Furniture>().furnitureData = myRoomData.furnitureList[i];
            }
            else if (myRoomData.furnitureList[i].subType == "FlowerPot")
            {

                //화분 소환 가능해지면 살려
                print("화분 이름: " + int.Parse(myRoomData.furnitureList[i].name));
                furniture = CreateFlowerPot.instance.CreatePot(int.Parse(myRoomData.furnitureList[i].name));
                furniture.AddComponent<BoxCollider>();
                furniture.GetComponent<BoxCollider>().size = furniture.transform.localScale;
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
        }

    }

    IEnumerator IELoadFirebaseRoom(string s)
    {
        var task = reference.Child("users").Child(s.Replace(".", "!")).Child("MyRoom").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted)
        {
            // MyRoom에 있는 데이터 불러오기
            DataSnapshot snapshot = task.Result;
            firebaseRoom = (string)snapshot.Value;
            print("로드 완료");
        }
        else
        {
            print("통신 실패");
        }
    }
}
