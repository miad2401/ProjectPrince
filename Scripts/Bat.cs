using Godot;
using System;

public class Bat : BaseEnemy
{
    /* 
     * Fields
     * int amplitude - The Max/Min height the bat will move around its starting point
     * int vSpeed - The speed at which the bat moves vertically
     * int hSpeed - The speed at which the bat moves horizontally
     */

    [Export] protected int amplitude;
    [Export] protected float frequency;
    [Export] protected int hSpeed;
    protected float timePassed;

    /*
     * Inherited Fields
     * bool hurtPlayer - If true, kills player when touched
     * Direction direction - The Direction the enemy last moved towards
     * Vector2 velocity - The x and y movement of the bat
     * Vector2 floor - Shows where the floor is, used for MoveAndSlide()
     */

    public override void _Ready()
    {
        base._Ready();
        if (direction == Direction.Left)
        {
            hSpeed = -hSpeed;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        //Method to move the enemy
        MoveEnemy(delta);
    }

    public override void MoveEnemy(float delta)
    {
        timePassed += delta;
        CheckForCollision();
        if (IsOnWall())
        {
            if(direction == Direction.Left) 
            { 
                direction = Direction.Right;
            }
            else 
            { 
                direction = Direction.Left;
                hSpeed = -hSpeed;
            }
        }
        velocity.y = (float)(amplitude *Math.Cos(2*Math.PI*frequency*timePassed));
        velocity.x = hSpeed;

        MoveAndSlide(velocity, floor, false, 4, 0.785398f, false);
    }

    public override void AnimateEnemy()
    {
        throw new NotImplementedException();
    }

    //Setters
    public void SetAmplitude(int a)
    {
        amplitude = a;
    }
    public void SetFrequency(float f)
    {
        frequency = f;
    }
    public void SetHSpeed(int hS)
    {
        hSpeed = hS;
    }
}
