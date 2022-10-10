using Godot;
using System;

public class CheckpointField : Area2D
{
    //Change values in editor, not in code
    [Export] int nextCheckpoint;
    [Export] bool activatesMagicJump;
    private void OnCheckpointFieldBodyEntered(Node body)
    {
        //Checks if the touched object was a player, if so, update checkpoint
        if (body.IsInGroup("Player"))
        {
            if(Main.checkpoint < nextCheckpoint){
                Main.checkpoint = nextCheckpoint;
            }

            if(activatesMagicJump){
                Player thePlayer;
                if (GetParent().Name.Contains("Level"))
                {
                    thePlayer = GetNode<Player>("../Player");
                }
                else
                {
                    thePlayer = GetNode<Player>("../../Player");
                }
                thePlayer.SetPlayerAbility(true,false, 2);
            }
        }
    }
}
