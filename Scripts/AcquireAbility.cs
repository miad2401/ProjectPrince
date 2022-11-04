using Godot;
using System;

public class AcquireAbility : Control
{
	[Signal] public delegate void SetPlayerAbility(bool add, bool clearFirst, int abilityID);

	[Export] private Ability newAbility;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite PickupsSprite = GetNode<Sprite>("PickupsSprite");
		//Frame 1 is The Sword
		//Frame 2 is the Potion
		//Frame 3 is the Book
		//Defaults to sword
		switch (newAbility)
		{
			case Ability.Sword:
				PickupsSprite.Frame = 0;
				break;
			case Ability.Potion:
				PickupsSprite.Frame = 1;
				break;
			case Ability.Book: 
				PickupsSprite.Frame = 2;
				break;
			default:
				PickupsSprite.Frame = 0;
				break;
		}
	}

	public void OnPickupAreaEntered(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation = newAbility.ToString() + "Acquired";
			//Finds the player and connects the signal
			if (GetParent().Name.Contains("Level") || GetParent().Name.Contains("Test"))
			{
				Connect(nameof(SetPlayerAbility), GetNode("../Player"), "SetPlayerAbility");
			}
			else
			{
				Connect(nameof(SetPlayerAbility), GetNode("../../Player"), "SetPlayerAbility");
			}
			switch (newAbility)
			{
				case Ability.Sword:
					EmitSignal(nameof(SetPlayerAbility), true, false, 0);
					break;
				case Ability.Potion:
					EmitSignal(nameof(SetPlayerAbility), true, false, 1);
					break;
				case Ability.Book:
					EmitSignal(nameof(SetPlayerAbility), true, false, 2);
					break;
				case Ability.NoAbilities:
					EmitSignal(nameof(SetPlayerAbility), true, false, 3);
					break;
				case Ability.EveryAbility:
					EmitSignal(nameof(SetPlayerAbility), true, false, 4);
					break;
				default:
					GD.Print("AcquireAbility failed to Equip");
					break;
			}
			QueueFree();
		}
	}
}
