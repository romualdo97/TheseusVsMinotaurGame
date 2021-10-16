using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MazeLevel", menuName = "MazeLevel")]
public class MazeLevel : ScriptableObject
{
    [Tooltip("How many horizontal cells for the maze?")]
    public int Width = 2;

    [Tooltip("How many vertical cells for the maze?")]
    public int Height = 2;

    [Tooltip("-1 means random seed")]
    public int Seed = -1;

    [Tooltip("Chance of adding cycles to the maze graph")]
    [Range(0f, 1f)]
    public float ChanceOfCycles = 0;
}
