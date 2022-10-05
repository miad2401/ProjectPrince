using Godot;
using System;


//  @ Robert Battle, 9/23/2022, 2 a.m. the day of submission :) 
public class OpeningCutscene : Control
{
	// spaceTextDelay - amount of time in seconds in takes for "press space to continue" to pop up on given line	
	[Export] float spaceTextDelay;
	// spaceTextBlinkSpeed - amount of time between blinks for the press space to continue prompt
	[Export] float spaceTextBlinkSpeed;
	// scrollSpeedScale - controls how fast text scrolls by, lower is slower, higher is faster (>1 makes no difference)
	[Export] float scrollSpeedScale;
	
	// Used for scrolling text
	int drawTextSpeed = 0;
	int chatterLimit = 0;
	float scrollSpeedSum;
	
	// Used to track when to show "press space to continue" prompt
	float currentTextTime = 0;
	float spaceTextTime = 0;
	
	// Array of dialogue lines
	String[] textList = new String[46]{
		"???: ZZzzzzZZZZZ....",
		"???: \"Your Highness, wake up...\"",
		"???: ZZzzzzZZZZZ....",
		"???: \"Your Highness...\"",
		"???: ZZZZZZzzzzzZZZZ.....",
		"???: \"GET UP!!!\"",
		"Daniel: \"AHHHHhhhhh!!!-\"",
		"Daniel: \"oh.. Good morning, Silas.\"",
		"Silas: \"\'Good morning\', Daniel?! Do you know what time it is??\"",
		"Daniel: \"8 a.m.?\"",
		"Silas: \"Try 2 p.m.\"",
		"Daniel: \"How??? When?! How screwed am I?\"",
		"Silas: \"Your father doesn't know...\"",
		"Silas: \"Your mother, on the other hand...\"",
		"Daniel: \"Oh no...\"",
		"Silas: \"If you hurry, you can make it out before she arrives to chastise you.\"",
		"Daniel: \"Really?\"",
		"Silas: \"Yes, really. Now!\"",
		"Silas: \"MOVE!\"",
		"Daniel: \"YESSIR!\"",
		"l   The two enter the garden.   l",
		"l   They walk along a path, each side lined with flowers of all different colors and types,   l",
		"l  some even out of season yet still flourishing.  l",
		"Daniel: \"She berated me for an hour.\"",
		"Silas: \"This was the third time this week, and it's only Wednesday.\"",
		"Silas: \"This is why I keep telling you to stop reading so late into the night.\"",
		"Daniel: \"Aw, but I couldn\'t! Madam Anatasia was just about to declare her intention to divorce Duke Grayson for the spectacular Marquis Hannah...\"",
		"Silas: \"Fascinating. You\'ll just have to tell me all about it later, Your Highness. Now, if we could...\"",
		"Daniel: \"You could always join me, you know?\"",
		"Silas: \"...don\'t even suggest such a preposterous thing...\"",
		"Daniel: \"Oh, don\'t be like that. A romantic book deserves a...\"",
		"l   Daniel retrieves a flower, a yellow primrose, and presents it to Silas.   l",
		"Daniel: \"...romantic setting.\"",
		"Silas: \"Page 62, huh?\"",
		"Daniel: \"N,,,no, that was all me.\"",
		"Daniel: \"Wait, how did you know?\"",
		"Silas: \"...you really need to stop reading those things.\"",
		"Daniel: \"HA!\"",
		"l   Daniel runs away teasingly.   l",
		"Daniel: \"You\'ll have to catch me to stop me!\"",
		"l   Loud noises are heard nearby, the distinct sound of metal armor clattering echoes   l",
		"Daniel: \"What the,,!\"",
		"l   A large group of knights clamor in.   l",
		"???: \"There\'s the Prince! Grab him!\"",
		"l   Daniel is grabbed. He attempts to reach out \nto Silas, but he is carried away.   l",
		"Silas: \"Damn it! Those Gloomwood scoundrels! I have to get him back.\"",
	};
	
	// Tracks current dialogue line
	int currentText = 0;


	// Node references
	Panel CutscenePanel;
	Label CutsceneText;
	Label SpaceText;
		
