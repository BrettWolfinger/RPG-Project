using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour {

        [SerializeField] Weapon_SO weapon = null;
        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}
