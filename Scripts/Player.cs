using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export] int speed = 80;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    //Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
    public override void _PhysicsProcess(float delta)
    {
        //velocity is the Velocity of the player. It is a Vector2, meaning it contains 2 variables, x and y. 
        Vector2 velocity = new Vector2();
        //Checks if the right arrowkey is pressed, and if so, sets the x portion of the velocity to 1
        if (Input.IsActionPressed("ui_right"))
        {
            velocity.x += 1.0f;
        }
        //Checks if the left arrowkey is pressed, and if so, sets the x portion of the velocity to -1
        if (Input.IsActionPressed("ui_left"))
        {
            velocity.x -= 1.0f;
        }
        //If no tracked key is pressed, the x and y components return to 0 

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
            //Moves the character based on velocity and speed
            //Speed is arbitrary and can be changed within the Godot editor
            MoveAndSlide(velocity * speed);
        }
    }
}
