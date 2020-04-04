using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using GameStructure;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Mill : MonoBehaviour, IWorkStorage, IHaveName, IHaveFunctions
{
    [SerializeField] private int store;
    [SerializeField] private int capacity;
    [SerializeField] private float deliveryTriggerProcetage;
    public Level level;
    [HideInInspector] public Work work;

    List<KeyValuePair<string, UnityAction>> _functs;
    public List<KeyValuePair<string, UnityAction>> PublFunctions => _functs;

    public List<Vector3> wallPoints = new List<Vector3>();
    public bool _wallMode;

    public string Name => "Mill";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PeasanController>() && work.workerslocal.Contains(other.gameObject.GetComponent<PeasanController>()))
        {
            work.SetWorkerActiv(other.gameObject.GetComponent<PeasanController>(), true);
            // space for animation triggers (walk to work)
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PeasanController>() && work.activeWorkers.Contains(other.gameObject.GetComponent<PeasanController>()))
        {
            work.SetWorkerActiv(other.gameObject.GetComponent<PeasanController>(), false);
            // space for animation triggers (work to walk)
        }
    }

    public void Start()
    {
        _functs = new List<KeyValuePair<string, UnityAction>> {
            new KeyValuePair<string, UnityAction>("wall", drawWall)
        };
        work = GetComponent<Work>();
        work.WorkGoing += WorkGoing;
        //GameEvent.Tik += TikUpdate;
        GameEvent.LateTik += LateTikUpdate;
    }

    void LateTikUpdate()
    {
        foreach (PeasanController peasan in work.workerslocal)
        {
            switch (peasan.state)
            {
                case State.Work:
                        work.SetVorkerRest(peasan);
                    break;
                case State.Rest:
                        work.SetVorkerGo(work.destination.position, peasan);
                    break;
            }
        }
    }


    public void WorkGoing(int amount)
    {
        if (work.storage.IsItemExists(Item.Floor))
        {
            work.storage.items[Item.Floor] = Mathf.Clamp(work.storage.items[Item.Floor] + amount, 0, capacity);
            store = work.storage.items[Item.Floor];
        } else
        {
            work.storage.items.Add(Item.Floor, Mathf.Clamp(amount, 0, capacity));
            store = work.storage.items[Item.Floor];
        }
        
        if ((float)store / capacity > deliveryTriggerProcetage && !work.storage.ConsunationStarted)
        {

            GameEvent.FullOfItems(Item.Floor, 0, work.storage);
        }

    }
    
    public bool FullOrNot()
    {
        if (!work.storage.items.ContainsKey(Item.Floor))
            return false;
        if (work.storage.items[Item.Floor] >= capacity)
            return true;
        else
            return false;
    }

    public void Update()
    {
        if (_wallMode)
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (hit.collider.tag == "Finish")
                {
                    wallPoints.Add(hit.point);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && wallPoints.Count >= 1)
                wallPoints.Remove(wallPoints.Last());
            else 
                Debug.Log("There is no items in wall");

            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                wallPoints = new List<Vector3>();
                _wallMode = false;
            }
                
            
            drawWall(wallPoints, 0.1f);
        }
        
        if (work.storage.items.ContainsKey(Item.Floor))
            store = work.storage.items[Item.Floor];
        else store = 0;
    }

    public void LevelUp()
    {
        if (level < Level.Level5)
        {
            level += 1;
            UpdateValues(level);
        }
    }

    public void UpdateValues(Level lev)
    {
        switch (lev)
        {
            case Level.Level1:

                break;
            case Level.Level2:

                break;
            case Level.Level3:

                break;
            case Level.Level4:

                break;
            case Level.Level5:

                break;
        }
    }

    public void drawWall()
    {
        _wallMode = true;
    }

    public void drawWall(List<Vector3> points, float maxStep)
    {
        if (points.Count > 1)
        {
            points.Add(points.First());
            for (int i = 0; i < points.Count - 1; i++)
            {
                int layerMask = 1 << 8;
                layerMask = ~layerMask;
                int cols = (int)Math.Floor(Mathf.Abs(Vector3.Distance(points[i] , points[i + 1])) / maxStep);

                for (int j = 0; j < cols; j++)
                {
                    Debug.DrawLine((points[i] - points[i+1]) * j, (points[i] - points[i+1]) * j + Vector3.up);
                }
            }    
        }
        else if (points.Count == 1)
        {
            Debug.DrawLine(points[0], points[0] + Vector3.up);
        }
        
    }
}
