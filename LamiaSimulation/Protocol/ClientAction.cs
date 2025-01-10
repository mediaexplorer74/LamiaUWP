namespace LamiaSimulation
{
    public enum ClientAction
    {
        // Unlocks a page for the client
        //   - string: Page ID to unlock
        UnlockPage,
        
        // Adds a location to the simulation
        //   - string: Location type to add
        AddLocation,

        // Creates a new player settlement at a world location
        //   - string: uuid of the location to add a settlement at
        AddSettlementAtLocation,

        // Add one new population to a settlement
        // params:
        //   - string: uuid of settlement to add to
        //   - string: ID of population species of member to add
        AddPopulation,

        // Renames a settlement to something else
        // params:
        //   - string: uuid of settlement
        //   - string: new name
        RenameSettlement,

        // Sends a message to the client to display
        // params:
        //   - string: message to display
        SendMessage,

        // Unlocks a task so it's available for population assignment
        // params:
        //   - string: uuid of settlement
        //   - string: id of task
        UnlockTask,

        // Assigns a population to a particular task
        // params:
        //   - string: uuid of settlement
        //   - string: uuid of population member
        //   - string: id of new task
        PopulationAssignToTask,

        // Adds an amount of a resource to a settlement inventory. Any resource that can't be stored is discarded.
        // params:
        //   - string: uuid of settlement
        //   - string: id of resource
        //   - float: amount of resource to add
        AddResourceToSettlementInventory,

        // Reduces an amount of a resource from a settlement's inventory
        // params:
        //   - string: uuid of settlement
        //   - string: id of resource
        //   - float: amount of resource to reduce
        SubtractResourceFromSettlementInventory,

        // Reduces an amount of a resource from a location's resources.
        // params:
        //   - string: uuid of location
        //   - string: id of resource
        //   - float: amount of resource to reduce
        SubtractResourceFromLocation,
        
        // Takes the next available food portion from settlement inventory
        // params:
        //   - string: uuid of settlement
        SettlementTakeAvailableFoodPortion,
        
        // Permanently removes a population from a settlement, essentially killing them
        // params:
        //   - string: uuid of settlement
        //   - string: uuid of population member
        SettlementRemovePopulation,
        
        // Unlock building at a settlement
        // params:
        //   - string: uuid of settlement
        //   - string: id of building type
        SettlementUnlockBuilding,
        
        // Purchases a building at a settlement if it can be afforded
        // params:
        //   - string: uuid of settlement
        //   - string: id of building type
        SettlementPurchaseBuilding,

        // Adds a building to a settlement, bypassing requirements or resource requirements
        // params:
        //   - string: uuid of settlement
        //   - string: id of building type
        SettlementForceAddBuilding,

        // Unlocks a research and applies it's effects
        // params:
        //   - string: id of research to unlock
        UnlockResearch,

        // Unlocks an upgrade if have requirements and can be afforded and applies it's effects
        // params:
        //   - string: uuid of settlement
        //   - string: id of upgrade to unlock
        UnlockUpgrade,
        
        // Unlocks an upgrade and applies it's effects, completely bypassing requirements and costs
        // params:
        //   - string: uuid of settlement
        //   - string: id of upgrade to unlock
        ForceUnlockUpgrade,
        
    }
}