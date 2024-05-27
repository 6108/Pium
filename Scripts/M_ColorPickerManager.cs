using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ColorPickerManager : MonoBehaviour
{
    public GameObject model;
    public GameObject colorPickerPrefab;
    public List<GameObject> colorPickerList = new List<GameObject>();
    public List<Color> colorList = new List<Color>();
    public GameObject colorPickerCanvas;
    bool isColorChange;
    float time = 0;

    public static M_ColorPickerManager instance;
    public GameObject roomInventory;

    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            ChangeColorStart();
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            ChangeColor();
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            ChangeColorEnd();
        if (model)
            time += Time.deltaTime;
    }

    public void SetRectList(GameObject obj)
    {
        model = obj;
        M_PlaceManager.instance.saveCanvas.SetActive(false);
        for (int i = 0; i < colorPickerList.Count; i++)
        {
            Destroy(colorPickerList[i].gameObject);
        }
        colorPickerList.Clear();
        colorPickerCanvas.SetActive(true);
        model = obj;
        roomInventory.SetActive(false);
        if (!model.GetComponent<MeshRenderer>())
        {
            for (int i = 0; i < model.transform.GetChild(0).GetComponent<MeshRenderer>().materials.Length; i++)
            {
                GameObject colorPicker = Instantiate(colorPickerPrefab, transform);
                colorPicker.transform.localPosition = new Vector3(-1.2f * (model.transform.GetChild(0).GetComponent<MeshRenderer>().materials.Length - i), -0.5f, 0);

                colorPickerList.Add(colorPicker);
            }
        }
        //화분 안의 색은 바꿀 필요 없으니까 하나만 생성(원래 겉, 안 해서 두 개 생성됨)
        else if (model.GetComponent<Furniture>().furnitureData.subType == "FlowerPot")
        {
            GameObject colorPicker = Instantiate(colorPickerPrefab, transform);
            colorPicker.transform.localPosition = new Vector3(-1.2f, -0.5f, 0);
            colorPickerList.Add(colorPicker);
        }
        else
        {
            //색깔을 바꿀 모델 가져오기
            //모델의 머터리얼만큼 컬러 피커 판넬 하위에 컬러 피커 이미지 생성, 리스트에 담기
            for (int i = 0; i < model.GetComponent<MeshRenderer>().materials.Length; i++)
            {
                GameObject colorPicker = Instantiate(colorPickerPrefab, transform);
                colorPicker.transform.localPosition = new Vector3(-1.2f * (model.GetComponent<MeshRenderer>().materials.Length - i), -0.5f, 0);
                colorPickerList.Add(colorPicker);
                colorPicker.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = model.GetComponent<MeshRenderer>().materials[i].color;
            }
        }
    }

    void ChangeColorStart()
    {
        isColorChange = true;
    }

    /**색 변경*/
    void ChangeColor()
    {
        if (!isColorChange)
            return;
        if (time > 1)
        {
            SetRectList(model);
            time = 0;
        }
        //각 컬러 피커의 컬러를 리스트에 담기
        for (int i = 0; i < colorPickerList.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._startPoint, GameObject.Find("UIHelpers").transform.GetChild(0).GetComponent<LaserPointer>()._forward * 0.5f, out hit))
            {
                print(hit.transform.gameObject + ", " + colorPickerList[i].transform.gameObject);
                if (hit.transform.gameObject == colorPickerList[i].transform.gameObject)
                {
                    print("ColorPick");
                    Vector2 point = hit.textureCoord;
                    Texture2D texture = hit.transform.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                    int x = (int)(point.x * texture.width);
                    int y = (int)(point.y * texture.height);
                    print(x + ", " + y);
                    Color c = texture.GetPixel(x, y);
                    colorPickerList[i].transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
                    if (model.GetComponent<MeshRenderer>())
                        model.GetComponent<MeshRenderer>().materials[i].color = c;
                    else
                        model.transform.GetChild(0).GetComponent<MeshRenderer>().materials[i].color = c;
                }
            }
        }
    }

    void ChangeColorEnd()
    {
        isColorChange = false;
    }

    public void SaveColor()
    {
        model.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        M_PlaceManager.instance.SaveFurnitureData(model);
        if (model.GetComponent<MeshRenderer>())
        {
            for (int i = 0; i < model.GetComponent<MeshRenderer>().materials.Length; i++)
            {
                colorList.Add(model.GetComponent<MeshRenderer>().materials[i].color);
            }
        }
        else
        {
            for (int i = 0; i < model.transform.GetChild(0).GetComponent<MeshRenderer>().materials.Length; i++)
            {
                colorList.Add(model.transform.GetChild(0).GetComponent<MeshRenderer>().materials[i].color);
            }
        }
        ColorPickerOff();
        M_PlaceManager.instance.saveCanvas.SetActive(true);
        
    }

    public void ColorPickerOff()
    {
        colorPickerCanvas.SetActive(false);
        for (int i = 0; i < colorPickerList.Count; i++)
        {
            Destroy(colorPickerList[i].gameObject);
        }
        colorPickerList.Clear();
        transform.gameObject.SetActive(false);
        model = null;
    }
}
