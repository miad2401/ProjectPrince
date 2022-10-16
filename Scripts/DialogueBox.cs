using Godot;
using System;
using System.IO;

public class DialogueBox : Control
{
	//Creates a signal to connect to PauseMenu
	[Signal] public delegate void PauseForDialogue(bool pause);
	[Signal] public delegate void DialogueBoxClosed();

	/*
	 * Export Fields
	 * slideInTime - The time it takes for the Note to slide into view in seconds
	 * slideOutTime - The time it takes for the Note to slide out of view in seconds
	 * 
	 * Constants
	 * positionOffset - The amount of pixels to 
	 * 
	 * Fields
	 * finalPosition - The relative position of the note when fully in view
	 * hiddenPosition - The relative position of the note when not in view
	 * NoteBackground - A reference to the NoteBackground node
	 * slideIn - Bool for if the note is sliding in
	 * slideOut - Bool for if the note is sliding out
	 * slideProgress - The current progress that the note has slid in/out
	 * noteActivated - Determines if the node has already been read
	 */


	[Export] bool startDialogue = true;
	// spaceTextDelay - amount of time in seconds in takes for "press space to continue" to pop up on given line	
	[Export] float spaceTextDelay = 4f;
	// spaceTextBlinkSpeed - amount of time between blinks for the press space to continue prompt
	[Export] float spaceTextBlinkSpeed = 0.75f;
	// scrollSpeedScale - controls how fast text scrolls by, lower is slower, higher is faster (>1 makes no difference)
	[Export] float scrollSpeedScale = 0.18f;
	
	// Used for scrolling text
	int drawTextSpeed = 0;
	int chatterLimit = 0;
	float scrollSpeedSum;
	
	// Used to track when to show "press space to continue" prompt
	float currentTextTime = 0;
	float spaceTextTime = 0;

	//File containing dialogue
	[Export] string textFilename;
	// Array of text lines
	String[] textList;
	// Array of who appears on left portrait
	[Export] String[] leftPortraits;
	// Array of who appears on right portrait
	[Export] String[] rightPortraits;
	
	// TODO SHAKE LINES
	
	
	[Export] float[] scrollSpeedScales;
	[Export] int[] lineShakes;
	
	
	// Total amount of time a shake takes
	[Export] float shakeTotalTime;
	// Amount of time between each shake movement
	[Export] float shakeBetweenTime;
	// How much each shake moves the text
	[Export] int currentShakeStrength = 3;
	// Stores how many shake movements there are
	int shakeMovements = 10;
	// Keeps track of how long the text has shaken
	float shakeTotalTimer = 0;
	// Keeps track of how long the current movement has happened
	float shakeBetweenTimer = 0f;
	// Tracks current movement
	int currentShakeMovement = 1;
	// Flag that tracks if currently shaking
	bool shaking = false;
	
	
	Container Left, Middle, Right;
	Label Text, SpaceText;
	CanvasLayer DBL;
	
	Sprite LeftPortrait, RightPortrait;
	
	
	Texture Silas, Malcolm, Maisie, Daniel;
	
	// Original position reference to undo text shaking
	Vector2 TextPosition;
	
	
	//Control PauseMenu;
	
	
	// Total amount of time a shake takes (not fully implemented - only affects the "WAKEUP" convo)
	/*float shakeTotalTime = 1.6f;
	// Amount of time between each shake movement
	float shakeBetweenTime;
	float defaultShakeBetweenTime = 0.065f;
	int currentShakeStrength;
	int defaultShakeStrength = 9;
	// Stores how many shake movements there are
	int shakeMovements = 10;
	// Keeps track of how long the text has shaken
	float shakeTotalTimer = 0;
	// Keeps track of how long the current movement has happened
	float shakeBetweenTimer = 0f;
	// Tracks current movement
	int currentShakeMovement = 1;
	// Flag that tracks if currently shaking
	bool shaking = false;*/
	
