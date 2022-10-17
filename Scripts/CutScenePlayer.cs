using Godot;
using System;

public enum cutSceneName
{
    Drain,
    Ending
}

public class CutScenePlayer : Area2D
{
    [Export] cutSceneName cutScene;
    [Export] Level nextLevel;

    [Signal] public delegate void ActivateVideoPlayer(String streamPath);
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect(nameof(ActivateVideoPlayer), GetNode("/root/Main/GUI/VideoPlayer"), "ShowVideoPlayer");
    }

    private void onBodyEnterField(Node node)
    {
        if(node.IsInGroup("Player"))
        {
            this.SetDeferred("monitoring", false);
            EmitSignal(nameof(ActivateVideoPlayer), cutScene);
        }
    }
}
