using Godot;
using System;

public class AcquireAbility : Control
{
    [Export] Ability newAbility;

    Sprite PickupsSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PickupsSprite = GetNode<Sprite>("PickupsSprite");
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
        GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation = newAbility.ToString() + "Acquired";
        Player thePlayer = GetNode<Player>("../Player");
        switch (newAbility)
        {
            case Ability.Sword:
                thePlayer.SetPlayerAbility(true,false, 0);
                break;
            case Ability.Potion:
                thePlayer.SetPlayerAbility(true,false, 1);
                break;
            case Ability.Book:
                thePlayer.SetPlayerAbility(true,false, 2);
                break;
            default:
                GD.Print("AcquireAbility failed to Equip");
                break;
        }
    }
}
