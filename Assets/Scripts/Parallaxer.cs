using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{

    class PoolObject//this needs to keep track of all of the objects of a certain type (stars,clouds,pipes); responsible for if the object is going to be in use
                    //because we aren't intantiating or destorying during gameplay (except at start) we have to figure out if its in use
    {

        public Transform transform;
        public bool inUse; //if its available or not

        //constructer (t to initialize)
        public PoolObject(Transform t)
        {
            transform = t;
        }

        // if its in use
        public void Use()
        {
            inUse = true;
        }

        //if you dipose, it won't be used
        public void Dispose()
        {
            inUse = false;
        }

    }

    [System.Serializable]
    public struct YSpawnRange
    {
        public float minY;
        public float maxY;
    }

    public GameObject Prefab; //paramters
    public int poolSize; //how many objects should be spawn
    public float shiftSpeed; //how fast the paralaxed obj moving
    public float spawnRate; //how often are they spawning

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; //if we want to spawn immediately, (boolean prewarm in shuriken particle system in unity, so when you press start the particles are already taking shape they need), everything but pipes
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio; //out pipes and paralaxed objs arne't being spawned within the screenspace (publishing means you are considering all aspect ratios)

    float spawnTimer;
    PoolObject[] poolObjects; //array
    float targetAspect; // width divided by height (handling aspect ratio), camera class doesn't give us x or y, just x/y, so store target ratio in a float
    GameManager game;

    void Awake() //initializing, subbing to gameoverconfirmed to respawn the objects
    {
        Configure();
    }

    void Start()
    {
        game = GameManager.Instance; //this needs to be start, start is always after awake, and awake defines gameManager
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed() //dispose all objects (move offscreen), bird will go to start position so all gameobjects should be restarted
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        Configure();
    }

    void Update()
    {
        if (game.GameOver) return; //don't need if gameover

        Shift(); //shifting paralaxed objects
        spawnTimer += Time.deltaTime; //Increase Spawn Timer
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure() //called in awake
    {
        //spawning pool objects
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject; // go short for gameobject,creating object spawning into game, only happen once (spawning prefab and casting it as gameobject)
            Transform t = go.transform; //need to store transforms
            t.SetParent(transform); //paralaxer on all parent objects, so t will set the parent (whatever script is attached to
            t.position = Vector3.one * 1000; //initialize it off screen
            poolObjects[i] = new PoolObject(t); //value passed a parameter within the instructor poolObject we need
        }

        if (spawnImmediate) // if its true
        {
            SpawnImmediate();
        }
    }

    void Spawn() //placing object in right place on screen
    {
        //moving pool objects into place
        Transform t = GetPoolObject(); //get first available pool object
        if (t == null) return; //if true, this indicates that poolSize is too small
        Vector3 pos = Vector3.zero; //initialze
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        t.position = pos; //since pos is independent variable, set transform = pos, t is coming from poolobjects, so set the position is considered spawned
    }

    void SpawnImmediate() //prewarming position where something is spawned initially
    {
        Transform t = GetPoolObject();
        if (t == null) return; //if true, this indicates that poolSize is too small
        Vector3 pos = Vector3.zero; //initialze
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        t.position = pos; //since pos is independent variable, set transform = pos, t is coming from poolobjects, so set the position is considered spawned
        Spawn();
    }

    void Shift()
    {
        //loop through pool objects 
        //moving them
        //discarding them as they go off screen
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime; //frame independent, modigy local when working within a parent gameObject
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject) //check if paralaxed obj off screen, if so dispose so it is availble to spawn again
    {
        //place objects off screen
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000; //making it way offscreen
        }
    }

    Transform GetPoolObject()
    {
        //retrieving first available pool object
        for (int i = 0; i < poolObjects.Length; i++) //loop to see available poolObjects
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use(); //mark as being used now
                return poolObjects[i].transform;
            }
        }
        return null;
    }

}