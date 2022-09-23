using Godot;
using System;

public class Menu : Control
{	
	private void _on_StartButton_pressed()
	{
		// GetTree().ChangeScene("res://scenes/OpeningCutscene.tscn");
		GetTree().ChangeScene("res://Scenes/Main.tscn");
	}
	
	private void _on_QuitButton_pressed() {
	// quit game
		GetTree().Quit();
	}
	
	// TODO: options
	/*
	private void _on_OptionsButton_pressed() {
		// open options
	}
	
	*/
}









