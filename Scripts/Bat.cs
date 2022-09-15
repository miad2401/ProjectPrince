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

    [Export] private int amplitude;
    [Export] private int vSpeed;
    [Export] private int hSpeed;

    /*
     * Inherited Fields
     * bool hurtPlayer - If true, kills player when touched
     * Direction direction - The Direction the enemy last moved towards
     * Vector2 velocity - The x and y movement of the bat
     * Vector2 floor - Shows where the floor is, used for MoveAndSlide()
     */

    public override void MoveEnemy(float delta)
    {
        velocity.x = 1;

        MoveAndSlide(velocity, floor, false, 4, 0.785398f, false);
    }
}
