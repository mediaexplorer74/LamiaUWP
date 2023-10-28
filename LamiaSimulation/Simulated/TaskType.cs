using System;

namespace LamiaSimulation
{
    public enum TaskTypeBehaviour
    {
        IDLE,
        RESEARCH,
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
        public float hungerReduction;

        public string[] GetDescriptionDisplay(string settlementUuid)
        {
            var behaviourText = "";
            var exactAmount = 0f;
            var exactTimeToComplete = 0f;
            switch(behaviour)
            {
                case TaskTypeBehaviour.IDLE:
                    return new[] { T._(description) };
                case TaskTypeBehaviour.RESEARCH:
                    var researchTextFormat = T._("Generate: {0} Research/{1} secs");
                    exactAmount = Settlement.GetExtractTaskAmount(settlementUuid, ID);
                    exactTimeToComplete = Settlement.GetTimeToCompleteTask(settlementUuid, ID);
                    behaviourText = String.Format(researchTextFormat, exactAmount, exactTimeToComplete);
                    break;
                case TaskTypeBehaviour.EXTRACT:
                    var resourceTypeName = T._(DataQuery<ResourceType>.GetByID(extractResourceType).name);
                    var behaviourTextFormat = T._("Extract: {0} {1}/{2} secs");
                    exactAmount = Settlement.GetExtractTaskAmount(settlementUuid, ID);
                    exactTimeToComplete = Settlement.GetTimeToCompleteTask(settlementUuid, ID);
                    behaviourText = String.Format(behaviourTextFormat, resourceTypeName, exactAmount, exactTimeToComplete);
                    break;
            }
            return new[] {T._(description), "", behaviourText};
        }
    }
}