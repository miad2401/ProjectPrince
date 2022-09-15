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

		//Sets the Curr. PackedScene to the Level
		CurrentLevelPackedScene = levelDictionary[startingLevel];
		//Sets the Curr. Node to an instance of the Curr. PackedScene
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;
		//Adds the Level to the Scene (Starts the Level)
		AddChild(CurrentLevelNode);
	}

	//Received from PauseMenu when reload has been selected
	public void ReloadCurrentLevel()
	{
		//Deletes the current Level when it is safe to do so
		CurrentLevelNode.QueueFree();
		//Creates a new (clean) instance of the current level
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;
		//Adds the clean Level to the scene (Starts the level again)
		AddChild(CurrentLevelNode);
	}

	//Received from TransitionField when it is time to switch to the next level
	public void NextLevel(Level nextLevel)
	{
		//Adds the specified Level to the curr. packedScene
		CurrentLevelPackedScene = levelDictionary[nextLevel];
		//Deletes the current Level when it is safe to do so
		CurrentLevelNode.QueueFree();
		//Creates an instance of the next level
		CurrentLevelNode = CurrentLevelPackedScene.Instance() as Control;
		//Adds the next Level to the scene (Starts the next level)
		//CallDeferred so that we don't accidently try to add a child on physics frame
		//This adds the CurrentLevelNode as a child when it is safe to do so
		CallDeferred("add_child", CurrentLevelNode);
	}
}
