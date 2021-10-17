using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsList", menuName = "FancyMazeWorld/LevelsList")]
public class LevelsList : ScriptableObject
{
    [Tooltip("List of all selectable levels")]
    public List<MazeLevel> Levels;
}
