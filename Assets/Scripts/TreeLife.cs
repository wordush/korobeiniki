using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TreeLife : MonoBehaviour, IHaveFunctions
{
    private Material[] _treeMat;

    public Color natCol;
    [FormerlySerializedAs("Label")] public Color label;

    [FormerlySerializedAs("Tree")] public GameObject tree;

    public List<KeyValuePair<string, UnityAction>> funct;
    public List<KeyValuePair<string, UnityAction>> PublFunctions => funct;


    // Start is called before the first frame update
    private void Start()
    {
        _treeMat = tree.GetComponent<Renderer>().materials;
        natCol = _treeMat[0].color;
        funct = new List<KeyValuePair<string, UnityAction>>()
        {
            new KeyValuePair<string, UnityAction>("Make red", MakeRed),
            new KeyValuePair<string, UnityAction>("Make natural", MakeNat)
        };
    }

    // Update is called once per frame

    public void MakeRed()
    {
        foreach (Material mat in _treeMat)
        {
            mat.color = label;
        }

    }

    public void MakeNat()
    {
        foreach (Material mat in _treeMat)
        {
            mat.color = natCol;
        }
    }

    public void mark()
    {        
        foreach (Material mat in _treeMat)
        {
            mat.color = Color.red;
        }
        
    }
    // 39E032 - green
    // FF0300 - red
}
