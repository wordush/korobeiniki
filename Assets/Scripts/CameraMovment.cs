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


    private Vector3 _camPos;
    private Quaternion _camRot;
    private GameObject _camObj;
    public Vector3 _cameraPositionByHit;
    private float _angle;

    private RaycastHit _hit;
    private Vector3 _targetPos;    
    
    private float _scale = 1;
    public float minScale;
    public float maxScale;



    public float moveSidesSize;
    public float slideSensitivity;
    
    void Start()
    {
        _camObj = this.gameObject;
        _camPos = _camObj.transform.position;
        _camRot = _camObj.transform.rotation;
        _targetPos = Vector3.up * 100;
    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(_targetPos, Vector3.down);
        int layerMask = (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
        Physics.Raycast(ray, out _hit, Mathf.Infinity, layerMask);

        _camRot = Quaternion.LookRotation(_hit.point - _camPos, Vector3.up);

        _scale -= Input.mouseScrollDelta.y * 0.1f;
        _scale = Mathf.Clamp(_scale,minScale,maxScale);
        _camPos = _hit.point + _cameraPositionByHit * _scale;

        _camObj.transform.position = Vector3.Lerp(_camObj.transform.position, _camPos , Time.deltaTime * 10);
        _camObj.transform.rotation = Quaternion.Lerp(_camObj.transform.rotation,_camRot, Time.deltaTime * 10);


        if ((Input.mousePosition.y < moveSidesSize && Input.mousePosition.y > 0) || (Input.GetKey(KeyCode.S)))// Down
        {
            _targetPos.z -= Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _targetPos.x -= Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }
        else
        if ((Screen.height - Input.mousePosition.y < moveSidesSize && Input.mousePosition.y < Screen.height) || (Input.GetKey(KeyCode.W)))// Up
        {
            _targetPos.z += Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _targetPos.x += Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }
        if ((Input.mousePosition.x < moveSidesSize && Input.mousePosition.x > 0) || (Input.GetKey(KeyCode.A))) // Left 
        {
            _targetPos.z += Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _targetPos.x -= Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }
        else
        if ((Screen.width - Input.mousePosition.x < moveSidesSize && Input.mousePosition.x < Screen.width) || (Input.GetKey(KeyCode.D)))// Right
        {
            _targetPos.z -= Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _targetPos.x += Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            _angle += 90 * Time.deltaTime;
            _cameraPositionByHit = RotateAround(_cameraPositionByHit, new Vector3(0, _cameraPositionByHit.y, 0), 90 * Time.deltaTime);
        }
        else
        if (Input.GetKey(KeyCode.E))
        {
            _angle -= 90 * Time.deltaTime;
            _cameraPositionByHit = RotateAround(_cameraPositionByHit, new Vector3(0, _cameraPositionByHit.y, 0), -90 * Time.deltaTime);
        }

    }

    void Update()
    {
        
    }

    public Vector3 RotateAround(Vector3 pos,Vector3 point,float angle)
    {
        Vector3 dir = pos - point;
        dir = Quaternion.Euler(new Vector3(0, angle, 0)) * dir;
        pos = dir + point;
        return pos;
    }
}
