using Godot;
using System;

public class Note : Control
{
    //Creates a signal to connect to PauseMenu
    [Signal] public delegate void PauseForNote(bool pause);

    /*
     * Export Fields
     * slideInTime - The time it takes for the Note to slide into view in seconds
     * slideOutTime - The time it takes for the Note to slide out of view in seconds
     * 
     * Constants
     * positionOffset - The amount of pixels to 
     * 
     * Fields
     * finalPosition - The relative position of the note when fully in view
     * hiddenPosition - The relative position of the note when not in view
     * NoteBackground - A reference to the NoteBackground node
     * slideIn - Bool for if the note is sliding in
     * slideOut - Bool for if the note is sliding out
     * slideProgress - The current progress that the note has slid in/out
     */
    [Export] float slideInTime;
    [Export] float slideOutTime;
    [Export] bool ExitHint;

    const int positionOffset = 254;

    float finalPosition;
    float hiddenPosition;
    TextureRect NoteBackground;
    bool slideIn = false;
    bool slideOut = false;
    float slideProgress = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Connects the PauseForNote signal to the PauseMenu
        Connect(nameof(PauseForNote), GetNode("/root/Main/GUI/PauseMenu"), "PauseFromOutsidePauseMenu");

        //Creates the reference for the NoteBackground and makes it invisible
        NoteBackground = GetNode<TextureRect>("Note/NoteBackground");
        NoteBackground.Visible = false;

        //If ExitHint is enabled, makes it visible
        if (ExitHint)
        {
            GetNode<Panel>("Note/NoteBackground/TextureButton/ExitHint").Visible = true;
        }

        //Sets the finalPosition to the current NoteBackground's y position
        finalPosition = NoteBackground.RectPosition.y;
        //Sets the hiddenPosition from the finalPosition and how far the offset is to make it off screen
        hiddenPosition = finalPosition + positionOffset;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //If the note is sliding in
        if (slideIn)
        {
            //Add progress to slideProgress
            slideProgress += delta;
            //The NoteBackground x position is untouched, but the y position changes
            //The y becomes a value between hidden and finalPosition based on slideProgress and the speed of the note's movement
            NoteBackground.RectPosition = new Vector2(NoteBackground.RectPosition.x, hiddenPosition - slideProgress*((hiddenPosition-finalPosition)/slideInTime));
            //If the Note has reached the center of the screen
            if(NoteBackground.RectPosition.y <= finalPosition)
            {
                //Stop sliding in, reset the slide value, and hide the note sprite
                slideIn = false;
                slideProgress = 0;
                GetNode<Sprite>("Area2D/Sprite").Visible = false;
            }
        }
        else if (slideOut)
        {
            //Add progress to slideProgress
            slideProgress += delta;
            //The NoteBackground x position is untouched, but the y position changes
            //The y becomes a value between hidden and finalPosition based on slideProgress and the speed of the note's movement
            NoteBackground.RectPosition = new Vector2(NoteBackground.RectPosition.x, finalPosition + slideProgress * ((hiddenPosition - finalPosition) / slideOutTime));
            //If the Note has left the screen
            if (NoteBackground.RectPosition.y > hiddenPosition)
            {
                //Stop sliding out
                slideOut = false;
                //Queuefree as the note will not be used again
                QueueFree();
            }
        }
    }

    //Called when the area is entered
    public void OnNoteBodyEntered(Node body)
    {
        //Checks if the touched object was a player
        if (body.IsInGroup("Player"))
        {
            //Send a Signal to PauseMenu to pause the game
            EmitSignal(nameof(PauseForNote), true);
            //Move the NoteBackgroudn to be juuuust off-screen then make it visible
            NoteBackground.RectPosition = new Vector2(NoteBackground.RectPosition.x, hiddenPosition);
            NoteBackground.Visible = true;
            //Start sliding in the note to the center of the screen
            slideIn = true;
        }
    }
    public void OnExitNotePressed()
    {
        slideOut = true;
        //Tells PauseMenu to unpause game
        EmitSignal(nameof(PauseForNote), false);
    }
}
