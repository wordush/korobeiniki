using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStructure;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PeasanHouse : MonoBehaviour , IHaveFunctions , IHaveName , IHaveDescription
{
    public GameObject[] houseObects;
    [FormerlySerializedAs("Entery")] public Transform entery;


    public string showName;
    public string Name { get { return showName; } }
    public string showDescription;
    public string Description { get { return showDescription; } }

    [FormerlySerializedAs("Functions")] public List<KeyValuePair<string, UnityAction>> functions;
       
    public List<KeyValuePair<string, UnityAction>> PublFunctions { get{return functions; } }

    float _buildigTime = 10;
    public int restCount;
    int _capacity;
    public Level level;

    public List<PeasanController> resters;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PeasanController>(out PeasanController peasan))
        {
            if (peasan.state == State.Rest)
                resters.Add(peasan);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PeasanController>(out PeasanController peasan))
        {
            if (resters.Contains(peasan))
                resters.Remove(peasan);
        }
    }

    public void TicUpdate()
    {
        foreach (PeasanController peas in resters)
        {
            peas.energy += restCount;
        }
    } 


    void Start()
    {
        GameEvent.Tik += TicUpdate;
        level = Level.Level1;
        UpdateValues(level);
        functions = new List<KeyValuePair<string, UnityAction>>
        {
            new KeyValuePair<string, UnityAction>("LevelUp", LevelUp),
            new KeyValuePair<string, UnityAction>("Destroy", DeleteThisSheet)
        };

        GameEvent.BuildingBuilded(gameObject);

        SelectedObject.Set(gameObject);
    }

    public GameObject GetActiveHouse()
    {
        return houseObects[(int)level];
    }

    public void DeleteThisSheet()
    {
        SelectedObject.Deselect();
        Destroy(gameObject,0);
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
        foreach (GameObject obj in houseObects)
        {
            obj.SetActive(false);
        }

        switch (lev)
        {
            case Level.Level1:
                houseObects[0].SetActive(true);
                break;
            case Level.Level2:
                houseObects[1].SetActive(true);
                break;
            case Level.Level3:
                houseObects[2].SetActive(true);
                break;
            case Level.Level4:
                houseObects[3].SetActive(true);
                break;
            case Level.Level5:
                houseObects[4].SetActive(true);
                break;
            default:
                houseObects[0].SetActive(true);
                break;
        }
    }
}
