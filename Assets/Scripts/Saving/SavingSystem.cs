using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

namespace RPG.Saving
{
    /*Core saving system script with generic saving functionality
    */
    public class SavingSystem : MonoBehaviour 
    {
        private const string lastSceneBuildIndex = "lastSceneBuildIndex";
        
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            if(state.ContainsKey(lastSceneBuildIndex))
            {
                int buildIndex = (int)state[lastSceneBuildIndex];
                if(buildIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    yield return SceneManager.LoadSceneAsync(buildIndex);
                }
            }
            RestoreState(state);
        }
        
        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile,state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("save to " + path);
            using(FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream,state);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state[lastSceneBuildIndex] = SceneManager.GetActiveScene().buildIndex;
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("load from "+ path);
            if(!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            else{
                using(FileStream stream = File.Open(path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (Dictionary<string, object>)formatter.Deserialize(stream);
                }
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            Dictionary<string, object> stateDict = state;
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if(state.ContainsKey(id))
                {
                    saveable.RestoreState(stateDict[id]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath,saveFile);
        }
    }
}