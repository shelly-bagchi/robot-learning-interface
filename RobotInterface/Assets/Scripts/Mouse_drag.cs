///////////////////////////////////////////////////////////////////////////////
//
//  Filename:        Mouse_drag.cs
//  Last Revision:   7/27/2021
//  Authors:         Esteban Segarra, Shelly Bagchi, Samay Govani
//
//  Description
//  ===========
//  Wrapper to move gameobjects in-game
//
///////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Mouse_drag : MonoBehaviour
{
    public Dropdown dropdown;
    public float distance;
    public float additive_ratio = 1.0f;
    public List<Vector3> positions = new List<Vector3>();
    public bool finished = false;

    public GameObject createSphere(Color color, Vector3 pos)
    {
        GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphererenderer = newObj.GetComponent<Renderer>();
        sphererenderer.material.SetColor("_Color", color);
        newObj.transform.position = pos;
        newObj.SetActive(false);
        return newObj;
    }

    public string Vector3_to_String(Vector3 val)
    {
        string retval;
        retval = val.x.ToString("F2") + ",";
        retval += val.y.ToString("F2") + ",";
        retval += val.z.ToString("F2");
        return retval;
    }

    public void Start()
    {
        dropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
    }

    private void Update()
    {

    }

    private void OnMouseDown()
    {
        distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
    }


    public Vector3 debug_vector;
    private void OnMouseDrag()
    {
        Vector3 mouse_pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 obj_pos = Camera.main.ScreenToWorldPoint(mouse_pos);
        debug_vector = obj_pos;
        transform.position = obj_pos;

        GameObject.Find("CoordX").GetComponent<InputField>().text = gameObject.transform.position.x.ToString("F2");
        GameObject.Find("CoordY").GetComponent<InputField>().text = gameObject.transform.position.y.ToString("F2");
        GameObject.Find("CoordZ").GetComponent<InputField>().text = gameObject.transform.position.z.ToString("F2");
    }

    private void OnMouseUp()
    {
        if (GameObject.Find("Edit").GetComponent<Toggle>().isOn)
        {
            dropdown.options[dropdown.value].text = Vector3_to_String(transform.position);
            positions[dropdown.value] = transform.position;
        }
        else
        {
            print("Position logged, " + debug_vector.ToString());
        }

    }


}
