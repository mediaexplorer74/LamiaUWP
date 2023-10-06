using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    [Serializable]
    internal class PopulationMember: SimulationObject, IActionReceiver, IQueryable, ISimulated
    {
        public PopulationSpeciesType species;
        public string name;
        public string taskAssigment;
        public string currentAction;

        private float timeToCompleteCurrentAction;
        private float currentActionProgress;
        private Dictionary<ResourceType, float> inventory;
        private float maxInventory;

        public PopulationMember(string speciesID)
        {
            inventory = new Dictionary<ResourceType, float>();
            var species = DataQuery<PopulationSpeciesType>.GetByID(speciesID);
            this.species = species ?? throw new ClientActionException(T._("Species does not exist."));
            name = NameGenerator.Instance.GenerateFullName();
            maxInventory = species.maxInventory;
            taskAssigment = "idle";
            currentAction = "idle";
        }

        // ---------------------------------------------------
        // IActionReceiver
        // ---------------------------------------------------

        public void PerformAction(ClientAction action)
        {
        }

        public void PerformAction<T>(ClientAction action, ClientParameter<T> param1)
        {
        }

        public void PerformAction<T1, T2>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
        }

        public void PerformAction<T1, T2, T3>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2,
            ClientParameter<T3> param3)
        {
            if(param2.Get as string != ID)
                return;
            switch(action)
            {
                case ClientAction.PopulationAssignToTask:
                    AssignToTask(param3.Get as string);
                    break;
            }
        }

        // ---------------------------------------------------
        // IQueryable
        // ---------------------------------------------------

        public void Query<T>(ref QueryResult<T> result, ClientQuery query)
        {
        }

        public void Query<T, T1>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1)
        {
        }

        public void Query<T, T1, T2>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2)
        {
            if(param2.Get as string != ID)
                return;
            switch(query)
            {
                case ClientQuery.PopulationMemberCurrentAction:
                    result = new QueryResult<string>(currentAction) as QueryResult<T>;
                    break;
                case ClientQuery.PopulationMemberCurrentActionProgress:
                    result = new QueryResult<float>(CurrentActionProgress()) as QueryResult<T>;
                    break;
                case ClientQuery.PopulationMemberCurrentActionName:
                    result = new QueryResult<string>(CurrentActionName()) as QueryResult<T>;
                    break;
            }
        }

        public void Query<T, T1, T2, T3>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
        }

        // ---------------------------------------------------
        // ISimulated
        // ---------------------------------------------------

        public void Simulate(float deltaTime)
        {
            PerformCurrentAction(deltaTime);
        }

        private void AssignToTask(string newTaskId)
        {
            if(taskAssigment == newTaskId)
                throw new ClientActionException(T._("Population member already assigned to task."));
            taskAssigment = newTaskId;
            if(currentAction == "idle")
                StartNewAction();
        }

        private void PerformCurrentAction(float deltaTime)
        {
            if (currentAction == "idle")
                return;
            currentActionProgress += deltaTime;
            if (currentActionProgress >= timeToCompleteCurrentAction)
            {
                CompleteCurrentAction();
                StartNewAction();
            }
        }

        private void CompleteCurrentAction()
        {
            switch (currentAction)
            {
                case "deposit":
                    foreach (var inventoryItem in inventory)
                        inventory[inventoryItem.Key] = 0f;
                    break;
                case "extract":
                    var task = Helpers.GetTaskTypeById(taskAssigment);
                    if (task.behaviour != TaskTypeBehaviour.EXTRACT)
                        return;
                    var resource = Helpers.GetResourceTypeById(task.extractResourceType);
                    if (!inventory.ContainsKey(resource))
                        inventory[resource] = 0f;
                    inventory[resource] += task.amount;
                    break;
            }
        }

        private void StartNewAction()
        {
            currentActionProgress = 0.0f;
            if (InventoryFull())
            {
                currentAction = "deposit";
                timeToCompleteCurrentAction = Consts.depositInventoryTime;
                return;
            }
            switch (taskAssigment)
            {
                case "idle":
                    currentAction = "idle";
                    timeToCompleteCurrentAction = 0.0f;
                    break;
                case "forage":
                    currentAction = "extract";
                    timeToCompleteCurrentAction =  Helpers.GetTaskTypeById(taskAssigment).timeToComplete;                    
                    break;
            }
        }

        private string CurrentActionName()
        {
            switch (currentAction)
            {
                case "idle":
                    return ". . .";
                case "deposit":
                    return T._("Depositing");
                case "extract":
                    switch (taskAssigment)
                    {
                        case "forage":
                            return T._("Foraging");
                        default:
                            return T._("Extracting");
                    }
            }

            return currentAction;
        }
        
        private float CurrentActionProgress()
        {
            if (currentAction == "idle")
                return 0.0f;
            return currentActionProgress / timeToCompleteCurrentAction;
        }

        private bool InventoryFull()
        {
            var currentAmount = 0f;
            foreach (var inventoryItem in inventory)
                currentAmount += inventory[inventoryItem.Key];
            return currentAmount >= maxInventory;
        }
        
    }
}