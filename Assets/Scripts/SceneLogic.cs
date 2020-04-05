using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameStructure;
using UnityEngine.Serialization;

public class SceneLogic : MonoBehaviour
{

    [FormerlySerializedAs("Houses")] public List<PeasanHouse> houses;
    [FormerlySerializedAs("RestList")] public List<Rest> restList;
    [FormerlySerializedAs("PeasanList")] public List<PeasanController> peasanList;
    [FormerlySerializedAs("StorageList")] public List<Storage> storageList;

    public Rest NearestRest(Vector3 point)
    {
        return restList.OrderBy(o => Vector3.Distance(point, o.destination.position)).ToList().First();
    }


    void OnHouseBuilded(GameObject house)
    {
        if (house.GetComponent<Rest>())
            restList.Add(house.GetComponent<Rest>());
            
        Debug.Log(house.name);
    }

    void OnPeasanSpawned(PeasanController peasan)
    {
        peasanList.Add(peasan);

        Debug.Log(peasan.name);
    }




    void Start()
    {
        houses = GetComponentsInChildren<PeasanHouse>().ToList();
        storageList = GetComponentsInChildren<Storage>().ToList();
        peasanList = GetComponentsInChildren<PeasanController>().ToList();
         
        GameEvent.Built += OnHouseBuilded;
        GameEvent.PeasanSpawned += OnPeasanSpawned;

        restList = GameObject.FindGameObjectWithTag("Buildings").GetComponentsInChildren<Rest>().ToList();
    }

    public float timer;

    void Update()
    {
        timer += Time.smoothDeltaTime;
        if (timer >= 1)
        {
            GameEvent.MakeTik();
            timer -= 1;
        }
    }
}

public static class GameEvent
{
    public delegate void GameTik();
    public static event GameTik Tik;
    public static event GameTik LateTik;

    public static void MakeTik()
    {
        Tik?.Invoke();
        LateTik?.Invoke();
    }


    public delegate void HouseBuilding(GameObject house);
    public static event HouseBuilding Built;

    public static void BuildingBuilded(GameObject house)
    {
        Built?.Invoke(house);
    }

    public delegate void PeasanHandler(PeasanController peasan);
    public static event PeasanHandler PeasanSpawned;
    public static void SpawnePeasan(PeasanController peasan)
    {
        PeasanSpawned?.Invoke(peasan);
    }

    public delegate void StorageHandler();
    public static event StorageHandler StorageBuilded;

    public delegate void ItemHandler(Item type, int count, ItemStorage point);
    public static event ItemHandler NeedItems;
    public static event ItemHandler FullItems;

    public static void FullOfItems(Item type, int count, ItemStorage point)
    {
        FullItems?.Invoke(type, count, point);
    }

    public static void NeedOfItems(Item type, int count, ItemStorage point)
    {
        NeedItems?.Invoke(type, count, point);
    }

}

