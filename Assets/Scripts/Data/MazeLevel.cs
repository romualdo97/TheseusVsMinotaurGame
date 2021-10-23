using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MazeLevel", menuName = "FancyMazeWorld/MazeLevel")]
public class MazeLevel : ScriptableObject
{
    [Header("Metadata")]
    public string Name = "Mocky pocky";
    public Vector3 CameraPos = new Vector3(0, 0, 10);
    public float CameraFov = 60;
    public Color CameraClearColor;

    [Header("World")]
    [Tooltip("How many horizontal cells for the maze?")]
    public int Width = 2;

    [Tooltip("How many vertical cells for the maze?")]
    public int Height = 2;

    [Tooltip("-1 means random seed")]
    public int Seed = -1;

    [Tooltip("Chance of adding cycles to the maze graph")]
    [Range(0f, 1f)]
    public float ChanceOfCycles = 0;

    [Header("Win condition")]
    [Tooltip("A coord outside and adjacent to the world which will be considered the exit node")]
    public Vector2Int Exit = new Vector2Int(-1, 0);

    [Tooltip("What edge of the exit node is connected to the world")]
    public MazeCellEdges ExitConnectedEdge = MazeCellEdges.Right;

    [Header("Entities")]
    [Tooltip("The initial coord of the player, invalid coord means random initial pos")]
    public Vector2Int PlayerPos;

    [Tooltip("The initial coord of the enemy, invalid coord means random initial pos")]
    public Vector2Int EnemyPos;
}
