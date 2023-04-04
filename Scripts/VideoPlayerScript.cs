using Godot;
using System;

public class VideoPlayerScript : VideoPlayer
{
    private Level NextLevel;
    [Signal] public delegate void TransitionAnimation(Level nLevel);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect(nameof(TransitionAnimation), GetNode("/root/Main/GUI/PauseMenu"), "TransitionFade");
    }

    public void ShowVideoPlayer(cutSceneName animationName)
    {
        switch(animationName)
        {
            case cutSceneName.Drain:
                this.Stream = GD.Load<VideoStream>("res://Sounds/Video/Drain.ogv");
                NextLevel = Level.SewerLevel;
                break;
            case cutSceneName.Ending:
                this.Stream = GD.Load<VideoStream>("res://Sounds/Video/Drain.ogv");
                break;
        }
        
        this.Visible = true;
        GetTree().Paused = true;
        this.Play();
        LoadNextLevel();
    }

    public void LoadNextLevel(){
        switch(NextLevel)
        {
            case Level.SewerLevel:
                EmitSignal(nameof(TransitionAnimation), NextLevel);
                Main.checkpoint = 1;
                break;
            default:
                GetTree().ChangeScene("res://Scenes/Menu.tscn");
                break;
        }
    }

    public void _on_VideoPlayer_finished()
    {
        this.Visible = false;
    }
}
