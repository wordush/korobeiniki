using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour
{
    public static bool ClickHandlerActiv; // Careful with that

    public void Start()
    {
        ClickHandlerActiv = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && ClickHandlerActiv)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log(hit.collider.name);
                    GameObject targ;
                    targ = hit.collider.CompareTag("Ivisible") ? hit.collider.gameObject.transform.parent.gameObject : hit.collider.gameObject;
                    Debug.Log($"Selected obj name: {targ.name}");
                    if (
                        targ.TryGetComponent<IHaveDescription>(out IHaveDescription haveD) ||
                        targ.TryGetComponent<IHaveFunctions>(out IHaveFunctions haveF) ||
                        targ.TryGetComponent<IHaveName>(out IHaveName haveN) ||
                        targ.TryGetComponent<IHaveStorage>(out IHaveStorage haveS)
                        )
                    {
                        SelectedObject.Set(targ);
                    }
                    else SelectedObject.Deselect();
                }
                else SelectedObject.Deselect();
            }
        }
    }

    public static GameObject[] GetObjectsInZone(Vector3 start, Vector3 end)
    {
        Collider[] colls = Physics.OverlapBox(new Vector3((start.x + end.x) / 2, 1, (start.z + end.z) / 2), new Vector3(Mathf.Abs((start.x - end.x)/2), 10, Mathf.Abs((start.z - end.z)/2)));

        List<GameObject> result = new List<GameObject>();

        foreach (Collider col in colls)
        {
            if (col.gameObject.tag == "Ivisible")
            {
                if (!result.Contains(col.gameObject.transform.parent.gameObject))
                    result.Add(col.gameObject.transform.parent.gameObject);
            }
            else
                if (!result.Contains(col.gameObject))
                    result.Add(col.gameObject);
        }
        return result.ToArray();
    }
}