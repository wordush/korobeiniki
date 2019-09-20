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
    private float angle;

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
                Ray ray = new Ray(targetPos, Vector3.down);
                int layerMask = 1 << 8;
                layerMask = ~layerMask;
                Physics.Raycast(ray, out hit,Mathf.Infinity,layerMask);

                camRot = Quaternion.LookRotation(hit.point - camPos, Vector3.up);

                Scale -= Input.mouseScrollDelta.y * 0.1f;
                Scale = Mathf.Clamp(Scale,minScale,maxScale);
                camPos = hit.point + CameraPositionByHit * Scale;

                camObj.transform.position = Vector3.Lerp(camObj.transform.position, camPos , Time.deltaTime * 10);
                camObj.transform.rotation = Quaternion.Lerp(camObj.transform.rotation,camRot, Time.deltaTime * 10);
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
            targetPos.z -= Mathf.Cos(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
            targetPos.x -= Mathf.Sin(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
        }
        else
        if ((Screen.height - Input.mousePosition.y < moveSidesSize && Input.mousePosition.y < Screen.height) || (Input.GetKey(KeyCode.W)))// Up
        {
            targetPos.z += Mathf.Cos(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
            targetPos.x += Mathf.Sin(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
        }
        if ((Input.mousePosition.x < moveSidesSize && Input.mousePosition.x > 0) || (Input.GetKey(KeyCode.A))) // Left 
        {
            targetPos.z += Mathf.Sin(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
            targetPos.x -= Mathf.Cos(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
        }
        else
        if ((Screen.width - Input.mousePosition.x < moveSidesSize && Input.mousePosition.x < Screen.width) || (Input.GetKey(KeyCode.D)))// Right
        {
            targetPos.z -= Mathf.Sin(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
            targetPos.x += Mathf.Cos(angle * Mathf.Deg2Rad) * slideSensitivity * Scale * 0.1f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            angle += 180 * Time.deltaTime;
            camPos = RotateAround(camPos,new Vector3(hit.point.x,0, hit.point.z),180 * Time.deltaTime);
            CameraPositionByHit = camPos - hit.point;
        }
        else
        if (Input.GetKey(KeyCode.E))
        {
            angle -= 180 * Time.deltaTime;
            camPos = RotateAround(camPos, new Vector3(hit.point.x, 0, hit.point.z), -180 * Time.deltaTime);
            CameraPositionByHit = camPos - hit.point;
        }

    }

    public Vector3 RotateAround(Vector3 pos,Vector3 point,float angle)
    {
        Vector3 dir = pos - point;
        dir = Quaternion.Euler(new Vector3(0, angle, 0)) * dir;
        pos = dir + point;
        return pos;
    }
}
