using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    public enum ResearchBehaviourMethod
    {
        UNLOCK_BUILDING,
        UNLOCK_TASK,
    }
    
    public class ResearchBehaviour
    {
        public ResearchBehaviourMethod method;
        public string id;
    }    

    public class ResearchType: DataType
    {
        public string name;
        public string description;
        public List<ResearchBehaviour> behaviour;
        public Dictionary<string, float> cost;
        public List<string> prerequisites;
        public string unlockMessage;
    }
}