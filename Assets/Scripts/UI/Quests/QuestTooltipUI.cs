using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI rewardText;
        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();

            //Clear out any existing objectives from prefabs
            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            foreach(Quest.Objective objecive in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;
                if(status.IsObjectiveComplete(objecive.reference))
                {
                    prefab = objectivePrefab;
                }
                GameObject objectiveInstance = Instantiate(prefab,objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objecive.description;
            }

            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            string rewardString = "";
            foreach (var reward in quest.GetRewards())
            {
                if(rewardString != "")
                {
                    rewardString += ", ";
                }
                if(reward.number > 1)
                {
                    rewardString += reward.number + " ";
                    rewardString += reward.item.GetDisplayName() + "s";
                }
                else
                {
                    rewardString += reward.item.GetDisplayName();
                }
            }
            if(rewardString == "")
            {
                rewardString = "None";
            }
            rewardString += ".";
            return rewardString;
        }
    }
}