using Godot;
using System;

public class PopUpHint : Control
{
	//Change values in editor, not in code
	//If True, displays an addiction textbox
	[Export] bool leftText;
	//Time to display Textboxes
	[Export] float fadelength; //In Seconds

	//Currently fading out/in
	bool fadeIn = false;
	//Value of how faded the textbox are
	float fadeProgress = 0;
	//HintContainer is the node that is faded in/out
	//(all children are also faded)
	Control HintContainer;

	public override void _Ready()
	{
		//Stores reference to the HintContainer
		HintContainer = GetNode<Control>("HintContainer");
		//If leftText is enabled, displays it, otherwise hides it
		if (leftText)
        {
			GetNode<MarginContainer>("HintContainer/HintContainerLeft").Visible = true;
		}
        else
        {
			GetNode<MarginContainer>("HintContainer/HintContainerLeft").Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
    {
		//Checks if the hint is visible and that it isn't already fully visible
        if (fadeIn && fadeProgress < fadelength)
        {
			fadeProgress += delta;
        }
		//Checks if the hint isn't visible and that it isn't already fully invisible
        else if(!fadeIn && fadeProgress > 0)
        {
			fadeProgress -= delta;
        }
		//If neither of these, returns so that we don't modulate no change.
        else
        {
			return;
        }
		//Changes the fade on the HintContainer
		HintContainer.Modulate = new Color(1, 1, 1, fadeProgress * (1 / fadelength));
	}

	//Called when body entered
	public void OnHintArea2DBodyEntered(Node body)
	{
		//Checks if the touched object was a player, and if so, displays the text
		if (body.IsInGroup("Player"))
		{
			fadeIn = true;
		}
	}

	//Called when body exited
	public void OnHintArea2DBodyExited(Node body)
    {
		//Checks if the touched object was a player, and if so, Hides the text
		if (body.IsInGroup("Player"))
		{
			fadeIn = false;
		}
	}
}
