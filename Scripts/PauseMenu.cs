using Godot;
using System;

public class PauseMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	// keep track of whether or not to pause game
	bool paused = false;
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  	public override void _Process(float delta) {
		// if pause was just pressed once
		if (Input.IsActionJustPressed("pause")) {
			// flip pause state and call func
			paused = !paused;
			Pause();
		}
	}
	
	public void Pause() {
		// if the bool is false (unpaused), hide pause menu
		if (paused == false) {
			this.Hide();
		}
		// else show pause menu
		else {
			this.Show();
		}
		// set game paused state to bool state
		GetTree().Paused = paused;
	}
	
	private void _on_ResumeButton_pressed() {
	// when resume button is pressed, set pause to false and call func
		paused = false;
		Pause();
	}
	
	private void _on_QuitButton_pressed() {
	// quit button pressed, quit game
		GetTree().Quit();
	}
}






