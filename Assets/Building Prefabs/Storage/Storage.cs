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
        storage = new ItemStorage(destination.gameObject);
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
        
    }

    public void TaskDone(PeasanController peasan)
    {
        
    }


    

    void OnFullItems(Item type, int count, ItemStorage point)
    {
        if (!Full.ContainsKey(type))
        {
            // Add activity
            Full.Add(type, point);
            Debug.Log($"Some need to be emptied");
        }
    }

    void OnNeedItems(Item type, int count, ItemStorage point)
    {
        if (!Need.ContainsKey(type))
        {
            // Add activity
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
        worker.state = State.Rest;
        worker.SetRest();
        logic.peasanList.Add(worker);
    }

}

