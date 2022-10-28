using Godot;
using System;

public class SettingsMenu : Control
{
    AudioStreamPlayer musicPlayer;
    Control pauseMenu;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Creates reference to main music player
        musicPlayer = GetNode<AudioStreamPlayer>("../../Music");
        //Creates reference to pause menu
        pauseMenu = GetNode<Control>("../PauseMenu");

        //Sets Default Music VolumeDb
        musicPlayer.VolumeDb = ((float)(GetNode<HSlider>("CenterContainer/VBoxContainer/HSlider").Value));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //Show Main Pause Menu Node which was hidden on Setthing Button Press
        if (Input.IsActionJustPressed("pause"))
		{
			pauseMenu.Show();
	    }
    }

    private void _on_HSlider_value_changed(float value){
        // updates the music volume
        musicPlayer.VolumeDb = value;
    }
}
