using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    public enum CameraMode
    {
        AdaptiveHightMode,
        FixedHeightMode
    }


    private Vector3 camPos;
    private Quaternion camRot;
    private GameObject camObj;
    public Vector3 CameraPositionByHit;
    
    private RaycastHit hit;
    private Vector3 targetPos;    
    
    private float Scale = 1;
    public float minScale;
    public float maxScale;

    public CameraMode mode;

    public float moveSidesSize;
    public float slideSensitivity;
    
    void Start()
    {
        camObj = this.gameObject;
        camPos = camObj.transform.position;
        camRot = camObj.transform.rotation;
        targetPos = Vector3.up * 100;
    }

    private void FixedUpdate()
    {
        switch (mode)
        {
            case CameraMode.AdaptiveHightMode:
                Physics.Raycast(new Ray(targetPos, Vector3.down), out hit);

                camRot = Quaternion.LookRotation(hit.point - camPos, Vector3.up);

                Scale -= Input.mouseScrollDelta.y * 0.1f;
                Scale = Mathf.Clamp(Scale,minScale,maxScale);
                camPos = hit.point + CameraPositionByHit * Scale;

                camObj.transform.position = Vector3.Lerp(camObj.transform.position, camPos , Time.deltaTime * 3);
                camObj.transform.rotation = Quaternion.Lerp(camObj.transform.rotation,camRot, Time.deltaTime * 3);
                break;
            case CameraMode.FixedHeightMode:
                Debug.LogError("Я это еще не доделал!!!, переключи mode на AdaptiveHightMode");
                break;
        }
    }

    void Update()
    {
        if ((Input.mousePosition.y < moveSidesSize && Input.mousePosition.y > 0) || (Input.GetKey(KeyCode.S)))// Down
        {
            targetPos.z -= slideSensitivity * Scale * 0.1f;
        }
        else
        if ((Screen.height - Input.mousePosition.y < moveSidesSize && Input.mousePosition.y < Screen.height) || (Input.GetKey(KeyCode.W)))// Up
        {
            targetPos.z += slideSensitivity * Scale * 0.1f;
        }
        if ((Input.mousePosition.x < moveSidesSize && Input.mousePosition.x > 0) || (Input.GetKey(KeyCode.A))) // Left 
        {
            targetPos.x -= slideSensitivity * Scale * 0.1f;
        }
        else
        if ((Screen.width - Input.mousePosition.x < moveSidesSize && Input.mousePosition.x < Screen.width) || (Input.GetKey(KeyCode.D)))// Right
        {
            targetPos.x += slideSensitivity * Scale * 0.1f;
        }
    }
}
