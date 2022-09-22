using Godot;
using System;

public class BossKnight : Knight
{
    [Export] private int hSpeed;
    [Export] private int maxVSpeed;
    [Export] private int gravity;
    [Export] private Direction direction;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        SetHSpeed(hSpeed);
        SetMaxVSpeed(maxVSpeed);
        SetGravity(gravity);
        SetDirection(direction);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

    }
}
