using Godot;
using System;
using System.Collections.Generic;

public class Main : Control
{
	//startingLevel determines what level to start the game at
	[Export] Level startingLevel;

	IDictionary<Level, PackedScene> levelDictionary = new Dictionary<Level, PackedScene>();
	//Stores the current level's Packscene
	public PackedScene CurrentLevelPackedScene;
	//Stores the current level as a referenceable node
	public Control CurrentLevelNode;

	public PackedScene playerPackedScene;
	public KinematicBody2D ThePlayer;
	public static int checkpoint = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//When Main is loaded, loads the Levels to be used later
		PackedScene Level1 = GD.Load<PackedScene>("res://Scenes/Levels/Level1.tscn");
		PackedScene Level2 = GD.Load<PackedScene>("res://Scenes/Levels/Level2.tscn");
		PackedScene Level3 = GD.Load<PackedScene>("res://Scenes/Levels/Level3.tscn");
		PackedScene Level3Indoors = GD.Load<PackedScene>("res://Scenes/Levels/Level3Indoors.tscn");
		PackedScene Level4 = GD.Load<PackedScene>("res://Scenes/Levels/Level4.tscn");
		PackedScene BossLevel = GD.Load<PackedScene>("res://Scenes/Levels/BossLevel.tscn");

		//Adds the levels to the dictionary
		levelDictionary.Add(Level.Level1, Level1);
		levelDictionary.Add(Level.Level2, Level2);
		levelDictionary.Add(Level.Level3, Level3);
		levelDictionary.Add(Level.Level3Indoors, Level3Indoors);
		levelDictionary.Add(Level.Level4, Level4);
		levelDictionary.Add(Level.BossLevel, BossLevel);

		playerPackedScene = GD.Load<PackedScene>("res://Scenes/Player.tscn");
		ThePlayer = playerPackedScene.Instance() as KinematicBody2D;

		//Sets the Curr. PackedScene to the Level
		CurrentLevelPackedScene = levelDictionary[startingLevel];
		//Sets the Curr. Node to an instance of the Curr. PackedScene
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;

		ThePlayer.Position = CurrentLevelNode.GetNode<Position2D>("Environment/Checkpoint" + checkpoint).Position;

		//Allows us to test different levels with the abilities the player would have
		switch (startingLevel)
		{
			case Level.Level1:
				break;
			case Level.Level2:
				break;
			case Level.Level3:
				Player.swordHoldEnabled = true;
				Player.swordEquipped = true;
				break;
			case Level.Level3Indoors:
				Player.swordHoldEnabled = true;
				Player.magicAttackEnabled = true;
				Player.swordEquipped = true;
				Player.magicEquipped = false;
				break;
			case Level.Level4:
				Player.swordHoldEnabled = true;
				Player.magicAttackEnabled = true;
				Player.swordEquipped = true;
				Player.magicEquipped = false;
				break;
			case Level.BossLevel:
				Player.swordHoldEnabled = true;
				Player.magicAttackEnabled = true;
				Player.magicJumpEnabled = true;
				Player.swordEquipped = true;
				Player.magicEquipped = false;
				break;
			default:
				break;
		}
		
		//Adds the Level to the Scene (Starts the Level)
		AddChild(CurrentLevelNode);
		CurrentLevelNode.AddChild(ThePlayer);
	}

	//Received from PauseMenu when reload has been selected
	public void ReloadCurrentLevel()
	{
		//Deletes the current Level when it is safe to do so
		CurrentLevelNode.RemoveChild(ThePlayer);
		CurrentLevelNode.QueueFree();
		//Creates a new (clean) instance of the current level
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;

		ThePlayer.Position = CurrentLevelNode.GetNode<Position2D>("Environment/Checkpoint" + checkpoint).Position;

		//Adds the clean Level to the scene (Starts the level again)
		AddChild(CurrentLevelNode);
		CurrentLevelNode.AddChild(ThePlayer);
	}

	//Received from TransitionField when it is time to switch to the next level
	public void NextLevel(Level nextLevel)
	{
		//Adds the specified Level to the curr. packedScene
		CurrentLevelPackedScene = levelDictionary[nextLevel];
		CurrentLevelNode.RemoveChild(ThePlayer);
		//Deletes the current Level when it is safe to do so
		CurrentLevelNode.QueueFree();
		//Creates an instance of the next level
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;

		ThePlayer.Position = CurrentLevelNode.GetNode<Position2D>("Environment/Checkpoint" + checkpoint).Position;

		AddChild(CurrentLevelNode);
		CurrentLevelNode.AddChild(ThePlayer);
		//Adds the next Level to the scene (Starts the next level)
		//CallDeferred so that we don't accidently try to add a child on physics frame
		//This adds the CurrentLevelNode as a child when it is safe to do so
		//CallDeferred("add_child", CurrentLevelNode);
		//CurrentLevelNode.CallDeferred("add_child", Player);
	}
}
