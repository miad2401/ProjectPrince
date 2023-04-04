using Godot;
using System;

public class OpenDoor : PopUpHint
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public void OnTimerTimeout()
	{
		Area2D DoorArea = GetNode<Area2D>("DoorArea");
		DoorArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		DoorArea.GetNode<Sprite>("Sprite").Frame = 1;
	}

	public override void OnHintArea2DBodyEntered(Node body)
	{
		base.OnHintArea2DBodyEntered(body);
		if (body.IsInGroup("Player"))
		{
			GetNode<Timer>("Timer").Start();
		}
	}
}
