using UnityEngine;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

namespace RPG.Saving
{
    /*Core saving system script with generic saving functionality
    */
    public class JsonSavingSystem : MonoBehaviour 
    {
        private const string lastSceneBuildIndex = "lastSceneBuildIndex";
        
        public IEnumerator LoadLastScene(string saveFile)
        {
            JObject state = LoadJsonFromFile(saveFile);
            IDictionary<string,JToken> stateDict = state;
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if(stateDict.ContainsKey(lastSceneBuildIndex))
            {
                buildIndex = (int)stateDict[lastSceneBuildIndex];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreFromToken(state);
        }
        
        public void Save(string saveFile)
        {
            JObject state = LoadJsonFromFile(saveFile);
            CaptureAsToken(state);
            SaveFileAsJson(saveFile,state);
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }


        public void Load(string saveFile)
        {
            RestoreFromToken(LoadJsonFromFile(saveFile));
        }


        private void SaveFileAsJson(string saveFile, JObject state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("save to " + path);
            using (var textWriter = File.CreateText(path))
            {
                using (var writer = new JsonTextWriter(textWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    state.WriteTo(writer);
                }
            }

        }

        private JObject LoadJsonFromFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("load from "+ path);
            if(!File.Exists(path))
            {
                return new JObject();
            }
            using (var textReader = File.OpenText(path))
            {
                using (var reader = new JsonTextReader(textReader))
                {
                    reader.FloatParseHandling = FloatParseHandling.Double;

                    return JObject.Load(reader);
                }
            }

        }

        private void CaptureAsToken(JObject state)
        {
            foreach(JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureAsJtoken();
            }

            state[lastSceneBuildIndex] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreFromToken(JObject state)
        {
            IDictionary<string, JToken> stateDict = state;
            foreach(JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if(stateDict.ContainsKey(id))
                {
                    saveable.RestoreFromJToken(stateDict[id]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath,saveFile);
        }
    }
}