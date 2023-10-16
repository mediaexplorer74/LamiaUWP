using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    public enum BuildingBehaviour
    {
        POPULATION_CAPACITY,
    }
    
    public class BuildingType: DataType
    {
        public string name;
        public string description;
        public string category;
        public BuildingBehaviour behaviour;
        public float behaviourValue;
        public Dictionary<string, float> cost;
        public float costGrowth;
        
        public string[] GetDescriptionDisplay()
        {
            var behaviourText = "";
            switch(behaviour)
            {
                case BuildingBehaviour.POPULATION_CAPACITY:
                    var behaviourTextFormat = T._("Increase population capacity by {0}");
                    behaviourText = String.Format(behaviourTextFormat, (int)behaviourValue);
                    break;
            }
            return new[] {T._(description), "", behaviourText};
        }
        
    }
}