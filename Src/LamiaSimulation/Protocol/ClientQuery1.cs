namespace LamiaSimulation
{
    public enum ClientQuery1
    {
        /*
         * Available game pages
         *   Result - (string, string)[]: page id and display name
         */
        AvailablePages,

        /*
         * Available globally-scoped pages 
         *   Result - (string, string)[]: page id and display name
         */
        AvailableGlobalPages,

        /*
         * Available settlement-scoped pages
         *   Params:
         *     - string: settlement id querying the pages of 
         *   Result - (string, string)[]: page id and display name
         */
        AvailableSettlementPages,
        
        /*
         * Already read messages, limits to 10
         *   Result - string[]: Messages to display
         */
        MessageHistory,
        
        /*
         * Unread messages to show the player
         *   Result - string[]: Messages to display
         */
        UnreadMessages,

        /*
         * If the specified page Id has been unlocked
         *   Result - bool
         *   Params:
         *     - string: page id to query
         */
        HasUnlockedPage,
        
        /*
         * If a building has been unlocked and is available to construct
         *   Result - bool
         *   Params:
         *     - string: building ID to query
         */
        HasUnlockedBuilding,

        // --------------------------------------------------------------------
        // RESEARCH
        // --------------------------------------------------------------------

        /*
         * Research available for unlocking
         *   Result - string[]:  Research IDs available
         */
        ResearchAvailable,

        /*
         * Research already completed
         *   Result - string[]:  Research IDs completed
         */
        ResearchUnlocked,

        /*
         * Name of a research to display to the player 
         *   Result - string
         *   Params:
         *     - string: research id to query
         */
        ResearchDisplayName,

        /*
         * Description of a research 
         *   Result - string[]: research description
         *   Params:
         *     - string: research ID to query
         */
        ResearchDescription,
        
        /*
         * Returns if a research can be unlocked with the current available resources
         *   Result - bool
         *   Params:
         *     - string: research ID to query
         */
        ResearchCanAfford,
        
        /*
         * Gives resource types that are required to unlock a research
         *   Result - string[]: Array of resource IDs
         *   Params:
         *     - string: research ID to query
         */
        ResearchResourceList,

        /*
         * Gives exactly how much of a particular resource is required to unlock a research
         *   Result - float: how much of this resource is required
         *   Params:
         *     - string: research ID to query
         *     - string: resource ID to query
         */
        ResearchSingleResourceCost,
        
        // --------------------------------------------------------------------
        // RESOURCES
        // --------------------------------------------------------------------

        /*
         * Display name for a resource category
         *   Result - string: Name of a resource category
         *   Params:
         *     - string: Resource category id to query
         */
        ResourceCategoryName,
        
        /*
         * Description of a resource category
         *   Result - string: Description of a resource category
         *   Params:
         *     - string: Resource category id to query
         */
        ResourceCategoryDescription,
        
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
         * Categories of resources in a settlement's inventory
         *   Result - string[]: Resource category IDs available
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementInventoryCategories,
        
        /*
         * All resources that are being stored in a resource category of a settlement's inventory
         *   Result - string[]: Resource IDs being stored for the specified category
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: resource category id to query
         */
        SettlementInventoryResources,

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
        
        /*
         * How much hunger the next available food portion would heal.
         *   Result - float: Value between 0 and 1 indicating how much hunger would be healed when eating next
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementAvailableFoodPortion,
        
        /*
         * All buildings that are at a settlement
         *   Result - string[]: Building IDs in a settlement
         *   Params:
         *     - string: uuid of settlement to query
         */
        SettlementBuildings,
        
        /*
         * Number of a type of building in a settlement
         *   Result - int: Number of buildings at a settlement
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: building ID to query
         */
        SettlementBuildingsAmount,

        /*
         * Name to show to the user to refer to a building
         *   Result - string: Building name
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: building ID to query
         */
        SettlementBuildingDisplayName,
        
        /*
         * Description of a building in a settlement
         *   Result - string[]: Building description
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: building ID to query
         */
        SettlementBuildingDescription,
        
        /*
         * Returns if a building is available for purchase
         *   Result - bool
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: building ID to query
         */
        SettlementBuildingCanAfford,
        
        /*
         * Gives resource types that are required for a building to be built
         *   Result - string[]: Array of resource IDs 
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: building ID to query
         */
        SettlementBuildingResourceList,

        /*
         * Gives exactly how much of a particular resource is required to construct a building 
         *   Result - float: how much of this resource is required
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: building ID to query
         *     - string: resource ID to query
         */
        SettlementBuildingSingleResourceCost,
        
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
         * How much of a particular resource is in this pop's inventory
         *   Result - float: How many of the resource this pop has
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         *     - string: resource ID to query
         */
        PopulationMemberInventoryResourceAmount,
        
        /*
         * How full the population's stomach is
         *   Result - float: Number between 0 and 1 representing how full their stomach is
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: uuid of population to query
         */
        PopulationMemberHunger,
        
        // --------------------------------------------------------------------
        // UPGRADES
        // --------------------------------------------------------------------

        /*
         * Upgrades available for unlocking
         *   Result - string[]:  Upgrade IDs available
         */
        UpgradesAvailable,

        /*
         * Upgrades already completed
         *   Result - string[]:  Upgrade IDs completed
         */
        UpgradesUnlocked,

        /*
         * Name of a upgrades to display to the player
         *   Result - string
         *   Params:
         *     - string: upgrade id to query
         */
        UpgradeDisplayName,

        /*
         * Description of an upgrade
         *   Result - string[]: upgrade description
         *   Params:
         *     - string: upgrade ID to query
         */
        UpgradeDescription,
        
        /*
         * Returns if an upgrade can be unlocked with the current available resources
         *   Result - bool
         *   Params:
         *     - string: upgrade ID to query
         */
        UpgradeCanAfford,
        
        /*
         * Gives resource types that are required to unlock an upgrade
         *   Result - string[]: Array of resource IDs
         *   Params:
         *     - string: upgrade ID to query
         */
        UpgradeResourceList,

        /*
         * Gives exactly how much of a particular resource is required to unlock an upgrade
         *   Result - float: how much of this resource is required
         *   Params:
         *     - string: upgrade ID to query
         *     - string: resource ID to query
         */
        UpgradeSingleResourceCost,
        
        
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
         */
        Tasks,
        
        /*
         * If a task has been unlocked
         *   Result - bool
         *   Params:
         *     - string: task ID to query
         */
        TaskUnlocked,

        /*
         * Display name of task
         *   Result - string: Displayable name for task
         *   Params:
         *     - string: id of task
         */
        TaskName,

        /*
         * Description of task for a settlement (shows extract rate etc)
         *   Result - string[]: Lines of description
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: id of task
         */
        SettlementTaskDescription,

        /*
         * Ids of population members assigned to a task
         *   Result -  string[]: UUIDs of population members assigned to a particular task
         *   Params:
         *     - string: uuid of settlement to query
         *     - string: id of task
         */
        SettlementTaskAssignments,
        
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

        // --------------------------------------------------------------------
        // GAME STATE
        // --------------------------------------------------------------------

        /*
         * Current game day
         *   Result - int: Current day in the simulation
         */
        CurrentDay,

    }
}