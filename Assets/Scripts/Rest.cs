using System.Collections.Generic;
using UnityEngine;
using GameStructure;
using UnityEngine.Serialization;

public class Rest : MonoBehaviour, IHaveName
{

    public string Name { get { return name; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PeasanController>())
        {
            PeasanController peasan = other.gameObject.GetComponent<PeasanController>();
            resters.Add(peasan);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PeasanController>())
        {
            PeasanController peasan = other.gameObject.GetComponent<PeasanController>();
            resters.Remove(peasan);
        }
    }


    private void Start()
    {
        GameEvent.Tik += TikUpdate;
    }

    public void TikUpdate()
    {
        RestGoing?.Invoke(1);
    }
    

    [FormerlySerializedAs("Destination")] public Transform destination;
    public List<PeasanController> resters;
    public delegate void RestHandler(int count);
    public event RestHandler RestGoing;
}
