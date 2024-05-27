using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ColorPicker : MonoBehaviour
{
    public GameObject model;
    public GameObject colorPickerPrefab;
    public List<GameObject> colorPickerList = new List<GameObject>();
    public List<Color> colorList = new List<Color>();

    void Update()
    {
        if (colorPickerList.Count <= 0)
        {
            SetRectList();
        }
        if (Input.GetMouseButtonDown(0))
        {
            ChangeColor();
        }
    }

    void SetRectList()
    {
        for (int i = 0; i < model.GetComponent<MeshRenderer>().materials.Length; i++)
        {
            GameObject colorPicker = Instantiate(colorPickerPrefab, model.transform);
            colorPickerList.Add(colorPicker);
        }
    }

    void ChangeColor()
    {
        for (int i = 0; i < colorPickerList.Count; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == colorPickerList[i].transform.gameObject)
                {
                    Vector2 point = hit.textureCoord;
                    Texture2D texture = hit.transform.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                    int x = (int)(point.x * texture.width);
                    int y = (int)(point.y * texture.height);
                    Color c = texture.GetPixel(x, y);
                    model.GetComponent<MeshRenderer>().materials[i].color = c;
                }
            }
        }
    }

    void SaveColor()
    {

        for (int i = 0; i < model.GetComponent<MeshRenderer>().materials.Length; i++)
        {
            colorList.Add(model.GetComponent<MeshRenderer>().materials[i].color);
        }
    }
}
