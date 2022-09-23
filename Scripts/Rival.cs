using Godot;
using System;

public enum CameraDirections
{
	playerToRival,
	rivalToPlayer,
	airToRival,
	airToPlayer
}

public class Rival : BaseEnemy
{
	[Export] int hSpeed;
	[Export] int maxVSpeed;
	[Export] int gravity;

	AnimationPlayer BossBattleAnimationPlayer;
	Player thePlayer;
	AnimationTree rivalAnimationTree;
	AnimationNodeStateMachinePlayback rivalANSMP;
	int timesHit = 0;

	Vector2 prevRivalLocation;
	Vector2 nextRivalLocation;
	bool movingRival;
	float timeToMoveRival;
	float rivalXMovementProgress;
	float rivalYMovementProgress;
	float rivalMoveProgress;

	bool movingCamera;
	CameraDirections movingCameraToPlayer;
	Vector2 cameraStartingPosition;
	Vector2 cameraEndingPosition;
	float timeToMoveCamera;
	float cameraXMovementProgress;
	float cameraYMovementProgress;
	float cameraMoveProgress;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        base._Ready();
		thePlayer = GetNode<Player>("../Player");
		BossBattleAnimationPlayer = GetNode<AnimationPlayer>("../BossBattlePlayer");
		rivalAnimationTree = GetNode<AnimationTree>("RivalAnimationTree");
		rivalANSMP = rivalAnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
		nextRivalLocation = GlobalPosition;
		if (GetDirection() == Direction.Left)
		{
			rivalAnimationTree.Set("parameters/Idle/blend_position", -1);
		}
		else
		{
			rivalAnimationTree.Set("parameters/Idle/blend_position", 1);
		}
	}

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
		MoveCamera(delta);
		MoveRivalSmooth(delta);
    }

	public override void MoveEnemy(float delta)
	{
		CheckForCollision();
		//MoveAndSlide(velocity, floor, false, 4, 0.785398f, false);
	}

	public override void AnimateEnemy()
	{
		if (velocity == Vector2.Zero && !IsOnWall())
		{
			//If not moving, switches to an idle animation
			rivalANSMP.Travel("Idle");
		}
		else
		{
			//If moving, switches to an run animation
			//enemyANSMP.Travel("Run");
			//Changes direction of the animation based on the velocity
			rivalAnimationTree.Set("parameters/Idle/blend_position", velocity.x);
			//animationTree.Set("parameters/Run/blend_position", velocity.x);
		}
	}

	public void OnPlayerEnteredBossBattle(Node body)
    {
        if (body.IsInGroup("Player"))
        {
			BossBattleAnimationPlayer.Play("EnteredBossBattle");
		}
    }

    public override void CheckForCollision()
    {
        base.CheckForCollision();
    }

	public void MoveRival(float seconds)
    {
		movingRival = true;
		timeToMoveRival = seconds;
		rivalXMovementProgress = 0;
		rivalYMovementProgress = 0;
		rivalMoveProgress = 0;
	}

	public void MoveRivalSmooth(float delta)
    {
		if (movingRival)
		{
			if (rivalMoveProgress >= timeToMoveRival)
			{
				movingRival = false;
			}
			else
			{
				rivalMoveProgress += delta;
				rivalXMovementProgress = rivalMoveProgress * (prevRivalLocation.x - nextRivalLocation.x) / timeToMoveRival;
				rivalYMovementProgress = rivalMoveProgress * (prevRivalLocation.y - nextRivalLocation.y) / timeToMoveRival;
				GlobalPosition = new Vector2(prevRivalLocation.x - rivalXMovementProgress, prevRivalLocation.y - rivalYMovementProgress);
			}
		}
	}

    public void MoveCamera(CameraDirections moveToPlayer, float seconds)
    {
        switch (moveToPlayer)
        {
			case CameraDirections.playerToRival:
				cameraStartingPosition = thePlayer.GlobalPosition;
				cameraEndingPosition = GlobalPosition;
				break;
			case CameraDirections.rivalToPlayer:
				cameraStartingPosition = GlobalPosition;
				cameraEndingPosition = thePlayer.GlobalPosition;
				break;
			case CameraDirections.airToRival:
				cameraStartingPosition = thePlayer.GetNode<Camera2D>("Camera2D").GlobalPosition;
				cameraEndingPosition = GlobalPosition;
				break;
			case CameraDirections.airToPlayer:
				cameraStartingPosition = thePlayer.GetNode<Camera2D>("Camera2D").GlobalPosition;
				cameraEndingPosition = thePlayer.GlobalPosition;
				break;
		}
		movingCameraToPlayer = moveToPlayer;
		movingCamera = true;
		timeToMoveCamera = seconds;
		cameraXMovementProgress = 0;
		cameraYMovementProgress = 0;
		cameraMoveProgress = 0;
	}

	public void MoveCamera(float delta)
    {
		if (movingCamera)
		{
			if (cameraMoveProgress >= timeToMoveCamera)
			{
				movingCamera = false;
			}
			else
			{
				cameraMoveProgress += delta;
				cameraXMovementProgress = cameraMoveProgress * (cameraStartingPosition.x - cameraEndingPosition.x) / timeToMoveCamera;
				cameraYMovementProgress = cameraMoveProgress * (cameraStartingPosition.y - cameraEndingPosition.y) / timeToMoveCamera;
				thePlayer.MoveCamera(new Vector2(cameraStartingPosition.x - cameraXMovementProgress, cameraStartingPosition.y - cameraYMovementProgress));
			}
		}
	}
	public void pausePlayer(bool pause)
    {
		GetNode<PauseMenu>("../../GUI/PauseMenu").PauseFromOutsidePauseMenu(pause);
    }

	public void GotHit()
    {
		timesHit++;
		prevRivalLocation = nextRivalLocation;
		if(timesHit < 4)
        {
			prevRivalLocation = nextRivalLocation;
			nextRivalLocation = GetNode<Position2D>("../Environment/RivalPosition" + (timesHit + 1)).GlobalPosition;
			BossBattleAnimationPlayer.Play("BossHit");
		}
        else
        {
			BossBattleAnimationPlayer.Play("RivalLoses");
		}
    }
}
