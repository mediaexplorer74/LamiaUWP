using System;

namespace LamiaSimulation
{
    public class ResourceType: DataType
    {
        public string name;
        public string description;
        public string category;
        public float weight;
        public float hungerRecoveryFactor;

        public ResourceCategory GetCategory()
        {
            return Helpers.GetDataTypeById<ResourceCategory>(category);
        }
    }
}