using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "RPG/Ability/Targeting/DelayedClick", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D cursorTexture;
        [SerializeField] Vector2 cursorHotspot;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float areaAffectRadius;
        [SerializeField] Transform targetGraphicPrefab;

        Transform targetingPrefabInstance = null;

        public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> finished)
        {
            PlayerController playerController = user.GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(user, playerController, finished));
        }

        private IEnumerator Targeting(GameObject user, PlayerController playerController, Action<IEnumerable<GameObject>> finished)
        {
            while (true)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                playerController.enabled = false;
                if(targetingPrefabInstance == null)
                {
                    targetingPrefabInstance = Instantiate(targetGraphicPrefab);
                }
                else
                {
                    targetingPrefabInstance.gameObject.SetActive(true);
                }
                targetingPrefabInstance.localScale = new Vector3(areaAffectRadius*2, 1, areaAffectRadius*2);
                RaycastHit raycastHit;
                if(Physics.Raycast(PlayerController.GetMouseRay(), out raycastHit, 1000, layerMask))
                {
                    targetingPrefabInstance.position = raycastHit.point;
                    if(Input.GetMouseButtonDown(0))
                    {
                        //absorb the whole mouse click to prevent trailing movement
                        yield return new WaitWhile(() => Input.GetMouseButton(0));
                        playerController.enabled = true;
                        targetingPrefabInstance.gameObject.SetActive(false);
                        finished(GetGameObjectsInRadius(raycastHit.point));
                        yield break;
                    }
                    //run every frame (like update without MonoBehaviour)
                    yield return null;
                }
            }
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, areaAffectRadius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}