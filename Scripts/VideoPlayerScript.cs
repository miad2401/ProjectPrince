using Godot;
using System;

public class VideoPlayerScript : VideoPlayer
{
    private cutSceneName NextLevel;
    [Signal] public delegate void TransitionAnimation(Level nLevel);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect(nameof(TransitionAnimation), GetNode("/root/Main/GUI/PauseMenu"), "TransitionFade");
    }

    public void ShowVideoPlayer(cutSceneName nextLevel)
    {
        switch(nextLevel)
        {
            case cutSceneName.Drain:
                this.Stream = GD.Load<VideoStream>("res://Sounds/Video/Drain.ogv");
                break;
            case cutSceneName.Ending:
                this.Stream = GD.Load<VideoStream>("res://Sounds/Video/Drain.ogv");
                break;
        }
        
        this.Visible = true;
        GetTree().Paused = true;
        this.Play();

        NextLevel = nextLevel;
    }

    public void _on_VideoPlayer_finished()
    {
        this.Visible = false;
        switch(NextLevel)
        {
            case cutSceneName.Drain:
                EmitSignal(nameof(TransitionAnimation), Level.Level3);
                Main.checkpoint = 1;
                break;
            case cutSceneName.Ending:
                GetTree().ChangeScene("res://Scenes/Menu.tscn");
                break;
        }
    }
}
