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
            //Load();
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKey(KeyCode.L))
            {
                Load();
            }
        }

        public void Save()
        {
            savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            savingSystem.Load(defaultSaveFile);
        }
    }
}