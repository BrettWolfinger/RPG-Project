using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        private void Awake() {
            player = GameObject.FindWithTag("Player");
            //cinematic starts
            GetComponent<PlayableDirector>().played += DisableControl;
            //cinematic stops
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }
        void DisableControl(PlayableDirector aDirector)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector aDirector)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
