using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    private GameObject _camObj;
    private Vector3 _position;
    private Quaternion _rotationDelta;
    private float _angle = 0;
    private Camera _camera;

    private RaycastHit _hit;

    private float _scale = 1;
    private float _scaleLast;
    public float minScale;
    public float maxScale;
    
    public AnimationCurve cameraPositionCurve;
    
    public float slideSensitivity;

    public float midleMouseSens; 
    private Vector3 lastPositon;

    void Start()
    {
        _camera = Camera.main;
        _camObj = gameObject;
        _position = gameObject.transform.position;
        _rotationDelta = gameObject.transform.rotation;
    }

    private void Update()
    {
        Ray ray = new Ray(_position, eevec(_rotationDelta.eulerAngles));
        int layerMask = (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
        bool isRay = Physics.Raycast(ray, out _hit, Mathf.Infinity, layerMask);

        if (Input.GetKey(KeyCode.Q) && isRay)
        {
            _angle += 90 * Time.deltaTime;
            _position -= _position - RotateAround(_position, new Vector3(_hit.point.x, _position.y, _hit.point.z), 90 * Time.deltaTime);
        }
        else
        if (Input.GetKey(KeyCode.E) && isRay)
        {
            _angle -= 90 * Time.deltaTime;
            _position -= _position - RotateAround(_position, new Vector3(_hit.point.x, _position.y, _hit.point.z), -90 * Time.deltaTime);
        }
        
        _scale -= (Input.mouseScrollDelta.y * 0.1f);
        _scale = Mathf.Clamp(_scale, minScale, maxScale);
        
        Debug.Log((_scale - _scaleLast));
        
        _rotationDelta = Quaternion.LookRotation(_hit.point - _position, Vector3.up);

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.Mouse2))// Down
        {
            _position.z -= Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _position.x -= Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }
        else
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Mouse2))// Up
        {
            _position.z += Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _position.x += Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.Mouse2)) // Left 
        {
            _position.z += Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _position.x -= Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }
        else
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Mouse2))// Right
        {
            _position.z -= Mathf.Sin(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
            _position.x += Mathf.Cos(_angle * Mathf.Deg2Rad) * slideSensitivity * _scale * 0.1f;
        }

        if (Input.GetKey(KeyCode.Mouse2))
        {
            _position += RotateAround(Input.mousePosition - lastPositon,Vector3.up, _angle) / 30 * midleMouseSens;
        }


        ray = new Ray(_hit.point + (Vector3.up * 1000), Vector3.down);

        isRay = Physics.Raycast(ray, out _hit, Mathf.Infinity, layerMask);

        _position = _hit.point + (_position - _hit.point).normalized * _scale * 10;

        //_rotationDelta = Quaternion.Euler(_rotationDelta.eulerAngles.x * ((_scale - _scaleLast) * 0.5f + 1) ,_rotationDelta.eulerAngles.y,_rotationDelta.eulerAngles.z);
        
        lastPositon = Input.mousePosition;


        _camObj.transform.rotation = Quaternion.Lerp(_camObj.transform.rotation, _rotationDelta, Time.deltaTime * 10);
        _camObj.transform.position = Vector3.Lerp(_camObj.transform.position, _position, Time.deltaTime * 10);
    }

    public Vector3 RotateAround(Vector3 pos,Vector3 point,float angle)
    {
        Vector3 dir = pos - point;
        dir = Quaternion.Euler(new Vector3(0, angle, 0)) * dir;
        pos = dir + point;
        return pos;
    }

    public Vector3 eevec(Vector3 eul)
    {
        return Quaternion.Euler(eul) * Vector3.forward;
    }
}
