using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using GameStructure;
using UnityEngine.Serialization;

public class PeasanController : MonoBehaviour , IHaveStorage
{
    public State state;
    public GameStructure.Work work;
    public int age;
    public float walkSpeed;
    public int healthPoints;
    public int energy;
    public int energyMax;

    public float waiting;
    public bool wait;

    public PeasanHouse house;
    public Work workObj;

    public ItemStorage items;

    public ItemStorage PublStorage { get { return items; } }

    public delegate void TaskHandler(PeasanController peasan);
    public TaskHandler taskDone;
    public GameObject Temprary;
    public bool activated;
    public GameObject destination;

    public SceneLogic logic;
    public NavMeshAgent agent;

    private bool _follow;
    private GameObject _followTarg;

    [FormerlySerializedAs("AgentVelocity")] public Vector3 agentVelocity;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 1000;
        logic = GameObject.FindGameObjectWithTag("Buildings").GetComponent<SceneLogic>();
        GameEvent.SpawnePeasan(this);
        agent.stoppingDistance = 0.5f;
        items = new ItemStorage(gameObject);

        agent.updateRotation = false;
        Physics.IgnoreLayerCollision(9,9);
    }

    public void SetRest()
    {
        Vector3 position = gameObject.transform.position;
        agent.SetDestination(logic.NearestRest(position).destination.position);
        destination = logic.NearestRest(position).destination.gameObject;
        activated = true;
    }


    public void Update()
    {
        agentVelocity = Vector3.Lerp(agentVelocity,agent.velocity,Time.deltaTime * 10);
        if (agentVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(agentVelocity.normalized.x,0, agentVelocity.normalized.z));



        float dist = Vector3.Distance(gameObject.transform.position, destination.transform.position);

        if (dist <= 0.6f && activated)
        {
            activated = false;
            taskDone?.Invoke(this);
        }

        if (wait)
        {
            if (waiting > 0)
            {
                agent.enabled = false;
                waiting -= Time.deltaTime;
            }
            else
            {
                agent.enabled = true;
                agent.SetDestination(destination.transform.position);
                wait = false;
            }
        }

        if (_follow)
        {
            agent.destination = _followTarg.transform.position;
        }
        
        
    }

    public void SetDestination(GameObject point)
    {
        destination = point;
        agent.SetDestination(destination.transform.position);
        activated = true;
    }

    public void SetFollow(GameObject target)
    {
        _follow = true;
        _followTarg = target;
    }

    public void UnFollow()
    {
        _follow = false;
    }
}


