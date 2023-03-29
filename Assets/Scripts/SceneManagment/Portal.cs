using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] Portal_SO portalData;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeWaitTime = 1f;

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if(portalData.SceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            
            DontDestroyOnLoad(this.gameObject);
            yield return fader.FadeOut(fadeOutTime);
            
            yield return SceneManager.LoadSceneAsync(portalData.SceneToLoad);

            Portal otherPortal = GetOtherPortal();
            savingWrapper.Load();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(this.gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;

                if(portal.portalData != portalData.Destination) continue;

                return portal;
            }
            return null;
        }
    }
}