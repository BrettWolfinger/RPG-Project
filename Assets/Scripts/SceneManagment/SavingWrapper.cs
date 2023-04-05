/* Wrapper for saving system that applies it more specifically to our game
*/
using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour 
    {
        const string defaultSaveFile = "save.json";
        JsonSavingSystem savingSystem;

        [SerializeField] float fadeInTime = 0.2f;

        
        private void Awake() {
            savingSystem = GetComponent<JsonSavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene() {
            yield return savingSystem.LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();

            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
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

        public void Delete()
        {
            savingSystem.Delete(defaultSaveFile);
        }
    }
}