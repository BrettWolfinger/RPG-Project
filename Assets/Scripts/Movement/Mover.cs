using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
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

        public JToken CaptureAsJToken()
        {
            return JsonStatics.ToToken(transform.position);
        }

        public void RestoreFromJToken(JToken state)
        {
            Vector3 position = JsonStatics.ToVector3(state);
            transform.GetComponent<NavMeshAgent>().Warp(position);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
