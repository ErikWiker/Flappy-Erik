using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] //This makes unity only use this script only on this type
public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos; //Where bird starts

    public AudioSource tapSound;
    public AudioSource scoreSound;
    public AudioSource dieSound;

    //private refs
    Rigidbody2D rigidBody;
    Quaternion downRotation; //fancy form of rotation, four values xyzw, values 0 to 1.... good for rotation
    Quaternion forwardRotation;

    GameManager game;
    TrailRenderer trail;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>(); //gets component off the object
        downRotation = Quaternion.Euler(0, 0, -100); //putting it face down (converting vector 3 into quaternion)
        forwardRotation = Quaternion.Euler(0, 0, 40); // having it face up a bit
        game = GameManager.Instance;
        rigidBody.simulated = false;
        //trail = GetComponent<TrailRenderer>();
        //trail.sortingOrder = 20; 
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidBody.velocity = Vector3.zero; // usually recommended not to modify velocity
        rigidBody.simulated = true; //Starts listening to physics, cuz on trigger enter sets it to false
    }

    void OnGameOverConfirmed() //we haven't pressed play yet, so set back to start postion
    {
        transform.localPosition = startPos; //anytime you are inside a parent object, want to set local position
        transform.rotation = Quaternion.identity;
    }

    //Called once per frame
    void Update()
    {
        if (game.GameOver) return;

        if (Input.GetMouseButtonDown(0)) //0 is left click, 1 is right click (translates to tap on mobile devices)
        {

            rigidBody.velocity = Vector2.zero;
            transform.rotation = forwardRotation;
            rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force); //also has impulse
            tapSound.Play();
        }

        //Trans.rot is quaternion no euler
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime); // lerp going from source to target value over certain amount of time
                                                                                                             // target is downRot, tiltSmooth is a paramater defined up top
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ScoreZone")
        {
            //register a score event
            OnPlayerScored(); //event sent to GameManager;

            scoreSound.Play();
        }

        if (col.gameObject.tag == "DeadZone")
        {
            rigidBody.simulated = false; //freezes player where they hit
            //register a dead event
            OnPlayerDied(); //event sent to GameManager;
            dieSound.Play();
        }
    }

}