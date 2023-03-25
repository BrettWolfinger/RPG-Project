using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour 
    {
        IAction currentAction;
        public void StartAction(IAction action)
        {
            //same action being scheduled, nothing to cancel
            if(currentAction == action) return;
            if(currentAction != null)
            {
                currentAction.Cancel();
            }
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}