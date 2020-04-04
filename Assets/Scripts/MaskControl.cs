using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaskControl : MonoBehaviour
{

    public string respawnTag = "Respawn";
    public List<Collider> colliders;

    void Start()
    {
        DragAndDrop.isOn = false;
    }

    void Update()
    {
        DragAndDrop.isOn = true;
        foreach (Collider cl in colliders)
        {
            string ttag;
            if (cl.tag == "Ivisible")
                ttag = cl.transform.parent.gameObject.tag;
            else
                ttag = cl.tag;

            if (ttag != respawnTag)
                DragAndDrop.isOn = false;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        colliders.Add(coll);
    }

    void OnTriggerStay(Collider coll)
    {
        if (!colliders.Find(cl => cl.name == coll.name))
        {
            colliders.Add(coll);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        colliders.Remove(coll);
    }
}

