using Godot;
using System;

public enum Ability
{
    Sword,
    Potion,
    Book
}
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

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void OnPickupAreaEntered(Node body)
    {
        GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation = newAbility.ToString() + "Acquired";
        /*
        switch (newAbility)
        {
            case Ability.Sword:
                Player.swordHoldEnabled = true;
                Player.swordEquipped = true;
                Player.magicEquipped = false;
                break;
            case Ability.Potion:
                Player.magicAttackEnabled = true;
                Player.magicEquipped = true;
                Player.swordEquipped = false;
                break;
            case Ability.Book:
                Player.magicJumpEnabled = true;
                break;
            default:
                GD.Print("AcquireAbility failed to Equip");
                break;
        }
        */
    }
}
