using Godot;
using System;
using LamiaSimulation;

public partial class GameController : Node
{
	public Simulation simulation;

	[Export] public string currentSettlementUuid;
	
	public override void _Ready()
	{
		simulation = Simulation.Instance;
		simulation.Start();
		currentSettlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
	}
	
	public override void _Process(double delta)
	{
		simulation.Simulate((float)delta);
	}

	public void SaveGame()
	{
		simulation.SaveGame();
	}

	public void RestartGame()
	{
		simulation.Reset();
		simulation.Start();
	}
}
