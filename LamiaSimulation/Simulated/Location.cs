using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    [Serializable]
    internal class Location : SimulationObject, IActionReceiver, IQueryable, ISimulated
    {
        private LocationType locationType;
        private Dictionary<ResourceType, float> availableResources;

        public Location(string locationTypeName)
        {
            locationType = DataQuery<LocationType>.GetByID(locationTypeName);
            availableResources = new Dictionary<ResourceType, float>();
            foreach(var resource in locationType.resources)
            {
                var resourceType = DataQuery<ResourceType>.GetByID(resource.Key);
                availableResources[resourceType] = resource.Value;
            }
        }

        public Settlement CreateSettlementAt(string name)
        {
            var settlement = new Settlement(name)
            {
                locationType = locationType,
                availableResources = new Dictionary<ResourceType, float>(availableResources)
            };
            return settlement;
        }

        public void PerformAction(ClientAction action)
        {
        }

        public void PerformAction<T1>(ClientAction action, ClientParameter<T1> param1)
        {
        }

        public void PerformAction<T1, T2>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
        }

        public void PerformAction<T1, T2, T3>(ClientAction action, ClientParameter<T1> param1,
            ClientParameter<T2> param2,
            ClientParameter<T3> param3)
        {
        }

        public void Query<T1>(ref QueryResult<T1> result, ClientQuery query)
        {
        }

        public void Query<T, T1>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1)
        {
        }

        public void Query<T, T1, T2>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2)
        {
        }

        public void Query<T, T1, T2, T3>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
        }

        public void Simulate(float deltaTime)
        {
        }
    }
}