	int currentText = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		getTextFromFile(textFilename);
		this.Visible = startDialogue;
		Left = GetNode<Container>("DBL/Left");
		Right = GetNode<Container>("DBL/Right");
		Middle = GetNode<Container>("DBL/Middle");
		Text = GetNode<Label>("DBL/Text");
		SpaceText = GetNode<Label>("DBL/SpaceText");
		SpaceText.Visible = false;
		DBL = GetNode<CanvasLayer>("DBL");
		//TextPosition = Text.GetPosition();
		//Connects the PauseForDialogue signal to the PauseMenu
		Connect(nameof(PauseForDialogue), GetNode("/root/Main/GUI/PauseMenu"), "PauseFromOutsidePauseMenu");
		Connect(nameof(DialogueBoxClosed), GetNode("../Rival"), "DialogueBoxClosed");
		//PauseMenu = GetNode<Control>("/root/Main/GUI/PauseMenu");
		if (startDialogue) {
			EmitSignal(nameof(PauseForDialogue), true);
		}
		Text.Text = textList[currentText];
		chatterLimit = textList[currentText].Length;
		TextPosition = Text.GetPosition();
		
		shaking = lineShakes[currentText] > 0;
		
		
		// Load portraits
		Silas = ResourceLoader.Load("res://Art/Portraits/silas.png") as Texture;
		Daniel = ResourceLoader.Load("res://Art/Portraits/daniel.png") as Texture;
		Malcolm = ResourceLoader.Load("res://Art/Portraits/malcolm.png") as Texture;
		Maisie = ResourceLoader.Load("res://Art/Portraits/maisie.png") as Texture;
		LeftPortrait = GetNode<Sprite>("DBL/Left/Portrait/Sprite");
		RightPortrait = GetNode<Sprite>("DBL/Right/Portrait/Sprite");
		
		
		// Starting left portrait
		if (leftPortraits[currentText] != "" && leftPortraits[currentText] != "hidden") {
			if (leftPortraits[currentText] == "Silas"){
				LeftPortrait.Texture = Silas;
			}
			else if (leftPortraits[currentText] == "Daniel") {
				LeftPortrait.Texture = Daniel;
			}
			else if (leftPortraits[currentText] == "Malcolm") {
				LeftPortrait.Texture = Malcolm;
			}
			else {
				LeftPortrait.Texture = Maisie;
			}
		}
		else if (leftPortraits[currentText] == "hidden"){
			LeftPortrait.Visible = false;
		}
		else {
			Left.Visible = false;
		}
		// Starting right portrait
		if (rightPortraits[currentText] != "" && rightPortraits[currentText] != "hidden") {
			if (rightPortraits[currentText] == "Silas"){
				RightPortrait.Texture = Silas;
			}
			else if (rightPortraits[currentText] == "Daniel") {
				RightPortrait.Texture = Daniel;
			}
			else if (rightPortraits[currentText] == "Malcolm") {
				RightPortrait.Texture = Malcolm;
			}
			else {
				RightPortrait.Texture = Maisie;
			}
		}
		else if (rightPortraits[currentText] == "hidden"){
			RightPortrait.Visible = false;
		}
		else {
			Right.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		if (DBL.Visible) {
			if (Input.IsActionJustPressed("attack_sword")) {
				// If not at end of dialogue
				if (currentText != (textList.Length-1)) {
					// Stop any possible shakes
					stopShake();
					// Hide text
					Text.Visible = false;
					// Reset text position
					Text.SetPosition(TextPosition);
					// Advance text var
					currentText++;
					// Reset space prompt timer
					currentTextTime = 0;
					// Reset scroller
					drawTextSpeed = 0;
					Text.VisibleCharacters = 0;
					// Hide space prompt
					SpaceText.Visible = false;
					// Reset scroller
					chatterLimit = textList[currentText].Length;
					// Ready text
					Text.Text = textList[currentText];
					// Make text visible
					Text.Visible = true;
					shaking = lineShakes[currentText] > 0;
				}
				// End of dialogue, play game
				else {
					//EndDialogue();
					//EmitSignal(nameof(PauseForDialogue), false);
					//DBL.Visible = false;
					EndDialogue();
				}
				// check for portrait, if so load
				if (leftPortraits[currentText] != "" && leftPortraits[currentText] != "hidden") {
					if (leftPortraits[currentText] == "Silas"){
						LeftPortrait.Texture = Silas;
					}
					else if (leftPortraits[currentText] == "Daniel") {
						LeftPortrait.Texture = Daniel;
					}
					else if (leftPortraits[currentText] == "Malcolm") {
						LeftPortrait.Texture = Malcolm;
					}
					else {
						LeftPortrait.Texture = Maisie;
					}
					Left.Visible = true;
					LeftPortrait.Visible = true;
				}
				else if (leftPortraits[currentText] == "hidden"){
					Left.Visible = true;
					LeftPortrait.Visible = false;
				}
				else {
					Left.Visible = false;
				}
				
				// Right portraits
				if (rightPortraits[currentText] != "" && rightPortraits[currentText] != "hidden") {
					if (rightPortraits[currentText] == "Silas"){
						RightPortrait.Texture = Silas;
					}
					else if (rightPortraits[currentText] == "Daniel") {
						RightPortrait.Texture = Daniel;
					}
					else if (rightPortraits[currentText] == "Malcolm") {
						RightPortrait.Texture = Malcolm;
					}
					else {
						RightPortrait.Texture = Maisie;
					}
					Right.Visible = true;
					RightPortrait.Visible = true;
				}
				else if (rightPortraits[currentText] == "hidden"){
					Right.Visible = true;
					RightPortrait.Visible = false;
				}
				else {
					Right.Visible = false;
				}
			}
			// If esc is pressed, close dialogue
			else if (Input.IsActionPressed("pause")) {
				//EmitSignal(nameof(PauseForDialogue), false);
				//DBL.Visible = false;
				EndDialogue();
			}
			// Process
			else {
				// If time has elapsed to show space prompt
				if (currentTextTime > spaceTextDelay) {
					// Blinking space prompt
					if (spaceTextTime > spaceTextBlinkSpeed) {
						spaceTextTime = 0;
						SpaceText.Visible = !SpaceText.Visible;
					}
					else {
						spaceTextTime += delta;
					}
				}
				else {
					currentTextTime += delta;
				}
			}
			
			// Scroll text
			scrollSpeedSum += (delta*(scrollSpeedScales[currentText] > 0 ? scrollSpeedScales[currentText] : scrollSpeedScale));
			if (scrollSpeedSum > delta) {
				scrollSpeedSum = 0;
				showChatter();
			}
			
			// Shake text if applicable
			if (shaking) {
				shakeTotalTimer += delta;
				if (shakeTotalTimer < shakeTotalTime) {
					shakeBetweenTimer += delta;
					if (shakeBetweenTimer > shakeBetweenTime) {
						if (currentShakeMovement == 1 || currentShakeMovement == 5 || currentShakeMovement == 8){
							Text.SetPosition(new Vector2(Text.GetPosition().x+currentShakeStrength,Text.GetPosition().y));
						}
						else if (currentShakeMovement == 2 || currentShakeMovement == 4){
							Text.SetPosition(new Vector2(Text.GetPosition().x,Text.GetPosition().y+currentShakeStrength));
						}
						else if (currentShakeMovement == 3 || currentShakeMovement == 6 || currentShakeMovement == 10) {
							Text.SetPosition(new Vector2(Text.GetPosition().x-currentShakeStrength,Text.GetPosition().y));
						}
						else {
							Text.SetPosition(new Vector2(Text.GetPosition().x,Text.GetPosition().y-currentShakeStrength));
						}
						currentShakeMovement++;
						if (currentShakeMovement > 10) {
							currentShakeMovement = 0;
						}
						shakeBetweenTimer = 0;
					}
				}
				else {
					stopShake();
				}
			}
		}
	}
	
	// Scroll function
	private void showChatter() {
		if (drawTextSpeed < chatterLimit) {
			drawTextSpeed++;
			Text.VisibleCharacters = drawTextSpeed;
		}
	}
	
	private void stopShake() {
		shakeTotalTimer = 0;
		shakeBetweenTimer = 0;
		currentShakeMovement = 1;
		shaking = false;
	}
	
	public void EndDialogue() {
		DBL.Visible = false;
		if (!startDialogue)
		{
			//GetNode<Rival>("../Rival").DialougeBoxClosed();
			EmitSignal(nameof(DialogueBoxClosed));
		}
		else
		{
			EmitSignal(nameof(PauseForDialogue), false);
		}
	}

	public void getTextFromFile(string filename)
	{
		var rawFile = System.IO.File.ReadAllLines(filename);

		foreach (var line in rawFile)
		{
			var data = line.Split("/n");
			textList = data;
		}
	}
}
