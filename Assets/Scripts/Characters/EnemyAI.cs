using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI
{
    private MazeWorld m_world;

    public EnemyAI(MazeWorld world)
    {
        m_world = world;
    }
    
    public MazeCellEdges CalculateNextStep(Vector2Int currPos, Vector2Int playerPos)
    {
        // Player and enemy pos
        if (playerPos == currPos) return MazeCellEdges.None;

        // Next 4 possible positions
        Vector2Int upPos = currPos + Vector2Int.up;
        Vector2Int downPos = currPos + Vector2Int.down;
        Vector2Int rightPos = currPos + Vector2Int.right;
        Vector2Int leftPos = currPos + Vector2Int.left;

        // Calculate manhattan distances
        int upToPlayerDistance = (playerPos - upPos).ManhattanDistance();
        int downToPlayerDistance = (playerPos - downPos).ManhattanDistance();
        int leftToPlayerDistance = (playerPos - leftPos).ManhattanDistance();
        int rightToPlayerDistance = (playerPos - rightPos).ManhattanDistance();
        int[] distances = new int[4] { upToPlayerDistance, downToPlayerDistance, leftToPlayerDistance, rightToPlayerDistance };

        // Which option will move the enemy closer? (ignoring the walls)
        int min = int.MaxValue;
        for (int i = 0; i < distances.Length; i++)
        {
            min = distances[i] < min ? distances[i] : min;
        }

        List<int> bestOptions = new List<int>();
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] == min)
            {
                bestOptions.Add(i);
            }
        }

        // There's an horizontal best option?
        MazeCellEdges dir = MazeCellEdges.None;
        for (int i = 0; i < bestOptions.Count; i++)
        {
            MazeCellEdges option = (MazeCellEdges)bestOptions[i];

            // Looking for 2 or 3 (horizontal movement option)
            if (bestOptions[i] > 1 && !m_world.MapInfo.IsWallAt(option, currPos))
            {
                dir = option;
            }
        }

        // If don't found an horizontal movement... use wathever it is the best option (which should be a vertical dir)
        if (dir == MazeCellEdges.None && !m_world.MapInfo.IsWallAt((MazeCellEdges)bestOptions[0], currPos))
        {
            dir = (MazeCellEdges)bestOptions[0];
        }

        return dir;
    }
}
