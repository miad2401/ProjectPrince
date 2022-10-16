using Godot;
using System;
using System.Collections;

public class EnemySpawner : Control
{
    [Export] enemyType EnemyType;
    [Export] float timeToSpawn;
    [Export] int maxEnemiesSpawned;
    [Export] int iHSpeed;
    [Export] int hDrag;
    [Export] int vSpeed;
    [Export] int hSpeed;
    [Export] int maxVSpeed;
    [Export] int gravity;
    [Export] Direction direction;
    [Export] bool canSpawn;

    private Vector2 SpawnLocation;
    private PackedScene bossBatScene;
    private PackedScene knightScene;
    private float timeProgress;

    public int numEnemiesSpawned;
    public Queue spawnedEnemies = new Queue();

    public override void _Ready()
    {
        SpawnLocation = GetNode<Position2D>("SpawnLocation").GlobalPosition;
        bossBatScene = GD.Load<PackedScene>("res://Scenes/BossBat.tscn");
        knightScene = GD.Load<PackedScene>("res://Scenes/Knight.tscn");
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        timeProgress += delta;
        if(timeProgress >= timeToSpawn && canSpawn)
        {
            timeProgress = 0;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        numEnemiesSpawned++;
        if(numEnemiesSpawned > maxEnemiesSpawned)
        {
            Node enemyToDespawn = spawnedEnemies.Dequeue() as Node;
            if(IsInstanceValid(enemyToDespawn))
            {
                enemyToDespawn.QueueFree();
            }
            numEnemiesSpawned--;
        }
        if (EnemyType == enemyType.Bat)
        {
            BossBat instancedBat = bossBatScene.Instance() as BossBat;
            instancedBat.SetIHSpeed(iHSpeed);
            instancedBat.SetDrag(hDrag);
            instancedBat.SetVSpeed(vSpeed);
            instancedBat.GlobalPosition = SpawnLocation;
            instancedBat.SetHurtPlayer(true);
            spawnedEnemies.Enqueue(instancedBat);
            GetParent().AddChild(instancedBat);
        }
        else if(EnemyType == enemyType.Knight)
        {
            Knight instancedKnight = knightScene.Instance() as Knight;
            instancedKnight.SetHSpeed(hSpeed);
            instancedKnight.SetMaxVSpeed(maxVSpeed);
            instancedKnight.SetGravity(gravity);
            instancedKnight.SetDirection(direction);
            instancedKnight.GlobalPosition = SpawnLocation;
            instancedKnight.SetHurtPlayer(true);
            spawnedEnemies.Enqueue(instancedKnight);
            GetParent().AddChild(instancedKnight);
        }
    }
}
