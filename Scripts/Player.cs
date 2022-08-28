using Godot;
using System;

public class Player : KinematicBody2D
{
    /*
     * All of these varaibles can be changed within the editor, but are not changed within the script
     * 
     * xAcceleration - Changes how fast the player accelerates left/right
     * maxHSpeed - Determines the maximum speed the player can go left/right
     * maxVSpeed - Determines the minimum speed the player can go up/down
     * jumpPower - Determines how high the player goes when they jump
     * gravity - pulls the player down at a constant rate
     */
    [Export] int xAcceleration = 10;
    [Export] int maxHSpeed = 500;
    [Export] int maxVSpeed = 500;
    [Export] int jumpPower = 300;
    [Export] int gravity = 10;
    //velocity is the Velocity of the player. It is a Vector2, meaning it contains 2 variables, x and y. 
    Vector2 velocity = new Vector2();
    //Used to determine what direction is up/down/left/right
    Vector2 floor = new Vector2(0,-1);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    //Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
    public override void _PhysicsProcess(float delta)
    {
        //Apply the force of gravity
        velocity.y += gravity;
        //If ran into wall, stops the player
        if (IsOnWall())
        {
            velocity.x = 0;
        }
        //Checks if the right arrowkey is pressed, and if so, sets the x portion of the velocity to 1
        if (Input.IsActionPressed("ui_right"))
        {
            velocity.x += xAcceleration;
        }
        //Checks if the left arrowkey is pressed, and if so, sets the x portion of the velocity to -1
        else if (Input.IsActionPressed("ui_left"))
        {
            velocity.x -= xAcceleration;
        }
        //If neither left/right key has been pressed, slows down the character 2x as fast as the player accelerates
        else
        {
            if(velocity.x > 0)
            {
                velocity.x -= xAcceleration * 2;
                if(velocity.x < 0)
                {
                    velocity.x = 0;
                }
            }
            else
            {
                velocity.x += xAcceleration * 2;
                if (velocity.x > 0)
                {
                    velocity.x = 0;
                }
            }
        }

        //Checks if the Player is on the Floor
        if (IsOnFloor())
        {
            //If the Player is on the floor and pressing the jump key, lets the player jump
            if (Input.IsActionPressed("ui_jump"))
            {
                velocity.y -= jumpPower;
            }
            //Else, stops vertical movement entirely
            else
            {
                velocity.y = 0;
            }
        }

        //Finds the child AnimationTree node and sets a references to it to a AnimationTree variable
        AnimationTree animationTree = GetNode<AnimationTree>("AnimationTree");
        //Finds the AnimationNodeStateMachinePlayback resource within the animationTree and sets it to its own variable
        //Because Godot doesn't allow arguments in the .Get() function, we also must cast it as a AnimationNodeStateMachinePlayback
        AnimationNodeStateMachinePlayback myANSMP = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
        if (velocity == Vector2.Zero)
        {
            //If no key is being pressed, switches to an idle animation
            myANSMP.Travel("Idle");
        }
        else
        {
            //If a key is being pressed, switches to an walk animation
            myANSMP.Travel("Walk");
            //Changes direction of the animation based on the velocity
            animationTree.Set("parameters/Idle/blend_position", velocity);
            animationTree.Set("parameters/Walk/blend_position", velocity);

            //Velocity is limited my the maxHSpeed and the maxVSpeed
            //These values can be changed within the editor
            velocity.x = Math.Min(velocity.x, maxHSpeed);
            velocity.x = Math.Max(velocity.x, -maxHSpeed);
            velocity.y = Math.Min(velocity.y, maxVSpeed);
            velocity.y = Math.Max(velocity.y, -maxVSpeed);
            //Moves the player according to the velocity and defines what direction to go
            MoveAndSlide(velocity, floor);
        }
    }
}
