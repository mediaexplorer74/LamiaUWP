using System;
using System.Collections.Generic;
using System.Linq;

namespace LamiaSimulation
{
    [Serializable]
    internal class Location : SimulationObject, IActionReceiver, IQueryable, ISimulated
    {
        public LocationType locationType;
        public Dictionary<ResourceType, float> availableResources;

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
            if(param1.Get as string != ID)
                return;
            switch (action)
            {
                // Subtract resource from location
                case ClientAction.SubtractResourceFromLocation:
                    var resource = Helpers.GetResourceTypeById(param2.Get as string);
                    var amount = param3.Coerce<float>();
                    if (!availableResources.ContainsKey(resource))
                        throw new ClientActionException(T._("Resource not available at location."));
                    availableResources[resource] -= amount;
                    if (availableResources[resource] < 0f)
                        availableResources[resource] = 0f;
                    break;
            }
        }
        
        public void Query<T>(ref QueryResult<T> result, ClientQuery query)
        {
        }

        public void Query<T, T1>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1)
        {
            if(param1.Get as string != ID)
                return;
            switch (query)
            {
                // Location resources available
                case ClientQuery.LocationResources:
                    result = new QueryResult<string[]>(GetResourceList()) as QueryResult<T>;
                    break;
            }
        }

        public void Query<T, T1, T2>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2)
        {
            if(param1.Get as string != ID)
                return;
            switch (query)
            {
                // Location resource amount
                case ClientQuery.LocationResourceAmount:
                    result = new QueryResult<float>(GetResourceAmount(param2.Get as string)) as QueryResult<T>;
                    break;
            }
        }

        public void Query<T, T1, T2, T3>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
        }

        public void Simulate(float deltaTime)
        {
        }
        
        private string[] GetResourceList()
        {
            var resources = availableResources.Keys.Select(resource => resource.ID).Distinct().ToList();
            return resources.ToArray();
        }

        private float GetResourceAmount(string resourceId)
        {
            var resource = Helpers.GetResourceTypeById(resourceId);
            return !availableResources.ContainsKey(resource) ? 0f : availableResources[resource];
        }
        
    }
}