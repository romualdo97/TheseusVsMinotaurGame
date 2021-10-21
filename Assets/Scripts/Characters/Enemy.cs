//#define ENABLE_SUPREME_KILLER_INSTINCT
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Enemy : CharacterEntity
{
    [SerializeField]
    private Player m_player;

    public static MazeCellEdges CalculateNextStep(Enemy enemy, Player player)
    {
        MazeWorld world = enemy.m_world;

        // Player and enemy pos
        Vector2Int playerPos = player.MazePos;
        Vector2Int currPos = enemy.MazePos;
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
            if (bestOptions[i] > 1 && !world.MapInfo.IsWallAt(option, currPos))
            {
                dir = option;
            }
        }

        // If don't found an horizontal movement... use wathever it is the best option (which should be a vertical dir)
        if (dir == MazeCellEdges.None && !world.MapInfo.IsWallAt((MazeCellEdges)bestOptions[0], currPos))
        {
            dir = (MazeCellEdges)bestOptions[0];
        }

        return dir;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(m_player, "Enemy requires a reference of player");

        InitEntity();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateEntity();

        // Some debug
        if (Input.GetKeyDown(KeyCode.F))
        {
            Move(CalculateNextStep(this, m_player));
        }
    }

    protected override void CalculateInitialMazePos()
    {
        if (m_world.MapInfo.IsValidCoord(m_world.CurrentLevel.EnemyPos) && m_world.CurrentLevel.EnemyPos != m_world.CurrentLevel.PlayerPos)
        {
            m_mazePos = m_world.CurrentLevel.EnemyPos;
        }
        else
        {
            do
            {
                base.CalculateInitialMazePos();
            }
            while (MazePos == m_player.MazePos || !m_world.MapInfo.IsValidCoord(MazePos));
        }
    }
}
