using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation_MouseDrag : MonoBehaviour
{
    public float speed = 5;
    public float distance;
    public Transform cameraRotater;
    
    void OnMouseDown()
    {
        cameraRotater = GameObject.Find("CameraRotater").transform;
        if(Input.GetMouseButton(0)){
            distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
        }
    }
    
    private void OnMouseDrag()
    {
        Vector3 mouse_pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 obj_pos = Camera.main.ScreenToWorldPoint(mouse_pos);
        cameraRotater.Rotate(obj_pos.x,obj_pos.y,obj_pos.z);
    }
}
