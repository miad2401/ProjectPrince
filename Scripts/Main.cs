using Godot;
using System;

public class Main : Control
{
	//Each Level is stored as a PackedScene
	public PackedScene Level1;
	public PackedScene Level2;
	public PackedScene Level3;
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

	//Received from _ when it is time to switch to the next level
	public void NextLevel(String Level)
	{
		//Adds the specified Level to the curr. packedScene
		switch (Level)
		{
			case "Level1":
				CurrentLevelPackedScene = Level1;
				break;
			case "Level2":
				CurrentLevelPackedScene = Level2;
				break;
			case "Level3":
				CurrentLevelPackedScene = Level3;
				break;
			case "Level4":
				CurrentLevelPackedScene = Level4;
				break;
			case "BossLevel":
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
		AddChild(CurrentLevelNode);
	}
}