	// Original position reference to undo text shaking
	Vector2 TextPosition;
	
	
	// Total amount of time a shake takes (not fully implemented - only affects the "WAKEUP" convo)
	[Export] float shakeTotalTime;
	// Amount of time between each shake movement
	[Export] float shakeBetweenTime;
	// How much each shake moves the text (not fully implemented - only affects the "WAKEUP" convo)
	[Export] int currentShakeStrength = 9;
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
	
	//Signal to changeMusic
	[Signal] public delegate void playMusic(AudioStream music);
	//Holder for music 
	AudioStream music;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		// setup vars
		CutsceneText = GetNode<Label>("CutscenePanel/CutsceneText");
		SpaceText = GetNode<Label>("CutscenePanel/PressSpaceText");
		SpaceText.Visible = false;
		chatterLimit = textList[currentText].Length;
		CutsceneText.Text = textList[currentText];
		TextPosition = CutsceneText.GetPosition();
		//Play music
		music = GD.Load<AudioStream>("res://Sounds/Music/OpeningDialogueLoop.mp3");
		Connect(nameof(playMusic), GetNode(".."), "changeMusic");
	}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  	public override void _Process(float delta) {
		AudioStreamPlayer player = GetNode<AudioStreamPlayer>("/Main/Music");
		
		if (!player.Playing){
			EmitSignal(nameof(playMusic), music);
		}
		
		if (Input.IsActionJustPressed("attack")) {
			// If not at end of dialogue
			if (currentText != 45) {
				// Stop any possible shakes
				stopShake();
				// Hide text
				CutsceneText.Visible = false;
				// Reset text position
				CutsceneText.SetPosition(TextPosition);
				// Advance text var
				currentText++;
				// Reset space prompt timer
				currentTextTime = 0;
				// Reset scroller
				drawTextSpeed = 0;
				CutsceneText.VisibleCharacters = 0;
				// Hide space prompt
				SpaceText.Visible = false;
				// Reset scroller
				chatterLimit = textList[currentText].Length;
				// Ready text
				CutsceneText.Text = textList[currentText];
				// Shake for wakeup convo
				if (currentText == 5 || currentText == 6) {
					shaking = true;
				}
				// Slow text afterwards
				else if (currentText == 7) {
					scrollSpeedScale /= 1.85f;
				}
				// Bring text back to normal speed
				else if (currentText == 8) {
					scrollSpeedScale *= 1.85f;
				}
				// Shake longer but less violently when knighs come
				else if (currentText == 40 || currentText == 42) {
					shaking = true;
					currentShakeStrength = 4;
					shakeTotalTime *= 3;
				}
				else if (currentText == 41) {
					shakeTotalTime /=3;
				}
				// Make text visible
				CutsceneText.Visible = true;
			}
			// End of dialogue, play game
			else {
				GetTree().ChangeScene("res://Scenes/Main.tscn");
			}
		}
		// If esc is pressed, quit game
		else if (Input.IsActionPressed("pause")) {
			GetTree().Quit();
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
		scrollSpeedSum += (delta*scrollSpeedScale);
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
						CutsceneText.SetPosition(new Vector2(CutsceneText.GetPosition().x+currentShakeStrength,CutsceneText.GetPosition().y));
					}
					else if (currentShakeMovement == 2 || currentShakeMovement == 4){
						CutsceneText.SetPosition(new Vector2(CutsceneText.GetPosition().x,CutsceneText.GetPosition().y+currentShakeStrength));
					}
					else if (currentShakeMovement == 3 || currentShakeMovement == 6 || currentShakeMovement == 10) {
						CutsceneText.SetPosition(new Vector2(CutsceneText.GetPosition().x-currentShakeStrength,CutsceneText.GetPosition().y));
					}
					else {
						CutsceneText.SetPosition(new Vector2(CutsceneText.GetPosition().x,CutsceneText.GetPosition().y-currentShakeStrength));
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
	
	// Scroll function
	public void showChatter() {
		if (drawTextSpeed < chatterLimit) {
			drawTextSpeed++;
			CutsceneText.VisibleCharacters = drawTextSpeed;
		}
	}
	
	// Stop text shaking and reset vars
	public void stopShake() {
		shaking = false;
		shakeTotalTimer = 0;
		shakeBetweenTimer = 0;
		currentShakeMovement = 1;
	}
	
}
