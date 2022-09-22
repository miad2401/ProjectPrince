using Godot;
using System;

public class EnemyBarrier : StaticBody2D
{
	//Change values in editor, not in code
	[Export] bool detectBat;
	[Export] bool detectKnight;
	[Export] bool detectRival;
	[Export] bool detectPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//CollisionLayers are specified as a bitmask
		//For Decimal - Add the results of 2 to the power of (layer to be enabled - 1)
		//For example, to enable layer 3, we would have to add 2^(3-1) (4) to the bitmask

		//Clears the CollisionLayer
		CollisionLayer = 0;
		//This series of if-statements is used to enable each specified enemy layer
		//Layer 3 is Bats
		//Layer 4 is Knights
		//Layer 5 is Rival
		//Layer 16 is Player
		if (detectBat)
		{
			CollisionLayer += (uint)Math.Pow(2, 2);
		}
		if (detectKnight)
		{
			CollisionLayer += (uint)Math.Pow(2, 3);
		}
		if (detectRival)
		{
			CollisionLayer += (uint)Math.Pow(2, 4);
		}
		if (detectPlayer)
		{
			CollisionLayer += (uint)Math.Pow(2, 15);
		}
	}
}
