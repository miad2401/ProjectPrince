using Godot;
using System;
using System.Collections.Generic;

public class Main : Control
{
	//startingLevel determines what level to start the game at
	[Export] Level startingLevel;

	//startingCheckpoint determines what checkpoint within the level to start the game at
	[Export] int startingCheckpoint = 1;

	IDictionary<Level, PackedScene> levelDictionary = new Dictionary<Level, PackedScene>();
	//Stores the current level's Packscene
	public PackedScene CurrentLevelPackedScene;
	//Stores the current level as a referenceable node
	public Control CurrentLevelNode;
	public Level CurrentLevelName;
	public PackedScene playerPackedScene;
	public Player thePlayer;
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
		PackedScene TestWorld = GD.Load<PackedScene>("res://Scenes/Levels/TestWorld.tscn");

		//Adds the levels to the dictionary
		levelDictionary.Add(Level.Level1, Level1);
		levelDictionary.Add(Level.Level2, Level2);
		levelDictionary.Add(Level.Level3, Level3);
		levelDictionary.Add(Level.Level3Indoors, Level3Indoors);
		levelDictionary.Add(Level.Level4, Level4);
		levelDictionary.Add(Level.BossLevel, BossLevel);
		levelDictionary.Add(Level.TestWorld, TestWorld);

		playerPackedScene = GD.Load<PackedScene>("res://Scenes/Player.tscn");
		thePlayer = playerPackedScene.Instance() as Player;

		//Sets the Curr. PackedScene to the Level
		CurrentLevelPackedScene = levelDictionary[startingLevel];
		//Sets the Curr. Node to an instance of the Curr. PackedScene
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;
		
		CurrentLevelName = startingLevel;
		//Sets the Curr. checkpoint to starting Checkpoint
		checkpoint = startingCheckpoint;

		thePlayer.Position = CurrentLevelNode.GetNode<Position2D>("Environment/Checkpoint" + checkpoint).Position;

		//Allows us to test different levels with the abilities the player would have
		changePlayerAbilities();
		//Adds the Level to the Scene (Starts the Level)
		CurrentLevelNode.AddChild(thePlayer);
		AddChild(CurrentLevelNode);
		//CurrentLevelNode.AddChild(ThePlayer);
	}

	//Received from PauseMenu when reload has been selected
	public void ReloadCurrentLevel()
	{
		//Deletes the current Level when it is safe to do so
		CurrentLevelNode.RemoveChild(thePlayer);
		CurrentLevelNode.QueueFree();
		//Creates a new (clean) instance of the current level
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;

		thePlayer.Position = CurrentLevelNode.GetNode<Position2D>("Environment/Checkpoint" + checkpoint).Position;
		thePlayer.ResetVariables();

		changePlayerAbilities();
		//Adds the clean Level to the scene (Starts the level again)
		CurrentLevelNode.AddChild(thePlayer);
		AddChild(CurrentLevelNode);
	}

	//Received from TransitionField when it is time to switch to the next level
	public void NextLevel(Level nextLevel)
	{
		//Adds the specified Level to the curr. packedScene
		CurrentLevelPackedScene = levelDictionary[nextLevel];
		CurrentLevelName = nextLevel;
		CurrentLevelNode.RemoveChild(thePlayer);
		//Deletes the current Level when it is safe to do so
		CurrentLevelNode.QueueFree();
		//Creates an instance of the next level
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;

		thePlayer.Position = CurrentLevelNode.GetNode<Position2D>("Environment/Checkpoint" + checkpoint).Position;
		thePlayer.ResetVariables();

		changePlayerAbilities();

		CurrentLevelNode.AddChild(thePlayer);
		AddChild(CurrentLevelNode);
	}

	public void changePlayerAbilities()
    {
        switch (CurrentLevelName)
        {
			case Level.Level1:
				thePlayer.SetPlayerAbility(true, false, 3);
				break;
			case Level.Level2:
				thePlayer.SetPlayerAbility(true, false, 3);
				break;
			case Level.Level3:
				thePlayer.SetPlayerAbility(true, true, 0);
				thePlayer.SetPlayerAbility(true, false, 1);
				break;
			case Level.Level3Indoors:
				thePlayer.SetPlayerAbility(true, true, 0);
				break;
			case Level.Level4:
				thePlayer.SetPlayerAbility(true, true, 4);
				thePlayer.SetPlayerAbility(false, false, 2);
				break;
			case Level.BossLevel:
				thePlayer.SetPlayerAbility(true, true, 4);
				break;
			case Level.TestWorld:
				thePlayer.SetPlayerAbility(true, false, 3);
				break;
		}
    }
}
