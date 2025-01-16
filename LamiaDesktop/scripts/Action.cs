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
        Simulation.Instance.PerformAction(ClientAction.UnlockPage, pageId);
    }
    
    public void AddLocation(string locationType)
    {
        Simulation.Instance.PerformAction(ClientAction.AddLocation, locationType);
    }

    public void ConvertLocationToSettlement(string uuid)
    {
        Simulation.Instance.PerformAction(ClientAction.AddSettlementAtLocation, uuid);
    }

    public void AddPopulation(string settlementUuid, string speciesId)
    {
        Simulation.Instance.PerformAction(ClientAction.AddPopulation, settlementUuid, speciesId);
    }
    
    public void RenameSettlement(string settlementUuid, string newName)
    {
        Simulation.Instance.PerformAction(ClientAction.RenameSettlement, settlementUuid, newName);
    }

    public void SendMessage(string message)
    {
        Simulation.Instance.PerformAction(ClientAction.SendMessage, message);
    }
    
    public void UnlockTask(string taskId)
    {
        Simulation.Instance.PerformAction(ClientAction.UnlockTask, taskId);
    }

    public void UnlockBuilding(string buildingId)
    {
        Simulation.Instance.PerformAction(ClientAction.UnlockBuilding, buildingId);
    }
    
    public void PopulationAssignToTask(string settlementUuid, string populationUuid, string taskId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, taskId
        );
    }

    public void AddResourceToSettlementInventory(string settlementUuid, string resourceId, float amount)
    {
        Simulation.Instance.PerformAction(
            ClientAction.AddResourceToSettlementInventory, settlementUuid, resourceId, amount
        );
    }
    
    public void SubtractResourceFromSettlementInventory(string settlementUuid, string resourceId, float amount)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SubtractResourceFromSettlementInventory, settlementUuid, resourceId, amount
        );
    }

    public void SettlementTakeAvailableFoodPortion(string settlementUuid)
    {
        Simulation.Instance.PerformAction(ClientAction.SettlementTakeAvailableFoodPortion, settlementUuid);
    }
    
    public void SettlementRemovePopulation(string settlementUuid, string populationUuid)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SettlementRemovePopulation, settlementUuid, populationUuid
        );
    }
    
    public void SettlementPurchaseBuilding(string settlementUuid, string buildingId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.SettlementPurchaseBuilding, settlementUuid, buildingId
        );
    }

    public void UnlockResearch(string researchId)
    {
        Simulation.Instance.PerformAction(ClientAction.UnlockResearch, researchId);
    }
    
    public void ForceUnlockResearch(string researchId)
    {
        Simulation.Instance.PerformAction(ClientAction.ForceUnlockResearch, researchId);
    }

    public void UnlockUpgrade(string upgradeId)
    {
        Simulation.Instance.PerformAction(ClientAction.UnlockUpgrade, upgradeId);
    }

    public void ForceUnlockUpgrade(string upgradeId)
    {
        Simulation.Instance.PerformAction(ClientAction.ForceUnlockUpgrade, upgradeId);
    }

}