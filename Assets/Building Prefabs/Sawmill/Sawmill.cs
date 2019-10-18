using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameStructure;

public class Sawmill : MonoBehaviour
{
    public float store;
    public float capacity;
    public ObjectInformation information;
    public int perfomance;
    public Level level;
    public Work work;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PeasanController>())
        {
            work.SetWorkerActiv(other.gameObject.GetComponent<PeasanController>(), true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PeasanController>())
        {
            work.SetWorkerActiv(other.gameObject.GetComponent<PeasanController>(), false);
        }
    }

    public void Start()
    {
        work = GetComponent<Work>();
        AddListeners();
    }

    private void AddListeners()
    {
        work.WorkGoing += WorkGoing;
    }

    public void WorkGoing(int Amount)
    {
        if (work.storage.IsItemExists(Item.wood))
        {
            work.storage.items[Item.wood] += Amount;
        }
        else
        {
            work.storage.items.Add(Item.wood, Amount);
        }
    }

    public void Update()
    {

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
            default:

                break;
        }
    }


}
