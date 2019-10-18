using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStructure;
using UnityEngine.Events;

public class PeasanHouse : MonoBehaviour
{
    public GameObject[] houseObects;
    public ObjectInformation information;
    public Transform Entery;

    float BuildigTime = 10;
    int Capacity;
    public Level level;



    void Start()
    {
        information = GetComponent<ObjectInformation>();
        level = Level.Level1;
        UpdateValues(level);
        information.Functions.Add(LevelUp);
        information.Functions.Add(DeleteThisSheet);

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
        Destroy(gameObject,1);
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
        information.LinkDown = houseObects[(int)level].GetComponent<ObjectInformation>();
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
