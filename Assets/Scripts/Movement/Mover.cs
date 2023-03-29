using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent agent;
        ActionScheduler scheduler;
        //Variables related to animating the player movement
        Animator animator;
        float speed;
        Vector3 localVelocity;
        Vector3 velocity;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            scheduler.StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            agent.destination = destination;
            agent.isStopped = false;
        }

        public void SetSpeed(float speed)
        {
            agent.speed = speed;
        }

        public void Cancel()
        {
            agent.isStopped = true;
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
}
