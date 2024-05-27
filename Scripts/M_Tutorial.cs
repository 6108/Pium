using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class M_Tutorial : MonoBehaviour
{
    public GameObject guide; //���̵� ��ǳ��
    public Text guideText; //���̵� ��ǳ�� ���
    public int tutorialIndex = 0; //���� Ʃ�丮�� ��ȣ
    public GameObject curObject; //���� ����ִ� ����
    public GameObject roomObject; //���� �濡 ��ġ�� ������Ʈ���� �θ�
    public bool isSixNextDone; 
    public GameObject rightStickImage; //������ ���̽�ƽ �̹���
    public GameObject[] slots; //�κ��丮 ����

    //Ʃ�丮�� ��ŵ
    public GameObject skipCanvas; //��ŵ�� ĵ����
    
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
        //X������ ��ŵ
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
        guideText.text = "�ٽ� ���̷��̿���!\n������ ȭ�а� ȭ���� ��ġ�غ����?";
    }

    void Two()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = " ���̷��� Ŀ�ٶ� �������� ��ٸ��� ������� �����غ�����."; 
    }

    void Three()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "�ٹ̱� ��ɿ� ���Ծ��.\n�޼��� ��� �κ��丮�� Ȯ���غ����?";
        //Ŭ���ϸ� �Ѿ
    }

    void Four()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "���� ī�װ����� ���� ���� ����������.";
        //ȭ�� �� ������ �Ѿ
    }

    void Five()
    {
        rightStickImage.SetActive(true);
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "���� �ϳ��� ��� �ٴڿ� ���ƺ�����.\n���ڸ� ��� ������ ���̽�ƽ���� ȸ����ų �� �־��.";
        //ȭ���� �� ���¿��� ���̽�ƽ �����̸� �Ѿ
    }

    void Six()
    {
        rightStickImage.SetActive(false);
        curObject = null;
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "���� ���� ������ ��ġ�ǰ�\n��ġ�� ������ ������ ������ �� �־��.";
        //���� Ȯ�� ������ �Ѿ
    }

    public void ClickColorCheck()
    {
        if (tutorialIndex == 5)
            tutorialIndex++;
    }

    void SixNext()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "������ ���������?\n������ ���� ���¿��� �޼� ���������� ������������.";
    }

    void Seven()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "����� �� ��ư����\n���̷��� �����ϰų� �׸��� �� �־��.";
        //Ŭ������ �Ѿ
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
        guideText.text = "���� ��ư�� �ʱ�ȭ ��ư�̿���.\n��ư�� ���� ���� �ʱ�ȭ �غ�����.";
        //�ʱ�ȭ ������ �Ѿ
    }

    void Nine()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "����� �����ʿ� ��ġ�� ��ü���� ���� ģ���� ���� ���� �־��.";
    }

    void Ten()
    {
        guide.GetComponent<Animator>().SetTrigger("Next");
        guideText.text = "�׷� ���� �ǿ��� �����Ӱ� ��ܺ�����!";
        
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
        //Ŭ���ϸ� �� �̵�
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
