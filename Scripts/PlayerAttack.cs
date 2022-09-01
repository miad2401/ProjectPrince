using Godot;
using System;

public class PlayerAttack : RigidBody2D
{
	//speed is the speed at which the projectile moves
	[Export] int speed = 3;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite attackSprite = GetNode<Sprite>("AttackSprite");
		//Uses its CollisionPolygon to determine what direction it should be headed
		// change sprite orientation based on player dir
		if (GetNode<CollisionPolygon2D>("CollisionPoly").RotationDegrees != 0)
		{
			//If facing left, changes the speed of the projectile so it will move left
			speed = -speed;
			attackSprite.Scale = new Vector2(-1,1);
		}
		else {
			attackSprite.Scale = new Vector2(1,1);
		}
	}

	//Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
	public override void _PhysicsProcess(float delta)
	{
		//Every frame, moves the particle according to its speed
		GlobalPosition = new Vector2(GlobalPosition.x + speed, GlobalPosition.y);
	}

	//If the PlayerAttack touches any walls or enemies, this method is called
	//Node body is the object this collided with
	public void _on_PlayerAttack_body_entered(Node body)
	{
		//Checks if the touched object was an enemy, if so, deletes it
		if (body.IsInGroup("Enemy"))
		{
			body.QueueFree();
		}
		//Deletes itself
		QueueFree();
	}
}
