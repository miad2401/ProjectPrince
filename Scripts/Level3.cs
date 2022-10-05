using Godot;
using System;

public class Level3 : Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	//Signal to changeMusic
	[Signal] public delegate void playMusic(AudioStream music);
	//Holder for music 
	AudioStream music;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Load music
		music = GD.Load<AudioStream>("res://Sounds/Music/Level3Outdoors.mp3");
		Connect(nameof(playMusic), GetNode(".."), "changeMusic");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
  	{
		//Play music
		AudioStreamPlayer player = GetNode<AudioStreamPlayer>("../Music");
		if (!player.Playing)
		{
			EmitSignal(nameof(playMusic), music);
		}
	}
}
