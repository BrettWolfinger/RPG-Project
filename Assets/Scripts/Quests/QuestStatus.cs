using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public class QuestStatus
    {
        [SerializeField] Quest quest;
        [SerializeField] List<string> completedObjectives = new List<string>();

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public void CompleteObjective(string objectiveToComplete)
        {
            if(quest.HasObjective(objectiveToComplete))
            {
                completedObjectives.Add(objectiveToComplete);
            }
        }

        public int GetCompletedObjectivesCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        
        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }

        //Saving/Loading Methods
        public QuestStatus(JToken objectState)
        {
            if (objectState is JObject state)
            {
                IDictionary<string, JToken> stateDict = state;
                quest = Quest.GetByName(stateDict["questName"].ToObject<string>());
                completedObjectives.Clear();
                if (stateDict["completedObjectives"] is JArray completedState)
                {
                    IList<JToken> completedStateArray = completedState;
                    foreach (JToken objective in completedStateArray)
                    {
                        completedObjectives.Add(objective.ToObject<string>());
                    }
                }
            }
        }
        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["questName"] = quest.name;
            JArray completedState = new JArray();
            IList<JToken> completedStateArray = completedState;
            foreach (string objective in completedObjectives)
            {
                completedStateArray.Add(JToken.FromObject(objective));
            }
            stateDict["completedObjectives"] = completedState;
            return state;
        }
    }
}
