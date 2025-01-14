using Godot;
using LamiaSimulation;

/*
 * Thin wrapper around all ClientAction calls as you (understandably) cannot call generic C# methods from GDScript.
 */
public partial class Action: Node
{
    public override void _Ready()
    {
    }

    public void UnlockPage(string pageId)
    {
        Simulation.Instance.PerformAction(ClientAction.UnlockPage, new ClientParameter<string>(pageId));
    }
    
    public void AddLocation(string locationType)
    {
        Simulation.Instance.PerformAction(ClientAction.AddLocation, new ClientParameter<string>(locationType));
    }

    public void ConvertLocationToSettlement(string uuid)
    {
        Simulation.Instance.PerformAction(
            ClientAction.AddSettlementAtLocation, 
            new ClientParameter<string>(uuid)
            );
    }

    public void AddPopulation(string settlementUuid, string speciesId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.AddPopulation,
            new ClientParameter<string>(settlementUuid), new ClientParameter<string>(speciesId)
            );
    }
    
    public void RenameSettlement(string settlementUuid, string newName)
    {
        Simulation.Instance.PerformAction(
            ClientAction.RenameSettlement,
            new ClientParameter<string>(settlementUuid), new ClientParameter<string>(newName)
        );
    }

    public void SendMessage(string message)
    {
        Simulation.Instance.PerformAction(ClientAction.SendMessage, new ClientParameter<string>(message));
    }
    
    public void UnlockTask(string taskId)
    {
        Simulation.Instance.PerformAction(ClientAction.UnlockTask, taskId);
    }

    public void PopulationAssignToTask(string settlementUuid, string populationUuid, string taskId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.PopulationAssignToTask,
            new ClientParameter<string>(settlementUuid),
            new ClientParameter<string>(populationUuid),
            new ClientParameter<string>(taskId)
        );
    }

    public void AddResourceToSettlementInventory(string settlementUuid, string resourceId, float amount)
    {
        Simulation.Instance.PerformAction(
            ClientAction.AddResourceToSettlementInventory,
            new ClientParameter<string>(settlementUuid),
            new ClientParameter<string>(resourceId),
            new ClientParameter<System.Single>(amount)
        );
    }
    
    public void SubtractResourceFromSettlementInventory(string settlementUuid, string resourceId, float amount)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SubtractResourceFromSettlementInventory,
            new ClientParameter<string>(settlementUuid),
            new ClientParameter<string>(resourceId),
            new ClientParameter<System.Single>(amount)
        );
    }

    public void SettlementTakeAvailableFoodPortion(string settlementUuid)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SettlementTakeAvailableFoodPortion,
            new ClientParameter<string>(settlementUuid)
        );
    }
    
    public void SettlementRemovePopulation(string settlementUuid, string populationUuid)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SettlementRemovePopulation,
            new ClientParameter<string>(settlementUuid),
            new ClientParameter<string>(populationUuid)
        );
    }

    public void SettlementUnlockBuilding(string settlementUuid, string buildingId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SettlementUnlockBuilding,
            new ClientParameter<string>(settlementUuid),
            new ClientParameter<string>(buildingId)
        );
    }

    public void SettlementPurchaseBuilding(string settlementUuid, string buildingId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SettlementPurchaseBuilding,
            new ClientParameter<string>(settlementUuid),
            new ClientParameter<string>(buildingId)
        );
    }

    public void UnlockResearch(string researchId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.UnlockResearch,
            new ClientParameter<string>(researchId)
        );
    }

    public void UnlockUpgrade(string settlementUuid, string researchId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.UnlockUpgrade,
            new ClientParameter<string>(settlementUuid),
            new ClientParameter<string>(researchId)
        );
    }
    
}