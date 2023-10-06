namespace LamiaSimulation
{
    public enum ClientAction
    {
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
        
        // Reduces an amount of a resource from a location's resources.
        // params:
        //   - string: uuid of location
        //   - string: id of resource
        //   - float: amount of resource to reduce
        SubtractResourceFromLocation,
    }
}