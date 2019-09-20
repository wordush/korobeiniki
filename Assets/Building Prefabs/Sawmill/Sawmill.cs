using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStructure;

public class Sawmill : MonoBehaviour
{
    public float store;
    public float capacity;
    public ObjectInformation information;
    public int perfomance;
    public Level level;

    public void Start()
    {
        information = GetComponent<ObjectInformation>();
    }

    public void Update()
    {
        store += Mathf.Clamp(perfomance * Time.deltaTime,0,capacity);
        information.Description = ((int)store).ToString() + ": wood in the bank";
        if(SelectedObject.IsActiv() && SelectedObject.Get().Equals(gameObject))
            GameObject.Find("ObjectMenu").GetComponent<ObjectMenuHandler>().OnInformationChanged();
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
