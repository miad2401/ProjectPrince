using Godot;
using System;

public class PauseMenu : Control
{
	//Creates a signal to connect to another script
	[Signal] public delegate void ReloadCurrentLevel();

	[Export] public float ResetLength; //Time needed to reset Level
	// keep track of whether or not to pause game
	bool paused = false;
	//Keeps track of how long the reset button has been held down for
	float resetHeld = 0f;

	Panel ResetProgress;
	CenterContainer PauseCenterContainer;

	public override void _Ready()
	{
		//Connects the ReloadCurrentLevel Signal to Main, causing Main's ReloadCurrentLevel method to run when called
		Connect(nameof(ReloadCurrentLevel), GetParent().GetParent(), "ReloadCurrentLevel");

		//Adds reference to ResetProgress Panel
		ResetProgress = GetNode<Panel>("ResetProgress");
		//Adds reference to CenterContainer PauseCenterContainer
		PauseCenterContainer = GetNode<CenterContainer>("PauseCenterContainer");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		// if pause was just pressed once
		if (Input.IsActionJustPressed("pause")) {
			// flip pause state and call func
			paused = !paused;
			Pause();
		}
		//Checks if the reset button is pressed and that the player is not in a menu
		if (Input.IsActionPressed("ui_reset") && !PauseCenterContainer.Visible)
        {
			//Adds the time held resetHeld
			resetHeld += delta;
			//This line changes the transparency of ResetProgress at a rate to reflect the time reset has been held
			ResetProgress.Modulate = new Color(1, 1, 1, resetHeld * (1 / ResetLength));
			//Checks if the set amount of time to reset has passed
			if(resetHeld > ResetLength)
            {
				//Calls Reset method
				_on_RestartButton_pressed();
				//Resets held time
				resetHeld = 0;
				//Makes ResetProgress invisible again
				ResetProgress.Modulate = new Color(1, 1, 1, 0);
			}
        }
        else
        {
			//Resets held time
			resetHeld = 0;
			//Makes ResetProgress invisible again
			ResetProgress.Modulate = new Color(1, 1, 1, 0);

		}
	}
	
	public void Pause() {
		// if the bool is false (unpaused), hide pause menu
		if (paused == false) {
			PauseCenterContainer.Hide();
		}
		// else show pause menu
		else {
			PauseCenterContainer.Show();
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
	
	private void _on_RestartButton_pressed() {
	// restart button pressed, unpause game then reload scene
		paused = false;
		Pause();
		//Emits the ReloadCurrentLevel signal, causing the ReloadCurrentLevel method to run in Main
		EmitSignal(nameof(ReloadCurrentLevel));
	}
	
	private void _on_MenuButton_pressed() {
	// return to main menu button pressed, unpause game then change to main menu
		paused = false;
		Pause();
		GetTree().ChangeScene("res://Scenes/Menu.tscn");
	}
}












