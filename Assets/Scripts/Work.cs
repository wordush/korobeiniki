using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GameStructure;
using UnityEngine.Serialization;

public class Work : MonoBehaviour , IHaveStorage , INeedWorker
{
    [SerializeField] private GameStructure.Work type;

    public IWorkStorage curWork;

    public delegate void TaskGoing(PeasanController peasan);
    public TaskGoing done;
    

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
            peasan.work = GameStructure.Work.Rest;
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
                // SetVorkerGo(destination.position, worker);
                worker.SetDestination(destination.gameObject);
                logic.peasanList.Remove(worker);
                worker.workObj = this;
                worker.work = type;
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
        peasan.SetDestination(goal);
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
        if (activeWorkers.Count >= 1 && !curWork.FullOrNot())
        {
            WorkGoing?.Invoke(activeWorkers.Count);
            GetEnergy?.Invoke(1);
        }
    }

    public void Start()
    {
        curWork = gameObject.GetComponent<IWorkStorage>();
        storage = new ItemStorage(destination.gameObject);
        GameEvent.Tik += TicUpdate;
    }
    
    [SerializeField]
    public Transform destination;
    public List<PeasanController> workerslocal;
    public List<PeasanController> Workers { get { return workerslocal; } }
    public List<PeasanController> activeWorkers;
    public int vacancyCount;
    [HideInInspector]
    public delegate void WorkHandler(int amount);
    public event WorkHandler WorkGoing;
    public event WorkHandler GetEnergy;
    public ItemStorage PublStorage => storage;
    public ItemStorage storage;
}

public interface IWorkStorage
{
    bool FullOrNot();
}