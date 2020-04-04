using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameStructure;
using UnityEngine.Serialization;

public class Storage : MonoBehaviour, IHaveStorage, INeedWorker
{
    [FormerlySerializedAs("Destination")] public Transform destination;
    public ItemStorage storage;
    private SceneLogic _logic;

    public ItemStorage PublStorage { get { return storage; } }

    private static bool _isInitialised = true;
    [SerializeField] public static List<PeasanController> DeliveryBoys;
    public List<PeasanController> Workers { get { return DeliveryBoys; } }
    [SerializeField] public static List<PeasanController> ActiveDeliveryBoys;

    public static Dictionary<Item, ItemStorage> Full;
    public static Dictionary<Item, ItemStorage> Need;

    public delegate void StorageWorkHandler(int nardnss);
    public static StorageWorkHandler OnGetEnergy;

    private bool _switch;

    [FormerlySerializedAs("DeliveryInfo")] public Dictionary<PeasanController, KeyValuePair<Item, ItemStorage>> deliveryInfo;

    public void Start()
    {
        _logic = GameObject.FindGameObjectWithTag("Buildings").GetComponent<SceneLogic>();
        if (_isInitialised)
        {
            DeliveryBoys = new List<PeasanController>();
            ActiveDeliveryBoys = new List<PeasanController>();
            Full = new Dictionary<Item, ItemStorage>();
            Need = new Dictionary<Item, ItemStorage>();
            _isInitialised = false;
        }
        deliveryInfo = new Dictionary<PeasanController, KeyValuePair<Item, ItemStorage>>();
        storage = new ItemStorage(destination.position);
        GameEvent.Tik += TikUpdate;
        GameEvent.LateTik += LateTikUpdate;
        GameEvent.FullItems += OnFullItems;
        GameEvent.NeedItems += OnNeedItems;
    }

    private void TikUpdate()
    {

    }

    private void LateTikUpdate()
    {
        OnGetEnergy?.Invoke(1);
        if (DeliveryBoys.Count > 0 && (Need.Count > 0 || Full.Count > 0))
        {
            switch (_switch)
            {
                case true:
                    PeasanController boy = DeliveryBoys.First();
                    KeyValuePair<Item,ItemStorage> goal = Full.First();

                    goal.Value.ConsunationStarted = true;
                    boy.state = State.Consum1St;
                    boy.SetDestination(Full.First().Value.Destination);
                    deliveryInfo.Add(boy, goal);
                    boy.taskDone += TaskDone;

                    Full.Remove(goal.Key);
                    ActiveDeliveryBoys.Add(boy);
                    DeliveryBoys.Remove(boy);
                    _switch = false;
                    break;
                case false:
                    _switch = true;
                    break;
            }
        }
    }

    public void TaskDone(PeasanController peasan)
    {
        switch (peasan.state)
        {
            case State.Consum1St:
                ItemStorage.ItemTransfer(deliveryInfo[peasan].Key, deliveryInfo[peasan].Value, peasan.items);
                peasan.SetDestination(destination.position);
                peasan.state = State.Consum2Nd;
                deliveryInfo[peasan].Value.ConsunationStarted = false;
                Debug.Log("First stage of delivery done");
                break;
            case State.Consum2Nd:
                ItemStorage.ItemTransfer(deliveryInfo[peasan].Key, peasan.items, storage);
                peasan.state = State.Rest;
                ActiveDeliveryBoys.Remove(peasan);
                DeliveryBoys.Add(peasan);
                deliveryInfo.Remove(peasan);
                peasan.taskDone -= TaskDone;
                peasan.SetRest();
                Debug.Log("Second stage of delivery done");
                break;
            case State.Delivery1St:

                break;
            case State.Delivery2Nd:

                break;
        }
    }


    

    void OnFullItems(Item type, int count, ItemStorage point)
    {
        if (!Full.ContainsKey(type))
        {
            Full.Add(type, point);
            Debug.Log($"Some need to be emptied");
        }
    }

    void OnNeedItems(Item type, int count, ItemStorage point)
    {
        if (!Need.ContainsKey(type))
        {
            Need.Add(type, point);
            Debug.Log($"Some need to some {type.ToString()}");
        }

    }

    public void GetWorker()
    {
        GetDeliveryBoy();
    }

    public static void GetDeliveryBoy()
    {
        SceneLogic logic = GameObject.FindGameObjectWithTag(tag: "Buildings").GetComponent<SceneLogic>();
        if (logic.peasanList.Count > 0)
        {
            PeasanController worker = logic.peasanList.Last();
            DeliveryBoys.Add(item: worker);
            worker.work = GameStructure.Work.Work;
            worker.state = State.Rest;
            logic.peasanList.Remove(item: worker);
            Debug.Log(message: "Boy getted");
        } else
        {
            Debug.Log(message: "No freee workers");
        }
    }

    public void RemoveWorker()
    {
        RemoveDeliveryBoy();
    }

    public static void RemoveDeliveryBoy()
    {
        SceneLogic logic = GameObject.FindGameObjectWithTag("Buildings").GetComponent<SceneLogic>();
        PeasanController worker = DeliveryBoys.Last();
        DeliveryBoys.Remove(worker);
        worker.work = GameStructure.Work.Rest;
        worker.state = State.Rest;
        worker.SetRest();
        logic.peasanList.Add(worker);
    }

}

