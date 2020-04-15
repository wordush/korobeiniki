using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GameStructure;
using UnityEngine.Serialization;

public class Work : MonoBehaviour , IHaveStorage , INeedWorker
{
    public IWorkStorage curWork;

    public delegate void TaskGoing(PeasanController peasan);
    public TaskGoing done;
    
    [SerializeField]
    public Transform destination;
    public List<PeasanController> workerslocal;
    public List<PeasanController> Workers => workerslocal;
    public List<PeasanController> activeWorkers;
    public int vacancyCount;
    [HideInInspector]
    public delegate void WorkHandler(int amount);
    public event WorkHandler WorkGoing;
    public ItemStorage PublStorage => storage;
    public ItemStorage storage;


    public void Start()
    {
        curWork = gameObject.GetComponent<IWorkStorage>();
        storage = new ItemStorage(destination.gameObject);
        GameEvent.Tik += TicUpdate;
    }

    public void RemoveWorker()
    {
        SceneLogic logic = GameObject.FindGameObjectWithTag("Buildings").GetComponent<SceneLogic>();
        if (workerslocal.Count > 0)
        {
            PeasanController peasan = workerslocal[0];
            workerslocal.Remove(peasan);
            if (activeWorkers.Contains(peasan))
                activeWorkers.Remove(peasan);
            logic.peasanList.Add(peasan);
            peasan.state = State.Rest;
            peasan.SetRest();
            Debug.Log("Worker removed");
        }
    }

    public void GetWorker()
    {
        SceneLogic logic = GameObject.FindGameObjectWithTag("Buildings").GetComponent<SceneLogic>();
        if (logic.peasanList.Count > 0)
        {
            if (workerslocal.Count < vacancyCount)
            {
                PeasanController worker = logic.peasanList.Last();
                workerslocal.Add(worker);
                logic.peasanList.Remove(worker);
                worker.work = this;
                Debug.Log("Worker getted");
            } else
            {
                Debug.LogWarning("No free vacancies");
            }
        }
        else
        {
            Debug.LogWarning("No freee workerslocal");
        }
    }

    public void SetVorkerGo(GameObject goal, PeasanController peasan)
    {
        peasan.taskDone = null;
        peasan.taskDone += WorkerDone;
    }

    public void SetVorkerRest(PeasanController peasan)
    {
        peasan.SetRest();
    }

    void WorkerDone(PeasanController peasan)
    {
        done?.Invoke(peasan);
        peasan.taskDone -= WorkerDone;
    }


    public void SetWorkerActiv(PeasanController peasan ,bool state)
    {
        if (workerslocal.Contains(peasan) && state)
        {
            activeWorkers.Add(peasan);
            peasan.state = State.Work;
        }
        if (workerslocal.Contains(peasan) && !state)
        {
            activeWorkers.Remove(peasan);
            peasan.state = State.Rest;
        }
        
    }

    void TicUpdate()
    {

    }
}

public class PeasanActivity
{
    public GameObject destenation;
    public int priority;
    public ActivityType type;
    public List<string> corutineNames;
    public PeasanController peasan;

    public PeasanActivity(GameObject destenation, ActivityType type,  List<string> corutineNames, int priority = 0)
    {
        this.destenation = destenation;
        this.type = type;
        this.priority = priority;
        this.corutineNames = corutineNames;
        peasan = null;
    }
}

public enum ActivityType
{
    TreeCut,
}

public interface IWorkStorage
{
    bool FullOrNot();
}

public interface IWorkActivityList
{
    List<PeasanActivity> AcivityList { get; }
}