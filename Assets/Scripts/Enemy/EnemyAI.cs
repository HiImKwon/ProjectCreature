﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private float MovementSpeed; //between 0 and 1
    private int spotRange;
    private GameObject Player;
    private float dist;
    private bool _CanMove = false;
    public GameObject lookAt;
    private SkinnedMeshRenderer skin;
    private bool _Wandering = false;
    private bool _IsRotating = false;
    private bool _IsWalking = false;
    private RaycastHit hit;
    public float rotSpeed = 100f;
    private float health;
    private Enemy _En;
    private CollisionAvoidance avoidScript;

    NavMeshAgent agent;
    public LayerMask mask;

    private Rigidbody _Rb;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        spotRange = 50;
        _Rb = GetComponent<Rigidbody>();
        _Rb.angularDrag = 0;
        _En = GetComponent<Enemy>();
        health = _En.Health;
        MovementSpeed = _En.MovementSpeed1;
        //agent = GetComponent<NavMeshAgent>();
        avoidScript = GetComponent<CollisionAvoidance>();
    }

    // using FixedUpdate instead of update so PCs with slow framerates don't skip important calculations
    void FixedUpdate()
    {
        if (DetectsEnemy())
        {
            //if player is spotted by the enemy, chase the player and stop wandering
            ChasePlayer();
            StopCoroutine(Wander());
        }
        else { 
            if (!_Wandering)
            {
                StartCoroutine(Wander());
            }
            if (_IsRotating)
            {
                transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
            }
            if (_IsWalking)
            {
                //if self doesn't need course correction (isn't about to hit a wall), move forward normally
                if (!avoidScript.CourseCorrection())
                {
                    transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                }
            }
        }
        
    }

    IEnumerator Wander()
    {

        //enemy will wait for walkWait seconds, will then walk for walkTime seconds
        //with then pause for rotateWait seconds, then will rotate for rotTime
        //then will start the process again
        //each of these times are set randomly, with their range indicated below
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(1, 4);
        int rotate = Random.Range(-2, 2);
        int walkWait = Random.Range(1, 4);
        int walkTime = Random.Range(1, 5);

        _Wandering = true;

        yield return new WaitForSeconds(walkWait);
        _IsWalking = true;
        yield return new WaitForSeconds(walkTime);
        _IsWalking = false;
        yield return new WaitForSeconds(rotateWait);
        _IsRotating = true;
        yield return new WaitForSeconds(rotTime);
        _IsRotating = false;
        _Wandering = false;
    }

    private bool DetectsEnemy()
    {
        dist = Vector3.Distance(this.transform.position, Player.transform.position);
        //Renderer rend = Player.GetComponentInChildren<Renderer>();
        if (dist < spotRange/* && rend.enabled == true*/)
        {
            //face the player
            this.transform.LookAt(Player.transform.position + (Vector3.up*2));
            Ray objectRay = new Ray(transform.position + Vector3.up*4, Player.transform.position - (transform.position + Vector3.up * 4));
            //Debug.DrawRay(transform.position + Vector3.up * 4, Player.transform.position - (transform.position + Vector3.up * 4), Color.red);
            if (Physics.Raycast(objectRay, out hit, spotRange))
            {
                //if sees player, move towards it, otherwise do nothing
                if (hit.collider.tag == "Player")
                {
                    return true;
                    
                }
                return false;
            }
            else
            {
                print("ray not hitting anything at all?");
                
            }
        }
        return false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _CanMove = true;
        }
        //if comes into contact with projectile
        if(collision.gameObject.tag == "Attack")
        {
            _Rb.AddForce(-collision.relativeVelocity.normalized*50);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _CanMove = false;
        }
    }
    private void ChasePlayer()
    {
        transform.LookAt(hit.collider.gameObject.transform.position + (Vector3.up * 2)); //look at player
        Vector3 movement = Vector3.forward * MovementSpeed * Time.deltaTime; //move forward
        _Rb.transform.Translate(movement); //move towards the player
    }

    
}
