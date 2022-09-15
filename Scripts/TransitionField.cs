using Godot;
using System;

public class TransitionField : Area2D
{
    //Change values in editor, not in code
    [Export] Level nextLevel;

    //Signal that tells Main to change the current level to the given level
    [Signal] public delegate void TransitionAnimation(Level nLevel);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Connects signal that tells Main to change the current level to the given level
        Connect(nameof(TransitionAnimation), GetNode("/root/Main/GUI/PauseMenu"), "TransitionFade");
    }

    private void OnTransitionFieldBodyEntered(Node body)
    {
        //Checks if the touched object was a player, if so, sends the LevelTransition signal
        if (body.IsInGroup("Player"))
        {
            EmitSignal(nameof(TransitionAnimation), nextLevel);
        }
    }
}
