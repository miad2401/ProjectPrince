using Godot;
using System;

public class BossBat : Bat
{
    private int iHSpeed;
    private int hDrag;
    private int vSpeed;
    private Vector2 velocity;

    public override void _Ready()
    {
        base._Ready();
        velocity.x = iHSpeed;
    }

    public override void _PhysicsProcess(float delta)
    {
        CheckForCollision();
        MoveEnemy(delta);
    }

    public override void MoveEnemy(float delta)
    {
        if(velocity.x > 0)
        {
            velocity.x -= hDrag;
        }
        else if(velocity.x < 0)
        {
            velocity.x += hDrag;
        }
        velocity.y += vSpeed;
        MoveAndSlide(velocity);
    }

    //Setters
    public void SetIHSpeed(int ihs)
    {
        iHSpeed = ihs;
    }
    public void SetDrag(int hd)
    {
        hDrag = hd;
    }
    public void SetVSpeed(int vs)
    {
        vSpeed = vs;
    }
}
