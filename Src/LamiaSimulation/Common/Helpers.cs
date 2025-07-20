using System.Linq;

namespace LamiaSimulation
{
    public static class Helpers
    {
        public static PopulationSpeciesType GetSpeciesTypeById(string ID)
        {
            var filtered = DataQuery<PopulationSpeciesType>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._($"Species {ID} does not exist."));
            return filtered;
        }

        public static TaskType GetTaskTypeById(string ID)
        {
            var filtered = DataQuery<TaskType>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._($"Task {ID} does not exist."));
            return filtered;
        }

        public static ResourceType GetResourceTypeById(string ID)
        {
            var filtered = DataQuery<ResourceType>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._($"Resource {ID} does not exist."));
            return filtered;
        }
        
        public static LocationType GetLocationTypeById(string ID)
        {
            var filtered = DataQuery<LocationType>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._($"Location type {ID} does not exist."));
            return filtered;
        }
        
        public static T1 GetDataTypeById<T1>(string ID) where T1 : DataType
        {
            var filtered = DataQuery<T1>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._($"Data type of name {ID} does not exist."));
            return filtered;
        }
        

    }
}