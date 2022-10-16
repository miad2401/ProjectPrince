using Godot;
using System;

public class PauseMenu : Control
{
	//Creates a signal to connect to another script
	[Signal] public delegate void ReloadCurrentLevel(int checkpoint);
	[Signal] public delegate void ChangeLevel(Level nLevel, int checkpoint);

	[Export] public float ResetLength; //Time needed to reset Level
	[Export] public float DeathLength; //Time needed to fade to black when dead
	[Export] public float RespawnLength; //Time needed for respawn button to appear after faded to black
	[Export] public float TransitionLength; //Time needed for Transition to appear then fade

	// keep track of whether or not to pause game
	bool paused = false;
	// keep track of the game has been paused from outside PauseMenu
	bool outsidePaused = false;
	//Keeps track if the game is in a changeable state (if it is possible to pause/reset)
	bool changeable = true;
	//Value of how faded the black fade-in is
	float deathProgress = 0;
	float transitionProgress = 0;
	float respawnProgress = 0;
	//Keeps track on if the player died
	public bool playerDied = false;
	public bool transitioning = false;
	public bool fadeIn = true;
	float timePassed = 0;
	//Keeps track of how long the reset button has been held down for
	float resetHeld = 0f;
	Level nextLevel;
	public bool endScreen;

	Panel ResetProgress;
	CenterContainer PauseCenterContainer;
	public Panel DeathPanel;
	public Panel TransitionPanel;
	Button RespawnButton;
	Button RestartButton;
	public override void _Ready()
	{
		//Connects the ReloadCurrentLevel Signal to Main, causing Main's ReloadCurrentLevel method to run when called
		Connect(nameof(ReloadCurrentLevel), GetNode("/root/Main"), "ReloadCurrentLevel");
		Connect(nameof(ChangeLevel), GetNode("/root/Main"), "NextLevel");

		//Adds reference to TransitionPanel Panel
		TransitionPanel = GetNode<Panel>("TransitionPanel");
		//Adds reference to ResetProgress Panel
		ResetProgress = GetNode<Panel>("ResetProgress");
		//Adds reference to CenterContainer PauseCenterContainer
		PauseCenterContainer = GetNode<CenterContainer>("PauseCenterContainer");
		//Adds reference to DeathPanel Panel
		DeathPanel = GetNode<Panel>("DeathPanel");
		//Adds reference to RespawnButton Button
		RespawnButton = GetNode<Button>("DeathPanel/RespawnButton");
		//Adds reference to RestartButton button
		RestartButton = GetNode<Button>("PauseCenterContainer/VBoxContainer/RestartButton");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		timePassed += delta;
		//Checks if Player can unpause game
		if (changeable)
		{
			// if pause was just pressed once
			if (Input.IsActionJustPressed("pause"))
			{
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
				if (resetHeld > ResetLength)
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
		//If player cannot unpause the game and the player is in the deathscreen
		else if (playerDied)
		{
			//Fades to black
			if(deathProgress < DeathLength)
			{
				deathProgress += delta;
				DeathPanel.Modulate = new Color(1, 1, 1, deathProgress * (1 / DeathLength));
			}
			//Onced faded to black, fades in a button to respawn
			else if (respawnProgress < RespawnLength)
			{
				respawnProgress += delta;
				RespawnButton.Modulate = new Color(1, 1, 1, respawnProgress * (1 / RespawnLength));
			}
		}
		else if (transitioning)
		{
			transitionProgress += delta;
			if(fadeIn)
			{
				TransitionPanel.Modulate = new Color(1, 1, 1, transitionProgress * (1 / TransitionLength));
				if((transitionProgress * (1 / TransitionLength)) > 1)
				{
					fadeIn = false;
					if (!endScreen)
					{
						ChangeNextLevel();
						GetTree().Paused = false;
					}
					else
					{
						transitioning = false;
					}
				}
			}
			else
			{
				TransitionPanel.Modulate = new Color(1, 1, 1, 2 - transitionProgress * (1 / TransitionLength));
				if (2 - transitionProgress * (1 / TransitionLength) < 0)
				{
					if (!endScreen)
					{
						fadeIn = true;
						ResetVariables();
					}
					else
					{
						transitioning = false;
					}
				}
			}
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
	
	private void ResetVariables()
	{
		//Reset all PauseMenu variables
		DeathPanel.Visible = false;
		paused = false;
		changeable = true;
		outsidePaused = false;
		playerDied = false;
		transitioning = false;
		transitionProgress = 0;
		fadeIn = true;
		respawnProgress = 0;
		deathProgress = 0;
		DeathPanel.Modulate = new Color(1, 1, 1, 0);
		RespawnButton.Modulate = new Color(1, 1, 1, 0);
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
		if (RestartButton.Visible) {
			paused = false;
			Pause();
			//Emits the ReloadCurrentLevel signal, causing the ReloadCurrentLevel method to run in Main
			EmitSignal(nameof(ReloadCurrentLevel));
		}
	}

	private void _on_MenuButton_pressed() {
	// return to main menu button pressed, unpause game then change to main menu
		paused = false;
		Pause();
		GetTree().ChangeScene("res://Scenes/Menu.tscn");
	}

	private void _on_RespawnButton_pressed()
	{
		timePassed = 0;
		ResetVariables();
		//Unpause the game
		GetTree().Paused = paused;
		//Emits the ReloadCurrentLevel signal, causing the ReloadCurrentLevel method to run in Main
		EmitSignal(nameof(ReloadCurrentLevel));
	}

	public void PlayerDied()
	{
		if(timePassed > 0.5)
		{
			//When player dies make the game un-changable-from-the-player, pause the game, and set playerDied to true
			DeathPanel.Visible = true;
			playerDied = true;
			PauseFromOutsidePauseMenu(true);
		}
	}

	private void TransitionFade(Level nLevel)
	{
		nextLevel = nLevel;
		TransitionPanel.Visible = true;
		paused = true;
		changeable = false;
		GetTree().Paused = paused;
		transitioning = true;
	}

	public void PauseFromOutsidePauseMenu(bool outPause)
	{
		paused = outPause;
		changeable = !outPause;
		GetTree().Paused = paused;
	}
	
	private void ChangeNextLevel()
	{
		EmitSignal(nameof(ChangeLevel), nextLevel);
	}
}












