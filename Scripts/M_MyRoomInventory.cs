using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;


public class M_MyRoomInventory : MonoBehaviour
{
    //데이터
    public List<FurnitureData> myFurnitureList = new List<FurnitureData>(); //모든 가구
    public List<FurnitureData> curFurnitureList = new List<FurnitureData>(); //현재 인벤토리에 보여야하는 가구

    //인벤토리
    public string curType = "Bed"; //현재 타입
    public GameObject[] subType; //하위 카테고리 칸
    public GameObject[] slots; //인벤토리 칸

    //꽃 Object 저장
    string auth;
    DatabaseReference reference;

    //인벤토리 페이지
    int min;
    int max;
    int page = 1;
    public Text pageText;

    public static M_MyRoomInventory instance;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        instance = this;
        StartCoroutine(IELoadFurnitureData()); 
    }

    public void ResetPage()
    {
        curFurnitureList = myFurnitureList.FindAll(x => x.subType == curType);
        page = 1;
        min = 0;
        if (curFurnitureList.Count >= 12)
            max = 12;
        else
            max = curFurnitureList.Count;
        ChangePageText();
    }

    public void PrevPage()
    {
        curFurnitureList = myFurnitureList.FindAll(x => x.subType == curType);
        if (page == 1)
            return;
        page--;
        min = min - 12;
        if (max % 12 == 0)
            max = (max / 12 - 1) * 12;
        else
            max = max / 12 * 12;
        TypeSort();
        ChangePageText();
    }

    public void NextPage()
    {
        curFurnitureList = myFurnitureList.FindAll(x => x.subType == curType);
        if (max == curFurnitureList.Count)
            return;
        page++;
        min = max;
        if (curFurnitureList.Count - min < 12)
            max = curFurnitureList.Count;
        else
            max = min + 12;
        TypeSort();
        ChangePageText();
    }

    void ChangePageText()
    {
        if (curFurnitureList.Count % 12 == 0)
            pageText.text = page + "/" + curFurnitureList.Count / 12;
        else
            pageText.text = page + "/" + (curFurnitureList.Count / 12 + 1);
    }

    //만약 저장된 가구가 없으면 기본 가구 모두 불러와서 리스트에 저장
    //저장된 가구가 있으면 로드

    //현재 타입이 해당 타입으로 바뀌고 가구목록을 재정렬한다
    public void ClickTypeButton(string type)
    {
        curType = type;
        page = 1;
        min = 0;
        max = 12;
        ResetPage();
        TypeSort();
        
    }

    public void SubTypeOff(int num)
    {
        //타른 타입의 서브 타입 안보이게
        for (int i = 0; i < subType.Length; i++)
        {
            if (i == num)
                subType[i].SetActive(true);
            else
                subType[i].SetActive(false);
        }
    }

    //Type에 맞춰 가구 정렬
    public void TypeSort()
    {
        //타입명이 같은 객체들을 전체 가구리스트에서 찾아, 현재 가구 리스트에 넣음
        curFurnitureList = myFurnitureList.FindAll(x => x.subType == curType);
        CreateInventoryItem();
    }

    //현재 타입의 가구들만 생성
    void CreateInventoryItem()
    {
        //슬롯 하위 객체들 모두 삭제
        for (int i = 0; i < 12; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
                slots[i].GetComponent<MeshRenderer>().enabled = true;
            }
        }
        if (curType == "Vase")
        {
            GameObject furniture;
            for (int i = min; i < max; i++)
            {
                furniture = KSR_InsVase.instance.Ins(int.Parse(curFurnitureList[i].name));
                if (KSR_InsVase.instance.isLoad)
                {
                    furniture.AddComponent<Rigidbody>();
                    //가구 컴포넌트넣긔
                    if (!furniture.GetComponent<Furniture>())
                    {
                        furniture.AddComponent<Furniture>();
                        furniture.GetComponent<Furniture>().furnitureData = curFurnitureList[i];
                    }
                    //콜라이더 넘 작아서 임시로 바꿈
                    if (curType == "Vase")
                    {
                        furniture.GetComponent<BoxCollider>().size = furniture.GetComponent<Renderer>().bounds.size;
                        furniture.GetComponent<BoxCollider>().center = furniture.GetComponent<Renderer>().bounds.size / 2;
                    }

                    //RigidBody 없으면 Raycast가 부모를 가리켜서 그거 방지
                    furniture.AddComponent<Rigidbody>();
                    furniture.GetComponent<Rigidbody>().useGravity = false;
                    furniture.GetComponent<Rigidbody>().isKinematic = true;

                    furniture.transform.localScale /= 8;
                    if (i >= 12)
                    {
                        furniture.transform.position = slots[i - 12 * (page - 1)].transform.position;
                        slots[i - 12 * (page - 1)].GetComponent<MeshRenderer>().enabled = false;
                        furniture.transform.SetParent(slots[i - 12 * (page - 1)].transform);
                    }
                    else
                    {
                        furniture.transform.position = slots[i].transform.position;
                        slots[i].GetComponent<MeshRenderer>().enabled = false;
                        furniture.transform.SetParent(slots[i].transform);
                    }
                }
            }
        }
        else if (curType == "FlowerPot")
        {
            GameObject furniture;
            for (int i = min; i < max; i++)
            {
                furniture = CreateFlowerPot.instance.CreatePot(int.Parse(curFurnitureList[i].name));
                furniture.AddComponent<BoxCollider>();
                furniture.GetComponent<BoxCollider>().size = furniture.transform.localScale;
                if (!furniture.GetComponent<Furniture>())
                {
                    furniture.AddComponent<Furniture>();
                    furniture.GetComponent<Furniture>().furnitureData = curFurnitureList[i];
                }
                //RigidBody 없으면 Raycast가 부모를 가리켜서 그거 방지
                furniture.AddComponent<Rigidbody>();
                furniture.GetComponent<Rigidbody>().useGravity = false;
                furniture.GetComponent<Rigidbody>().isKinematic = true;

                furniture.transform.localScale /= 8;
                if (i >= 12)
                {
                    furniture.transform.position = slots[i - 12 * (page - 1)].transform.position;
                    slots[i - 12 * (page - 1)].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i - 12 * (page - 1)].transform);
                }
                else
                {
                    furniture.transform.position = slots[i].transform.position;
                    slots[i].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i].transform);
                }
            }
        }
        else if (curType == "Wall" || curType == "Fence" || curType == "Floor1" || curType == "Floor2"  || curType == "WindowFrame")
        {
            for (int i = min; i < max; i++)
            {
                string furnitureName = curFurnitureList[i].type + "/" + curFurnitureList[i].subType + "/" + curFurnitureList[i].name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/" + furnitureName));
                //RigidBody 없으면 Raycast가 부모를 가리켜서 그거 방지
                furniture.AddComponent<Rigidbody>();
                furniture.GetComponent<Rigidbody>().useGravity = false;
                furniture.GetComponent<Rigidbody>().isKinematic = true;
                if (!furniture.GetComponent<Furniture>())
                    furniture.AddComponent<Furniture>();
                furniture.GetComponent<Furniture>().furnitureData = curFurnitureList[i];
                Vector3 furnitureSize = furniture.transform.localScale;
                float max = furniture.GetComponent<Renderer>().bounds.size.x;
                if (max < furniture.GetComponent<Renderer>().bounds.size.y)
                    max = furniture.GetComponent<Renderer>().bounds.size.y;
                if (max < furniture.GetComponent<Renderer>().bounds.size.z)
                    max = furniture.GetComponent<Renderer>().bounds.size.z;
                furnitureSize.x /= max;
                furnitureSize.y /= max;
                furnitureSize.z /= max;
                furniture.transform.localScale = furnitureSize / 8;
                if (i >= 12)
                {
                    furniture.transform.position = slots[i - 12 * (page - 1)].transform.position;
                    slots[i - 12 * (page - 1)].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i - 12 * (page - 1)].transform);
                }
                else
                {
                    furniture.transform.position = slots[i].transform.position;
                    slots[i].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i].transform);
                }
                if (curType == "WindowFrame")
                    AddCollider(furniture);
                if (!furniture.GetComponent<BoxCollider>())
                    AddCollider(furniture);
            }
        }
        else if (curType == "Door")
        {
            for (int i = min; i < max; i++)
            {
                
                string furnitureName = curFurnitureList[i].type + "/" + curFurnitureList[i].subType + "/" + curFurnitureList[i].name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/" + furnitureName));
                //문 축 때문에 인벤토리 튀어나오는 거 방지
                furniture.transform.GetChild(0).transform.position = new Vector3(0, 0, 0);
                furniture.transform.GetChild(0).transform.GetChild(0).position = new Vector3(0, 0, 0);

                //furniture = furniture.transform.GetChild(0).transform.GetChild(0).gameObject;
                //RigidBody 없으면 Raycast가 부모를 가리켜서 그거 방지
                furniture.AddComponent<Rigidbody>();
                furniture.GetComponent<Rigidbody>().useGravity = false;
                furniture.GetComponent<Rigidbody>().isKinematic = true;
                if (!furniture.GetComponent<Furniture>())
                    furniture.AddComponent<Furniture>();
                furniture.GetComponent<Furniture>().furnitureData = curFurnitureList[i];
                furniture.transform.localScale /= 15;
                if (i >= 12)
                {
                    furniture.transform.position = slots[i - 12 * (page - 1)].transform.position;
                    slots[i - 12 * (page - 1)].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i - 12 * (page - 1)].transform);
                }
                else
                {
                    furniture.transform.position = slots[i].transform.position;
                    slots[i].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i].transform);
                }
                if (!furniture.GetComponent<BoxCollider>())
                    AddCollider(furniture.transform.GetChild(0).transform.GetChild(0).gameObject);
            }
        }
        else
        {
            for (int i = min; i < max; i++)
            {
                print(min);
                print(max);
                string furnitureName = curFurnitureList[i].type + "/" + curFurnitureList[i].subType + "/" + curFurnitureList[i].name;
                GameObject furniture = Instantiate(Resources.Load<GameObject>("Furnitures_FIN/" + furnitureName));
                //RigidBody 없으면 Raycast가 부모를 가리켜서 그거 방지
                furniture.AddComponent<Rigidbody>();
                furniture.GetComponent<Rigidbody>().useGravity = false;
                furniture.GetComponent<Rigidbody>().isKinematic = true;
                if (!furniture.GetComponent<Furniture>())
                    furniture.AddComponent<Furniture>();
                furniture.GetComponent<Furniture>().furnitureData = curFurnitureList[i];
                furniture.layer = LayerMask.NameToLayer("InventoryFurniture");
                Vector3 furnitureSize = furniture.transform.localScale;
                float maxSize = furniture.GetComponent<Renderer>().bounds.size.x;
                if (maxSize < furniture.GetComponent<Renderer>().bounds.size.y)
                    maxSize = furniture.GetComponent<Renderer>().bounds.size.y;
                if (maxSize < furniture.GetComponent<Renderer>().bounds.size.z)
                    maxSize = furniture.GetComponent<Renderer>().bounds.size.z;
                furnitureSize.x /= maxSize;
                furnitureSize.y /= maxSize;
                furnitureSize.z /= maxSize;
                furniture.transform.localScale = furnitureSize / 8;
                if (i >= 12)
                {
                    furniture.transform.position = slots[i - 12 * (page - 1)].transform.position;
                    slots[i - 12 * (page - 1)].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i - 12 * (page - 1)].transform);
                }
                else
                {
                    furniture.transform.position = slots[i].transform.position;
                    slots[i].GetComponent<MeshRenderer>().enabled = false;
                    furniture.transform.SetParent(slots[i].transform);
                }
                if (!furniture.GetComponent<BoxCollider>())
                    AddCollider(furniture);
            }
        }
        if (M_Tutorial.instance)
        {
            StartCoroutine(aaa());
        }
    }

    IEnumerator aaa()
    {
        yield return null;
        M_Tutorial.instance.SlotColliderOff();
    }

    void AddCollider(GameObject obj)
    {
        obj.AddComponent<BoxCollider>();
        float sizeX = 2f / obj.transform.localScale.x;
        float sizeY = 2f / obj.transform.localScale.y;
        float sizeZ = 2f / obj.transform.localScale.z;
        obj.GetComponent<BoxCollider>().size = new Vector3(sizeX, sizeY, sizeZ);
    }

    public void AddVase(FurnitureData vase)
    {
        myFurnitureList.Add(vase);
        SaveFurnitureData(myFurnitureList);
        // isSave = false;
    }
                                                                                                                                                                                                                                                                    
    string myRoomJson;

    // Firebase 로드
    IEnumerator IELoadFurnitureData()
    {
        var task = reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoomInventory").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);
  
        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            myRoomJson = (string)snapshot.Value;
            if (myRoomJson == null)
            {
                LoadDefaltFurnitureData();
            }
            else
            {
                LoadFurnitureData();
            }
            print("로드 완료");
        }
        else
        {
            print("통신 실패");
        }
    }

    //가구 데이터 불러오기
    void LoadFurnitureData()
    {
        myFurnitureList = JsonConvert.DeserializeObject<List<FurnitureData>>(myRoomJson);
        SaveFurnitureData(myFurnitureList);
    }

    //불러올 가구가 없을 때 사용
    //기본 가구 모두 불러오고 저장
    void LoadDefaltFurnitureData()
    {
        string furnitureData = Resources.Load<TextAsset>("AllFurnitureList").text;
        string[] lines = furnitureData.Split("\n");
        for (int i = 1; i < lines.Length; i++)
        {
            string[] variable = lines[i].Split(",");
            FurnitureData furniture = new FurnitureData();
            furniture.type = variable[0];
            furniture.subType = variable[1];
            furniture.name = variable[2];
            furniture.info = variable[3];
            myFurnitureList.Add(furniture);
        }
        SaveFurnitureData(myFurnitureList);
    }

    //가구 데이터 저장
    void SaveFurnitureData(List<FurnitureData> s)
    {
        string json = JsonConvert.SerializeObject(s);
        reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoomInventory").SetValueAsync(json);
    }
    bool isSave = false;
    void IESaveSome(string s)
    {
        reference.Child("users").Child(auth.Replace(".", "!")).GetValueAsync().ContinueWith(
            task =>
            {
                if (task.IsCompleted)
                {
                    reference.Child("users").Child(auth.Replace(".", "!")).Child("MyRoomInventory").SetValueAsync(s);
                    print("기본 데이터 저장 완료");
                    isSave = true;
                }
                else
                {
                    print("통신실패");
                }
            });
    }
}