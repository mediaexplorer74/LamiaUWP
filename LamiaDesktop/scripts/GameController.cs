using Godot;
using System;
using LamiaSimulation;

public partial class GameController : Node
{
	public Simulation simulation;

	[Export] public string currentSettlementUuid;
	[Export] public bool ready;
	
	public override void _Ready()
	{
		simulation = Simulation.Instance;
		simulation.LoadGame();
		simulation.Start();
		currentSettlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
		ready = true;
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
