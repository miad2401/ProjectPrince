using Godot;
using System;

public class Main : Control
{
	//Each Level is stored as a PackedScene
	public PackedScene Level1;
	public PackedScene Level2;
	public PackedScene Level3;
	public PackedScene Level3Indoors;
	public PackedScene Level4;
	public PackedScene BossLevel;

	//Stores the current level's Packscene
	public PackedScene CurrentLevelPackedScene;
	//Stores the current level as a referenceable node
	public Control CurrentLevelNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//When Main is loaded, loads the Levels to be used later
		Level1 = GD.Load<PackedScene>("res://Scenes/Levels/Level1.tscn");
		Level2 = GD.Load<PackedScene>("res://Scenes/Levels/Level2.tscn");
		Level3 = GD.Load<PackedScene>("res://Scenes/Levels/Level3.tscn");
		Level3Indoors = GD.Load<PackedScene>("res://Scenes/Levels/Level3Indoors.tscn");
		Level4 = GD.Load<PackedScene>("res://Scenes/Levels/Level4.tscn");
		BossLevel = GD.Load<PackedScene>("res://Scenes/Levels/BossLevel.tscn");

		//Starts with Level1
		//Sets the Curr. PackedScene to the First Level
		CurrentLevelPackedScene = Level1;
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
		switch (nextLevel)
		{
			case Level.Level1:
				CurrentLevelPackedScene = Level1;
				break;
			case Level.Level2:
				CurrentLevelPackedScene = Level2;
				break;
			case Level.Level3:
				CurrentLevelPackedScene = Level3;
				break;
			case Level.Level3Indoors:
				CurrentLevelPackedScene = Level3Indoors;
				break;
			case Level.Level4:
				CurrentLevelPackedScene = Level4;
				break;
			case Level.BossLevel:
				CurrentLevelPackedScene = BossLevel;
				break;
			default:
				CurrentLevelPackedScene = Level1;
				break;
		}
		
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
