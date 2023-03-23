using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    NavMeshAgent agent;
    //Variables related to animating the player movement
    Animator animator;
    float speed;
    Vector3 localVelocity;
    Vector3 velocity;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        UpdateAnimator();
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray , out hit);
        if(hasHit){
            agent.destination = hit.point;
        }
    }

    private void UpdateAnimator()
    {
        velocity = agent.velocity;
        //converts from global to local for animator
        localVelocity = transform.InverseTransformDirection(velocity);
        speed = localVelocity.z;
        animator.SetFloat("ForwardSpeed",speed);
    }
}
