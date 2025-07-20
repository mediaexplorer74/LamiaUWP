using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    public class BuildingBehaviour
    {
        public int populationCapacity;
        public Dictionary<string, float> storageCapacity;
    }
    
    public class BuildingType: DataType
    {
        public string name;
        public string description;
        public string category;
        public BuildingBehaviour behaviour;
        public Dictionary<string, float> cost;
        public float costGrowth;
        
        public string[] GetDescriptionDisplay()
        {
            var behaviourText = new List<string>{ T._(description), ""};
            var behaviourTextFormat = "";
            if(behaviour.populationCapacity > 0)
                behaviourText.Add(T._($"Increase population capacity by {behaviour.populationCapacity}"));
            if (behaviour.storageCapacity?.Count > 0)
            {
                behaviourText.Add(T._($"Increase storage capacity:"));
                foreach (var singleBehaviour in behaviour.storageCapacity)
                {
                    var resource = Helpers.GetDataTypeById<ResourceType>(singleBehaviour.Key);
                    behaviourText.Add(
                        T._($"{resource.name}: +{singleBehaviour.Value}")
                    );
                }
            }
            return behaviourText.ToArray();
        }
        
    }
}