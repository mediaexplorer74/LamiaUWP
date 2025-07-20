using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    public enum TaskTypeBehaviourMethod
    {
        IDLE,
        RESEARCH,
        EXTRACT,
        CONSUME,
        CREATE
    }
    
    public class TaskTypeBehaviour
    {
        public TaskTypeBehaviourMethod method;
        public string id;
        public float value;
    }
    
    public class TaskType: DataType
    {
        public string name;
        public string description;
        public List<TaskTypeBehaviour> behaviour;
        public float timeToComplete;
        public float hungerReduction;
        public string actionText;

        public static TaskType GetTaskById(string taskId)
        {
            var filtered = DataQuery<TaskType>.GetByID(taskId);
            if(filtered == null)
                throw new ClientActionException(T._("Task does not exist."));
            if(!Simulation.Instance.Query<bool, string>(ClientQuery.TaskUnlocked, taskId))
                throw new ClientActionException(T._("Task not unlocked."));
            return filtered;
        }
        
        public string[] GetDescriptionDisplay(string settlementUuid)
        {
            var behaviourText = new List<string>{ T._(description), "" };
            foreach(var(i, singleBehaviour) in behaviour.Enumerate())
            {
                if(singleBehaviour.method == TaskTypeBehaviourMethod.IDLE)
                    return new[] { T._(description) };
                var exactAmount = Settlement.GetExtractTaskAmount(settlementUuid, ID, i);
                var exactTimeToComplete = Settlement.GetTimeToCompleteTask(settlementUuid, ID);
                var resourceTypeName = "";
                var textFormat = "";
                switch (singleBehaviour.method)
                {
                    case TaskTypeBehaviourMethod.RESEARCH:
                        textFormat = T._("Generate: {0} Research/{1} secs");
                        behaviourText.Add(string.Format(textFormat, exactAmount, exactTimeToComplete));
                        break;
                    case TaskTypeBehaviourMethod.EXTRACT:
                        resourceTypeName = T._(DataQuery<ResourceType>.GetByID(singleBehaviour.id).name);
                        textFormat = T._("Extract: {0} {1}/{2} secs");
                        behaviourText.Add(string.Format(textFormat, resourceTypeName, exactAmount,
                            exactTimeToComplete));
                        break;
                    case TaskTypeBehaviourMethod.CONSUME:
                        resourceTypeName = T._(DataQuery<ResourceType>.GetByID(singleBehaviour.id).name);
                        textFormat = T._("Consume: {0} {1}/{2} secs");
                        behaviourText.Add(string.Format(textFormat, resourceTypeName, exactAmount,
                            exactTimeToComplete));
                        break;
                    case TaskTypeBehaviourMethod.CREATE:
                        resourceTypeName = T._(DataQuery<ResourceType>.GetByID(singleBehaviour.id).name);
                        textFormat = T._("Create: {0} {1}/{2} secs");
                        behaviourText.Add(string.Format(textFormat, resourceTypeName, exactAmount,
                            exactTimeToComplete));
                        break;
                }
            }

            return behaviourText.ToArray();
        }
    }
}