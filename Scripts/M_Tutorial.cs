using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class M_Tutorial : MonoBehaviour
{
    public GameObject guide; //가이드 말풍선
    public Text guideText; //가이드 말풍선 대사
    public int tutorialIndex = 0; //현재 튜토리얼 번호
    public GameObject curObject; //현재 들고있는 물건
    public GameObject roomObject; //현재 방에 배치된 오브젝트들의 부모
    public bool isSixNextDone; 
    public GameObject rightStickImage; //오른쪽 조이스틱 이미지
    public GameObject[] slots; //인벤토리 슬롯

    //튜토리얼 스킵
    public GameObject skipCanvas; //스킵용 캔버스
    
    GameObject fadeIn, fadeOut;
    int limit;

    public static M_Tutorial instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        fadeIn = Instantiate(Resources.Load("KSR/FadeIn") as GameObject, GameObject.Find("FadeIn/OutCanvas").transform);
        limit = 0;
        Destroy(fadeIn, 2.0f);
        StartCoroutine(IeTutorial());
    }
    
    void Update()
    {
        //X누르면 스킵
        if (OVRInput.GetDown(OVRInput.RawButton.X))
            skipCanvas.SetActive(true);

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            RaycastHit hit;
            if (tutorialIndex == 0 || tutorialIndex == 2 || tutorialIndex == 6 || tutorialIndex == 8 || tutorialIndex == 9)
                tutorialIndex++;
            if (Physics.Raycast(GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._startPoint, GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._forward * 0.5f, out hit))
            {
                print(hit.transform.name);
                if (hit.transform.name == "Bird" && tutorialIndex == 1)
                    tutorialIndex++;
                else if (hit.transform.GetComponent<Furniture>() && hit.transform.GetComponent<Furniture>().furnitureData.subType == "Chair")
                    curObject = hit.transform.gameObject;
            }
        }
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        if (input != new Vector2(0, 0) && curObject)
            tutorialIndex = 5;

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].transform.childCount > 0)
                {
                    for (int j = 0; j < slots[i].transform.GetChild(0).transform.GetComponents<BoxCollider>().Length; j++)
                        slots[i].transform.GetChild(0).transform.GetComponents<BoxCollider>()[j].enabled = false;
                }
            }
        }
    }

    IEnumerator IeTutorial()
    {
        One();
        yield return new WaitWhile(() => tutorialIndex < 1);
        Two();
        yield return new WaitWhile(() => tutorialIndex < 2);
        Three();
        yield return new WaitWhile(() => tutorialIndex < 3);
        Four();
        yield return new WaitWhile(() => tutorialIndex < 4);
        Five();
        yield return new WaitWhile(() => tutorialIndex < 5);
        Six();
        yield return new WaitWhile(() => tutorialIndex < 6);
        SixNext();
        yield return new WaitWhile(() => !isSixNextDone);
        Seven();
        yield return new WaitWhile(() => tutorialIndex < 7);
        Eight();
        yield return new WaitWhile(() => tutorialIndex < 8);
        Nine();
        yield return new WaitWhile(() => tutorialIndex < 9);
        Ten();
        yield return new WaitWhile(() => tutorialIndex < 10);
        Eleven();
        yield return new WaitWhile(() => tutorialIndex < 11);
    }
    
    public void SlotColliderOff()
    {

        if (M_MyRoomInventory.instance.curType != "Chair")
        {
            if (tutorialIndex < 6)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].transform.childCount > 0)
                    {
                        for (int j = 0; j < slots[i].transform.GetChild(0).transform.GetComponents<BoxCollider>().Length; j++)
                            slots[i].transform.GetChild(0).transform.GetComponents<BoxCollider>()[j].enabled = false;
                    }  
                }
            }
            else
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].transform.childCount > 0)
                    {
                        for (int j = 0; j < slots[i].transform.GetChild(0).transform.GetComponents<BoxCollider>().Length; j++)
                            slots[i].transform.GetChild(0).transform.GetComponents<BoxCollider>()[j].enabled = true;
                    }
                }
            }
        }
             
    }

    public void ClickButton()
    {
        if(tutorialIndex == 3)
            tutorialIndex = 4;
    }

    void One()
    {
        guideText.text = "다시 마이룸이에요!\n가져온 화분과 화병을 배치해볼까요?";
    }

    void Two()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = " 마이룸의 커다란 나무에서 기다리는 노랑새를 선택해보세요."; 
    }

    void Three()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "꾸미기 기능에 들어왔어요.\n왼손을 들어 인벤토리를 확인해볼까요?";
        //클릭하면 넘어감
    }

    void Four()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "가구 카테고리에서 의자 탭을 눌러보세요.";
        //화분 탭 누르면 넘어감
    }

    void Five()
    {
        rightStickImage.SetActive(true);
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "의자 하나를 들어 바닥에 놓아보세요.\n의자를 들고 오른손 조이스틱으로 회전시킬 수 있어요.";
        //화분을 든 상태에서 조이스틱 움직이면 넘어감
    }

    void Six()
    {
        rightStickImage.SetActive(false);
        curObject = null;
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "손을 떼면 가구가 배치되고\n배치된 가구는 색상을 설정할 수 있어요.";
        //색상 확인 누르면 넘어감
    }

    public void ClickColorCheck()
    {
        if (tutorialIndex == 5)
            tutorialIndex++;
    }

    void SixNext()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "가구를 지워볼까요?\n가구를 잡은 상태에서 왼손 휴지통으로 가져가보세요.";
    }

    void Seven()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "노랑새 옆 버튼으로\n마이룸을 저장하거나 그만둘 수 있어요.";
        //클릭으로 넘어감
    }

    public void ClickReset()
    {
        if (tutorialIndex == 7)
            tutorialIndex++;
        for (int i = 0; i < roomObject.transform.childCount; i++)
            Destroy(roomObject.transform.GetChild(i).gameObject);
        
    }

    void Eight()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "빨간 버튼은 초기화 버튼이에요.\n버튼을 눌러 방을 초기화 해보세요.";
        //초기화 누르면 넘어감
    }

    void Nine()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "노랑새 오른쪽에 위치한 우체통을 통해 친구를 만날 수도 있어요.";
    }

    void Ten()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "그럼 이제 피움을 자유롭게 즐겨보세요!";
        
    }

    public void ClickSkipYesButton()
    {
        if (limit == 0)
        {
            fadeOut = Instantiate(Resources.Load("KSR/FadeOut") as GameObject, GameObject.Find("FadeIn/OutCanvas").transform);
            limit++;
        }
        StartCoroutine(FadeOut());
    }

    public void ClickSkipNoButton()
    {
        skipCanvas.SetActive(false);
    }

    void Eleven()
    {
        //클릭하면 씬 이동
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (limit == 0)
            {
                fadeOut = Instantiate(Resources.Load("KSR/FadeOut") as GameObject, GameObject.Find("FadeIn/OutCanvas").transform);
                limit++;
            }
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MainScene");
    }
}
