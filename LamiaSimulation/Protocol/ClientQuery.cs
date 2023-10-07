namespace LamiaSimulation
{
    public enum ClientQuery
    {
        /*
         * Available game pages
         *   Result - (string, string)[]: page id and display name
         */
        AvailablePages,

        /*
         * Unread messages to show the player
         *   Result - string[]: Messages to display
         */
        UnreadMessages,

        // --------------------------------------------------------------------
        // RESOURCES
        // --------------------------------------------------------------------

        /*
         * Display name for a resource
         *   Result - string: Name of a resource
         *   Params:
         *     - string: Resource id to query
         */
        ResourceName,
        
        /*
         * Description of a resource
         *   Result - string: Description of a resource
         *   Params:
         *     - string: Resource id to query
         */
        ResourceDescription,
        
        // --------------------------------------------------------------------
        // LOCATIONS
        // --------------------------------------------------------------------
        
        /*
         * All UUIDs of world locations
         *   Result - string[]: UUIDs
         */
        Locations,
        
        /*
         * All resources that are available at a particular location
         *   Result - string[]: Resource IDs available
         *   Params:
         *     - string: uuid of location to query
         */
        LocationResources,

        /*
         * Amount of a particular resource that is available at a location
         *   Result - float: Amount of resource available
         *   Params:
         *     - string: uuid of location to query
         *     - string: resource ID to query
         */
        LocationResourceAmount,
        
        // --------------------------------------------------------------------
        // SETTLEMENTS
        // --------------------------------------------------------------------

        /*
         * All UUIDs of player settlements
         *   Result - string[]: UUIDs
         */
        Settlements,

        /*
         * Uuid of the location the settlement is placed at
         *   Result - string
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementLocation,
        
        /*
         * Displayable name of the settlement
         *   Result - string
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementName,

        /*
         * Population count of a settlement
         *   Result - int
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementCurrentPopulation,

        /*
         * Maximum population count of a settlement
         *   Result - int
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementPopulationMax,

        /*
         * IDs of all population members in a settlement
         *   Result - string[]: uuids of pop members
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementPopulationMembers,

        /*
         * IDs of all species represented in settlement population
         *   Result - string[]: IDs of species
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementPopulationSpecies,

        /*
         * IDs of all members of a particular species in settlement
         *   Result - string[]: uuids of pop members
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: ID of species to query
         */
        SettlementPopulationSpeciesMembers,
        
        /*
         * All resources that are being stored in a settlement's inventory
         *   Result - string[]: Resource IDs being stored
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementInventory,

        /*
         * Amount of a particular resource that is being stored in a settlement
         *   Result - float: Amount of resource being stored
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: resource ID to query
         */
        SettlementInventoryResourceAmount,
        
        /*
         * Maximum amount of a resource that can be stored in a settlement
         *   Result - float: Maximum amount of the resource can be stored
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: resource ID to query
         */
        SettlementInventoryResourceCapacity,
        
        /*
         * Amount that the resource in the settlement changed in the last second
         *   Result - float: Resource change in last second
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: resource ID to query
         */
        SettlementInventoryResourceDelta,
        
        
        // --------------------------------------------------------------------
        // POPULATION
        // --------------------------------------------------------------------
        /*
         * Displayable name of a population member
         *   Result - string
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberName,

        /*
         * Species of a population member
         *   Result - string: ID of species
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberSpecies,

        /*
         * Task assigned to a population member
         *   Result - string: ID of assigned task
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberTask,

        /*
         * Short name for the action the population is doing
         *   Result - string: Action short name
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberCurrentAction,
        
        /*
         * Progress of the current action that the population is doing
         *   Result - float: Number between 0 and 1 representing the action progress
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberCurrentActionProgress,

        /*
         * The display name for the current action that the pop member is doing right now
         *   Result - string: Name of the current action to display to the user
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberCurrentActionName,
        
        /*
         * Get the top-level state of the population ("task", "wait" etc)
         *   Result - string: State name
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberState,

        /*
         * Gives back the message to display if the population is in a "wait" state
         *   Result - string: Wait message
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberWaitMessage,
        
        /*
         * Progress number representing how full the population's inventory is
         *   Result - float: Number between 0 and 1 representing how full their inventory is
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberInventoryProgress,

        /*
         * How full the population's stomach is
         *   Result - float: Number between 0 and 1 representing how full their stomach is
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberHunger,
        
        
        // --------------------------------------------------------------------
        // SPECIES
        // --------------------------------------------------------------------
        /*
         * Displayable name of a population species
         *   Result - string
         *   Params:
         *     - string: ID of species
         */
        SpeciesName,

        /*
         * Displayable description of a population species
         *   Result - string[]: Lines of description
         *   Params:
         *     - string: ID of species
         */
        SpeciesDescription,

        // --------------------------------------------------------------------
        // TASKS
        // --------------------------------------------------------------------

        /*
         * Available population tasks
         *   Result - string[]: ID of tasks
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementTasks,
        
        /*
         * If a task has been unlocked
         *   Result - bool
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: task ID to query
         */
        SettlementTaskUnlocked,

        /*
         * Ids of population members assigned to a task
         *   Result -  string[]: UUIDs of population members assigned to a particular task
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: id of task
         */
        SettlementTaskAssignments,

        /*
         * Display name of task
         *   Result - string: Displayable name for task
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: id of task
         */
        SettlementTaskName,

        /*
         * Description of task
         *   Result - string[]: Lines of description
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: id of task
         */
        SettlementTaskDescription,

        /*
         * Number of population assigned to a task
         *   Result - int: Num of population assigned
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: id of task
         */
        SettlementTaskAssignedNum,

        /*
         * How many populations are allowed to be assigned to a task
         *   Result - int: Capacity for task querying. -1 indicates infinite capacity.
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: id of task
         */
        SettlementTaskMaximumCapacity,

    }
}