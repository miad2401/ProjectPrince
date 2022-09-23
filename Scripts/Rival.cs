using Godot;
using System;

public class Rival : BaseEnemy
{
	[Export] int hSpeed;
	[Export] int maxVSpeed;
	[Export] int gravity;

	AnimationPlayer BossBattleAnimationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		BossBattleAnimationPlayer = GetNode<AnimationPlayer>("../BossBattlePlayer");
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void MoveEnemy(float delta)
	{
		CheckForCollision();
		MoveAndSlide(velocity, floor, false, 4, 0.785398f, false);
	}

	public override void AnimateEnemy()
	{
		//Finds the child AnimationTree node and sets a references to it to a AnimationTree variable
		AnimationTree animationTree = GetNode<AnimationTree>("RivalAnimationTree");
		//Finds the AnimationNodeStateMachinePlayback resource within the animationTree and sets it to its own variable
		//Because Godot doesn't allow arguments in the .Get() function, we also must cast it as a AnimationNodeStateMachinePlayback
		AnimationNodeStateMachinePlayback enemyANSMP = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
		if (velocity == Vector2.Zero && !IsOnWall())
		{
			//If not moving, switches to an idle animation
			enemyANSMP.Travel("Idle");
		}
		else
		{
			//If moving, switches to an run animation
			//enemyANSMP.Travel("Run");
			//Changes direction of the animation based on the velocity
			animationTree.Set("parameters/Idle/blend_position", velocity.x);
			//animationTree.Set("parameters/Run/blend_position", velocity.x);
		}
	}

	public void OnPlayerEnteredBossBattle(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			BossBattleAnimationPlayer.Play("EnteredBossBattle");
			/*
			GetNode<Area2D>("EnvironmentDetectors/PlayerEnteredBoss").QueueFree();
			StaticBody2D EntryDoor = GetNode<StaticBody2D>("../EntryDoor");
			EntryDoor.GetNode<Sprite>("DoorSprite").Visible = true;
			EntryDoor.GetNode<CollisionShape2D>("DoorCollisionShape2D").GlobalPosition = new Vector2(536,88);
			Camera2D theCamera = GetNode<Camera2D>("../Player/Camera2D");
			theCamera
			//Story Stuff
			*/
		}
	}
}
