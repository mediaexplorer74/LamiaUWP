using System;

namespace LamiaSimulation
{
    public enum TaskTypeBehaviour
    {
        IDLE,
        EXTRACT
    }

    public class TaskType: DataType
    {
        public string name;
        public string description;
        public TaskTypeBehaviour behaviour;
        public string extractResourceType;
        public float amount;
        public float timeToComplete;

        public string[] GetDescriptionDisplay()
        {
            var behaviourText = "";
            switch(behaviour)
            {
                case TaskTypeBehaviour.IDLE:
                    return new[] { T._(description) };
                case TaskTypeBehaviour.EXTRACT:
                    var resourceTypeName = T._(DataQuery<ResourceType>.GetByID(extractResourceType).name);
                    var behaviourTextFormat = T._("Extract: {0} {1}/{2} secs");
                    behaviourText = String.Format(behaviourTextFormat, resourceTypeName, amount, timeToComplete);
                    break;
            }
            return new[] {T._(description), "", behaviourText};
        }
    }
}