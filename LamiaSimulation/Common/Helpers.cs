using System.Linq;

namespace LamiaSimulation
{
    public static class Helpers
    {
        public static PopulationSpeciesType GetSpeciesTypeById(string ID)
        {
            var filtered = DataQuery<PopulationSpeciesType>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._("Species does not exist."));
            return filtered;
        }

        public static TaskType GetTaskTypeById(string ID)
        {
            var filtered = DataQuery<TaskType>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._("Task does not exist."));
            return filtered;
        }

        public static ResourceType GetResourceTypeById(string ID)
        {
            var filtered = DataQuery<ResourceType>.GetByID(ID);
            if(filtered == null)
                throw new ClientActionException(T._("Resource does not exist."));
            return filtered;
        }

    }
}