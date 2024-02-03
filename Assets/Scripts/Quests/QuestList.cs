using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, IJsonSaveable
    {
        List<QuestStatus> questStatuses = new List<QuestStatus>();

        public event Action onNewQuestAdded;
        public event Action onObjectiveCompleted;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            questStatuses.Add(newStatus);
            if(onNewQuestAdded != null)
            {
                onNewQuestAdded();
            }
        }

        public void CompleteObjective(Quest quest, string objectiveToComplete)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objectiveToComplete);
            if (status.IsComplete())
            {
                GiveReward(quest);
            }
            if(onObjectiveCompleted != null)
            {
                onObjectiveCompleted();
            }
        }


        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return questStatuses;
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in questStatuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }
            return null;
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewards())
            {
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item,reward.number);
                if(!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item,reward.number);
                }
            }
        }

        //Saving/Loading Methods
        public JToken CaptureAsJToken()
        {
            JArray state = new JArray();
            IList<JToken> stateList = state;
            foreach (QuestStatus status in questStatuses)
            {
                stateList.Add(status.CaptureAsJToken());
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JArray stateArray)
            {
                questStatuses.Clear();
                IList<JToken> stateList = stateArray;
                foreach (JToken token in stateList)
                {
                    questStatuses.Add(new QuestStatus(token));
                }
            }
        }

    }
}
