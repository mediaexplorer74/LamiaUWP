using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    public enum ResearchBehaviour
    {
        UNLOCK_BUILDING,
    }

    public class ResearchType: DataType
    {
        public string name;
        public string description;
        public ResearchBehaviour behaviour;
        public string unlockId;
        public Dictionary<string, float> cost;
        public List<string> prerequisites;
    }
}