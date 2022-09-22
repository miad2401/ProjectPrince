using Godot;
using System;

public enum enemyType
{
    Bat,
    Knight,
    Rival
}
public class EnemySpawner : Control
{
    [Export] enemyType EnemyType;
    [Export] float timeToSpawn;
    [Export] int maxEnemiesSpawned;
    [Export] int iHSpeed;
    [Export] int hDrag;
    [Export] int vSpeed;

    private Vector2 SpawnLocation;
    private PackedScene bossBatScene;
    private PackedScene knightScene;
    private float timeProgress;

    public static int numEnemiesSpawned;

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
        if(timeProgress >= timeToSpawn && numEnemiesSpawned < maxEnemiesSpawned)
        {
            timeProgress = 0;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        if (EnemyType == enemyType.Bat)
        {
            BossBat instancedBat = bossBatScene.Instance() as BossBat;
            instancedBat.SetIHSpeed(iHSpeed);
            instancedBat.SetDrag(hDrag);
            instancedBat.SetVSpeed(vSpeed);
            instancedBat.GlobalPosition = SpawnLocation;
            instancedBat.SetHurtPlayer(true);
            GetParent().AddChild(instancedBat);
        }
    }
}
