using Godot;
using System;

public class FakeNote : Note
{
    /*
     * Export Fields
     * numOfBats - The number of Bats to spawn
     * ampMax - The maximum amplitude a Bat can have
     * ampMin - The minimum amplitude a Bat can have
     * freqMax - The maximum frequency a Bat can have
     * freqMin - The minimum frequency a Bat can have
     * batHSpeed - The speed that a bat moves horizontally
     * 
     * tLBoundary - The top left boundary of where the bats can spawn
     * bRBoundary - The bottom right boundary of where the bats can spawn
     * batScene - 
     */
    [Export] private int numOfBats;
    [Export] private int ampMax;
    [Export] private int ampMin;
    [Export] private float freqMax;
    [Export] private float freqMin;
    [Export] private int batHSpeed;

    private Vector2 tLBoundary;
    private Vector2 bRBoundary;
    private PackedScene batScene;
    public override void _Ready()
    {
        base._Ready();
        tLBoundary = GetNode<Position2D>("TopLeftBoundary").GlobalPosition;
        bRBoundary = GetNode<Position2D>("BottomRightBoundary").GlobalPosition;
        batScene = GD.Load<PackedScene>("res://Scenes/Bat.tscn");
    }
    public override void OnExitNotePressed()
    {
        base.OnExitNotePressed();
        Random rnd = new Random();
        for (int i = 0; i < numOfBats; i++)
        {
            Bat instancedBat = batScene.Instance() as Bat;
            instancedBat.SetAmplitude(rnd.Next(ampMin, ampMax));
            instancedBat.SetFrequency((float)(rnd.NextDouble() * (freqMax - freqMin) + freqMin));
            instancedBat.SetHSpeed(batHSpeed);
            instancedBat.GlobalPosition = new Vector2(rnd.Next((int)tLBoundary.x,(int)bRBoundary.x), rnd.Next((int)tLBoundary.y, (int)bRBoundary.y));
            instancedBat.CollisionMask = (uint)(Math.Pow(2, 0) + Math.Pow(2, 2) + Math.Pow(2, 15));
            GetParent().AddChild(instancedBat);
        }
        
    }
}
