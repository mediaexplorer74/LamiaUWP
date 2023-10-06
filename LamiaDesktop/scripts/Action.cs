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

    public void AddLocation(string locationType)
    {
        Simulation.Instance.PerformAction(ClientAction.AddLocation, new ClientParameter<string>(locationType));
    }

    public void ConvertLocationToSettlement(string uuid)
    {
        Simulation.Instance.PerformAction(
            ClientAction.ConvertLocationToSettlement, 
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
    
    public void UnlockTask(string settlementUuid, string taskId)
    {
        Simulation.Instance.PerformAction(
            ClientAction.UnlockTask,
            new ClientParameter<string>(settlementUuid), new ClientParameter<string>(taskId)
        );
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
    
}