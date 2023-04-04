using Godot;
using System;

public class CheckpointField : Area2D
{
	[Signal] public delegate void SetPlayerAbility(bool add, bool clearFirst, int abilityID);

	//Change values in editor, not in code
	[Export] int nextCheckpoint;
	[Export] bool SwordEnabled;
	[Export] bool RangedEnabled;
	[Export] bool MagicJumpEnabled;

	public override void _Ready()
	{
		if (GetParent().Name.Contains("Level") || GetParent().Name.Contains("Test"))
		{
			Connect(nameof(SetPlayerAbility), GetNode("../Player"), "SetPlayerAbility");
		}
		else
		{
			Connect(nameof(SetPlayerAbility), GetNode("../../Player"), "SetPlayerAbility");
		}
	}
	private void OnCheckpointFieldBodyEntered(Node body)
	{
		//Checks if the touched object was a player, if so, update checkpoint
		if (body.IsInGroup("Player"))
		{
			if(Main.checkpoint < nextCheckpoint){
				Main.checkpoint = nextCheckpoint;
			}

			if (SwordEnabled) { EmitSignal(nameof(SetPlayerAbility), true, true, 0); }
			if (RangedEnabled) { EmitSignal(nameof(SetPlayerAbility), true, false, 1); }
			if (MagicJumpEnabled) { EmitSignal(nameof(SetPlayerAbility), true, false, 2); }
			QueueFree();
		}
	}
}
