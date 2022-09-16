using Godot;
using System;

public class PlayerAttack : RigidBody2D
{
	// magicSpeed - Speed at which a magic bolt moves
	// swordSpeed - Speed at which a sword swing moves (much faster)
	// swordRange - Range of a sword attack
	[Export] int magicSpeed;
	[Export] int swordSpeed;
	[Export] int swordRange;
	
	// Copy of the attacks starting x coordinate
	float startPosition;
	// Flag that determines whether or not the attack is a sword swing
	bool sword = false;
	// Var that holds whether the attack is going at the speed of a magic attack or a sword attack
	int activeSpeed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Copy the attack's original x coordinate
		startPosition = GlobalPosition.x;
		// Store sprite in var
		Sprite attackSprite = GetNode<Sprite>("AttackSprite");
		// If the sprite is visible, it is a magic attack
		if (attackSprite.Visible) {
			// Set active speed to magic attack speed
			activeSpeed = magicSpeed;
		} 
		// If the attack sprite is not visible (visibility is set in Attack() of Player.cs), 
		// it is a sword attack
		else {
			// Set active speed to sword attack speed
			activeSpeed = swordSpeed;
			sword = true;
			attackSprite.Visible = true; // TMP --- just to show projectile
		}
		//Uses its CollisionPolygon to determine what direction it should be headed
		// change sprite orientation based on player dir
		if (GetNode<CollisionPolygon2D>("CollisionPoly").RotationDegrees != 0) {
			//If facing left, changes the speed of the projectile so it will move left
			activeSpeed *= -1;
			if (attackSprite.Visible) {
				attackSprite.Scale = new Vector2(-1,1);
			}
		}
	}

	//Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
	public override void _PhysicsProcess(float delta)
	{
		//Every frame, moves the projectile according to its active speed
		GlobalPosition = new Vector2(GlobalPosition.x + activeSpeed, GlobalPosition.y);
		// If attack is a sword attack
		if (sword) {
			// If the sword projectile has gone past its range
			// Dividing activeSpeed by swordSpeed returns a factor of 1 or -1, which tells which direction
			// to apply the swordRange to the x position
			if (activeSpeed/swordSpeed > 0) {
				if (GlobalPosition.x >= (startPosition + ((activeSpeed/swordSpeed) * swordRange))) {
					// Delete
					QueueFree();
				}
			}
			else {
				if (GlobalPosition.x <= (startPosition + ((activeSpeed/swordSpeed) * swordRange))) {
					// Delete
					QueueFree();
				}
			}
		}
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
