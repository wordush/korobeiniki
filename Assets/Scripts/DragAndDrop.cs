using UnityEngine;
using System;
using Menu;
using UnityEngine.SceneManagement;

public class DragAndDrop : MonoBehaviour
{

    public Transform[] original;
    public Transform[] mask;
    public string respawnTag = "Respawn";

    public static bool isOn;
    private Transform _originalTmp;
    private Transform _maskTmp;
    private Vector3 _curPos;

    public int autoSpawnId;

    
    public void ToMenu(int id)
    {
        GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>().Load(id);
    }


    void Start()
    {
        isOn = false;
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }


    public void SetMask(int id)
    {
        foreach (Transform obj in original)
        {
            string name = obj.name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0];
            if (id.ToString() == name)
            {
                _originalTmp = Instantiate(obj, GameObject.FindWithTag("Buildings").transform);
                _originalTmp.gameObject.SetActive(false);
            }
        }

        foreach (Transform obj in mask)
        {
            string name = obj.name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0];
            if (id.ToString() == name)
            {
                _maskTmp = Instantiate(obj, GameObject.FindWithTag("Buildings").transform);
            }
        }
    }



    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
            SetMask(autoSpawnId);


        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        if (Physics.Raycast(ray, out hit,Mathf.Infinity,layerMask))
        {
            _curPos = hit.point;
        }

        if (_maskTmp)
        {
            _maskTmp.position = _curPos;

            if (Input.GetKey(KeyCode.X))
            {
                _maskTmp.localEulerAngles += new Vector3(0, 72 * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.Z))
            {
                _maskTmp.localEulerAngles -= new Vector3(0, 72 * Time.deltaTime, 0);
            }

            if ((Input.GetMouseButtonDown(0) && isOn) || (Input.GetKeyDown(KeyCode.U) && isOn))
            {
                _originalTmp.gameObject.SetActive(true);
                _originalTmp.position = _maskTmp.position;
                _originalTmp.localEulerAngles = _maskTmp.localEulerAngles;
                _originalTmp = null;
                Destroy(_maskTmp.gameObject);
                _maskTmp = null;
                isOn = false;
                //NavMes
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(_originalTmp.gameObject);
                Destroy(_maskTmp.gameObject);
            }
        }
    }
}
