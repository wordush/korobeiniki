using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Events;
using GameStructure;
using UnityEditor;
using UnityEngine.Serialization;

public class Sawmill : MonoBehaviour , IWorkStorage, IHaveName , IHaveDescription, IHaveFunctions
{
    [SerializeField] private int store;
    [SerializeField] private int capacity;
    [SerializeField] private float deliveryTriggerProcetage;
    [SerializeField] private int energyCutdown;
    [SerializeField] private int energyCutup;
    public Level level;
    [HideInInspector] public Work work;

    List<KeyValuePair<string, UnityAction>> _funct;
    public List<KeyValuePair<string, UnityAction>> PublFunctions => _funct;

    public List<GameObject> level2;

    public string Name => name;

    public string descript;
    public string Description => descript;

    private Vector3 _zone1S;
    private Vector3 _zoneTempE;
    private bool _selectZone;

    public GameObject[] allTrees;
    public List<GameObject> treesInProcess;
    private bool _fSelected;

    public List<GameObject> baulks;
    public Transform Brewno;

    public Dictionary<GameObject, KeyValuePair<PeasanController, PeasanController>> _baulkDelivery;

    public List<GameObject> trees;



    private void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.GetComponent<PeasanController>() && work.Workers.Contains(other.gameObject.GetComponent<PeasanController>()))
        // {
        //     work.SetWorkerActiv(other.gameObject.GetComponent<PeasanController>(), true);
        //     // space for animation triggers (walk to work)
        // }
    }

    private void OnTriggerExit(Collider other)
    {
        // if (other.gameObject.GetComponent<PeasanController>() && work.activeWorkers.Contains(other.gameObject.GetComponent<PeasanController>()))
        // {
        //     work.SetWorkerActiv(other.gameObject.GetComponent<PeasanController>(), false);
        //     // space for animation triggers (work to walk)
        // }
    }

    public void Start()
    {
        _zone1S = Vector3.zero;
        _selectZone = false;
        baulks = new List<GameObject>();
        _baulkDelivery = new Dictionary<GameObject, KeyValuePair<PeasanController, PeasanController>>();

        _funct = new List<KeyValuePair<string, UnityAction>> {
            new KeyValuePair<string, UnityAction>("LevelUp", LevelUp),
            new KeyValuePair<string, UnityAction>("Select zone", SelectZone)
        };
        foreach (GameObject g in level2)
            g.SetActive(false);
        work = GetComponent<Work>();

        GameEvent.LateTik += LateTikUpdate;
        GameEvent.Tik += TikUpdate;
        work.done += DoneHandler;

        allTrees = GameObject.FindGameObjectsWithTag("Tree");
    }

    void TikUpdate()
    {
        foreach (PeasanController peas in work.Workers)
        {
            if (peas.state == State.Work && trees.Count > 0)
            {
                GameObject tree = trees.First();
                work.SetVorkerGo(tree, peas);
                peas.Temprary = tree; // Запсиь дерева во временую переменную крестьянина 
                treesInProcess.Add(trees.First());
                trees.Remove(tree);
                peas.state = State.Work1C;
            }
        }
    }
    
    /*
      Work1C - Срубил дерево, понес его обратно в лесопилку
      Work2C - Донес дерево
     */
    
    public void DoneHandler(PeasanController peasan)
    {
        Debug.Log($"{peasan.name} : state - {peasan.state} - Task done");
        switch (peasan.state)
        {
            case State.Work1C:
                StartCoroutine(nameof(TreeCut), peasan);
                break;
            case State.Work2C:
                Destroy(peasan.Temprary);
                peasan.state = State.Work;
                if (work.storage.items.ContainsKey(Item.Wood))
                    work.storage.items[Item.Wood] += 10;
                else 
                    work.storage.items.Add(Item.Wood, 10);
                break;
        }
    }

    IEnumerator TreeCut(PeasanController peasan)
    {
        // animation trigger
        
        
        yield return new WaitForSeconds(2f); // wait for animation ends

        peasan.state = State.Work2C;
        peasan.agent.enabled = true;
        treesInProcess.Remove(peasan.Temprary);
        allTrees = allTrees.Where(val =>  val != peasan.Temprary).ToArray();
        Destroy(peasan.Temprary);
        Transform b = Instantiate(Brewno, peasan.Temprary.transform.position, Quaternion.identity);
        b.GetComponent<Rigidbody>().AddTorque(new Vector3(0.3f,0.3f,0.3f));
        peasan.Temprary = b.gameObject;
        
        //wait for tree falls
        
        yield return new WaitForSeconds(6f);


        peasan.GetComponent<CharacterJoint>().connectedBody = b.GetComponent<Rigidbody>();
        work.SetVorkerGo(work.destination.gameObject , peasan);
        
    }

    void LateTikUpdate()
    {
        foreach (PeasanController peasan in work.Workers)
        {
            switch (peasan.state)
            {
                case State.Work:
                    if (peasan.energy <= energyCutdown)
                    {
                        work.SetVorkerRest(peasan);
                        peasan.state = State.Rest;
                    }
                    break;
            
            
                case State.Rest:
                    if (peasan.energy >= energyCutup)
                    {
                        work.SetVorkerGo(work.destination.gameObject, peasan);
                        peasan.state = State.Work;
                    }
                    break;
            }
        }
    }

    public bool FullOrNot()
    {
        if (!work.storage.items.ContainsKey(Item.Wood))
            return false;
        if (work.storage.items[Item.Wood] >= capacity)
            return true;
        return false;
    }

    public void Update()
    {
        if (work.storage.items.ContainsKey(Item.Wood))
            store = work.storage.items[Item.Wood];
        else store = 0;

        GameObject[] temp;

        if (_selectZone)
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && !_fSelected && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // Первое нажатие
                    _zone1S = hit.point;
                    _fSelected = true;
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // Второе нажатие
                    _fSelected = false;
                    ClickHandler.ClickHandlerActiv = true;
                    _selectZone = false;

                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && _selectZone)
                _zoneTempE = hit.point;


            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _selectZone = false;
                trees = new List<GameObject>();
                ClickHandler.ClickHandlerActiv = true;
            }

            if (!_fSelected && _selectZone)
            {
                // Первое нажатие
                Debug.DrawRay(_zoneTempE, Vector3.up);
                temp = new GameObject[0];
            }
            else
            {
                // Второе нажатие

                DrawWall(_zone1S, _zoneTempE, 0.1f);
                temp = ClickHandler.GetObjectsInZone(_zone1S, _zoneTempE);
            }

            trees = new List<GameObject>();
            foreach (GameObject obj in temp)
            {
                if (obj.CompareTag("Tree"))
                {
                    if (!trees.Contains(obj))
                        trees.Add(obj);
                }
            }
        }

        if (SelectedObject.Get() == gameObject)
            foreach (GameObject tre in allTrees)
            {
                if (trees.Contains(tre) || treesInProcess.Contains(tre))
                {
                    tre.GetComponent<TreeLife>().MakeRed();
                }
                else
                {
                    tre.GetComponent<TreeLife>().MakeNat();
                    trees.Remove(tre);
                }
            }
        else
            foreach (GameObject tre in allTrees)
            {
                tre.GetComponent<TreeLife>().MakeNat();
            }

    }


    public void LevelUp()
    {
        if (level < Level.Level5)
        {
            level += 1;
            UpdateValues(level);
        }
    }

    public void SelectZone()
    {
        ClickHandler.ClickHandlerActiv = false;
        _selectZone = true;
        _fSelected = false;
    }

    public void UpdateValues(Level lev)
    {
        switch (lev)
        {
            case Level.Level1:

                break;
            case Level.Level2:
                foreach (GameObject g in level2)
                    g.SetActive(true);
                break;
            case Level.Level3:

                break;
            case Level.Level4:

                break;
            case Level.Level5:

                break;
        }
    }

    public void DrawWall(Vector3 start, Vector3 end, float maxStep)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        int colX = (int)Math.Floor(Mathf.Abs(start.x - end.x) / maxStep);
        int colZ = (int)Math.Floor(Mathf.Abs(start.z - end.z) / maxStep);

        RaycastHit hit;
        Ray ray;


        for (int i = 0; i < colX; i++)
        {
            ray = new Ray(new Vector3(start.x + (end.x - start.x) / colX * i,100,start.z), Vector3.down);
            Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
            Debug.DrawLine(hit.point, hit.point + Vector3.up);
            ray = new Ray(new Vector3(start.x + (end.x - start.x) / colX * i, 100, end.z), Vector3.down);
            Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
            Debug.DrawLine(hit.point, hit.point + Vector3.up);
        }

        for (int i = 0; i < colZ; i++)
        {
            ray = new Ray(new Vector3(start.x, 100, start.z + (end.z - start.z) / colZ * i), Vector3.down);
            Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
            Debug.DrawLine(hit.point, hit.point + Vector3.up);
            ray = new Ray(new Vector3(end.x, 100, start.z + (end.z - start.z) / colZ * i), Vector3.down);
            Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
            Debug.DrawLine(hit.point, hit.point + Vector3.up);
        }
    }
}
