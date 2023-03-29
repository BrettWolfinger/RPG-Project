/* Wrapper for saving system that applies it more specifically to our game
*/
using UnityEngine;

namespace RPG.Saving
{
    public class SavingWrapper : MonoBehaviour 
    {
        const string defaultSaveFile = "save.sav";
        SavingSystem savingSystem;

        private void Awake() {
            savingSystem = GetComponent<SavingSystem>();
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.S))
            {
                savingSystem.Save(defaultSaveFile);
            }
            if(Input.GetKey(KeyCode.L))
            {
                savingSystem.Load(defaultSaveFile);
            }
        }
    }
}