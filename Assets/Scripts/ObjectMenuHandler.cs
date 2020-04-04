using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GameStructure;
using UnityEngine.Serialization;

public class ObjectMenuHandler : MonoBehaviour
{

    [FormerlySerializedAs("Btns")] public Button[] btns;
    public Button[] workerBtns;
    public GameObject workersMenu;
    [FormerlySerializedAs("ItemlListMenu")] public GameObject itemlListMenu;
    [FormerlySerializedAs("ObjectName")] public Text objectName;
    [FormerlySerializedAs("ObjectDescription")] public Text objectDescription;

    private Transform[] _children;

    public void OnObjectSelected()
    {
        foreach (Transform child in _children)
        {
            child.gameObject.SetActive(true);
        }

        gameObject.GetComponent<Image>().enabled = true;

        foreach (Button btn in btns)
        {
            btn.onClick.RemoveAllListeners();
        }

        foreach (Button b in workerBtns)
        {
            b.onClick.RemoveAllListeners();
        }





        IHaveStorage publicStorage;

        if (SelectedObject.Get().TryGetComponent<IHaveStorage>(out publicStorage))
        {
            itemlListMenu.SetActive(true);

            Content[] contentCells = itemlListMenu.GetComponentsInChildren<Content>();
            int counter = 0;
            foreach (KeyValuePair<Item, int> j in publicStorage.PublStorage.items)
            {
                contentCells[counter].countI.text = j.Value.ToString();
                contentCells[counter].nameI.text = j.Key.ToString();
                counter++;
            }
            for (int j = counter; j < contentCells.Length; j++)
            {
                contentCells[j].countI.text = "";
                contentCells[j].nameI.text = "";
            }
        }
        else
        {
            itemlListMenu.SetActive(false);
            foreach (Transform child in itemlListMenu.GetComponentsInChildren<Transform>())
            {
                child.gameObject.SetActive(false);
            }
        }



        INeedWorker needWorker;

        if (SelectedObject.Get().TryGetComponent<INeedWorker>(out needWorker))
        {
            workersMenu.SetActive(true);
            for (int i = 0; i < workerBtns.Length; i++)
            {
                workerBtns[i].gameObject.SetActive(true);
                workerBtns[i].image.color = Color.white;
                if (i == 0)
                {
                    workerBtns[i].onClick.AddListener(needWorker.GetWorker);
                    workerBtns[i].onClick.AddListener(OnObjectSelected);
                    workerBtns[i].image.color = Color.blue;
                }
                else if (i < needWorker.Workers.Count + 1)
                {
                    workerBtns[i].onClick.AddListener(needWorker.RemoveWorker);
                    workerBtns[i].onClick.AddListener(OnObjectSelected);
                }
                else if (i > needWorker.Workers.Count)
                {
                    workerBtns[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            workersMenu.SetActive(false);
            foreach (Transform child in workersMenu.GetComponentsInChildren<Transform>())
            {
                child.gameObject.SetActive(false);
            }
        }



        IHaveFunctions haveFunctions;

        if (SelectedObject.Get().TryGetComponent<IHaveFunctions>(out haveFunctions))
        {

            for (int i = 0; i < btns.Length; i++)
            {
                if (i < haveFunctions.PublFunctions.Count)
                {
                    btns[i].onClick.AddListener(haveFunctions.PublFunctions[i].Value);
                    btns[i].gameObject.GetComponentInChildren<Text>().text = haveFunctions.PublFunctions[i].Key;
                    btns[i].onClick.AddListener(OnInformationChanged);
                }
                else btns[i].gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Button btn in btns)
            {
                btn.gameObject.SetActive(false);
            }
        }



        IHaveName haveName;

        if (SelectedObject.Get().TryGetComponent<IHaveName>(out haveName))
            objectName.text = haveName.Name;
        else
            objectName.text = "I dont know what a fuck is it";

        

        IHaveDescription haveDescription;

        if (SelectedObject.Get().TryGetComponent<IHaveDescription>(out haveDescription))
            objectDescription.text = haveDescription.Description;
        else
            objectDescription.text = "I dont how to describe it";
    }

    public void OnInformationChanged()
    {
        if (SelectedObject.IsActiv())
        {
            IHaveStorage publicStorage;

            

            if (SelectedObject.Get().TryGetComponent<IHaveStorage>(out publicStorage))
            {
                itemlListMenu.SetActive(true);

                Content[] contentCells = itemlListMenu.GetComponentsInChildren<Content>();
                int counter = 0;
                foreach (KeyValuePair<Item, int> j in publicStorage.PublStorage.items)
                {
                    contentCells[counter].countI.text = j.Value.ToString();
                    contentCells[counter].nameI.text = j.Key.ToString();
                    counter++;
                }
                for (int j = counter; j < contentCells.Length; j++)
                {
                    contentCells[j].countI.text = "";
                    contentCells[j].nameI.text = "";
                }
            }
        }
    }

    public void OnObjectDeselected()
    {
        Off();
    }

    private void Off()
    {
        gameObject.GetComponent<Image>().enabled = false;
        foreach (Transform child in _children)
        {
            child.gameObject.SetActive(false);
        }
    }


    void Start()
    {
        GameEvent.LateTik += OnInformationChanged;

        SelectedObject.Selected += OnObjectSelected;
        SelectedObject.Deselsected += OnObjectDeselected;

        _children = gameObject.GetComponentsInChildren<Transform>();
        SelectedObject.Deselect();
    }

}

public interface IHaveStorage
{
    ItemStorage PublStorage { get; }
}

public interface IHaveFunctions
{
    List<KeyValuePair<string,UnityAction>> PublFunctions { get; }
}

public interface IHaveName
{
    string Name { get; }
}

public interface IHaveDescription
{
    string Description { get; }
}



public interface INeedWorker
{
    void GetWorker();
    void RemoveWorker();
    List<PeasanController> Workers { get; }
}


public static class SelectedObject
{
    public delegate void ObjectStateHandler();

    public static event ObjectStateHandler Selected;
    public static event ObjectStateHandler Deselsected;


    private static GameObject _object;

    public static GameObject Get()
    {
        return _object;
    }

    public static void Set(GameObject obj)
    {
        _object = obj;
        Selected?.Invoke();
    }

    public static bool IsActiv()
    {
        return _object;
    }

    public static void Deselect()
    {
        _object = null;
        Deselsected?.Invoke();
    }
}
