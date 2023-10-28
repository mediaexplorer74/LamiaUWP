using System;
using System.Collections.Generic;
using System.Linq;

namespace LamiaSimulation
{
    internal class Location : SimulationObject, IActionReceiver, IQueryable, ISimulated
    {
        private LocationType locationType => DataQuery<LocationType>.GetByID(locationTypeName);
        public string locationTypeName { get; set; }
        public Dictionary<string, float> availableResources { get; set; }

        public Location(){ }
        
        public Location(string locationTypeName)
        {
            this.locationTypeName = locationTypeName;
            availableResources = new Dictionary<string, float>();
            foreach(var resource in locationType.resources)
                availableResources[resource.Key] = resource.Value;
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
                    var resourceTypeId = param2.Get as string; 
                    var amount = param3.Coerce<float>();
                    if (!availableResources.ContainsKey(resourceTypeId))
                        throw new ClientActionException(T._("Resource not available at location."));
                    availableResources[resourceTypeId] -= amount;
                    if (availableResources[resourceTypeId] < 0f)
                        availableResources[resourceTypeId] = 0f;
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

        public void LoadedFromSave()
        {
        }
        
        public void Simulate(float deltaTime)
        {
        }
        
        private string[] GetResourceList()
        {
            return availableResources.Keys.ToArray();
        }

        private float GetResourceAmount(string resourceId)
        {
            return !availableResources.ContainsKey(resourceId) ? 0f : availableResources[resourceId];
        }
        
    }
}