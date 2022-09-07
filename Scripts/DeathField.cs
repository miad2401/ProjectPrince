using Godot;
using System;

public class DeathField : Area2D
{
	//Change values in editor, not in code
	[Export] bool hurtPlayer;
	[Export] bool hurtEnemy;
	[Export] bool hurtMoveableObject;

	//Signal that tells the PauseMenu if the player died
	[Signal] public delegate void PlayerDeath();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Connects signal that tells the PauseMenu if the player died
		Connect(nameof(PlayerDeath), GetNode("/root/Main/GUI/PauseMenu"), "PlayerDied");
	}

	//If the field is entered, checks if the invading body should be deleted, then deletes then if so
	public void _on_DeathField_body_entered(Node body)
	{
		//Checks if the touched object was a player and if hurtPlayer is true, if so, sends the PlayerDeath signal
		if (hurtPlayer && body.IsInGroup("Player"))
		{
			EmitSignal(nameof(PlayerDeath));
		}
		//Checks if the touched object was an enemy and hurtEnemy is true, if so, deletes it
		if (hurtEnemy && body.IsInGroup("Enemy"))
		{
			body.QueueFree();
		}
		//Checks if the touched object was a moveable object and hurtMoveableObject is true, if so, deletes it
		if (hurtMoveableObject && body.IsInGroup("MoveableObject"))
		{
			body.QueueFree();
		}
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
