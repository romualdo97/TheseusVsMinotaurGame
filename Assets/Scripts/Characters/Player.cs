using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : CharacterEntity
{
    [SerializeField]
    private Enemy m_enemy;

    private void Start()
    {
        Assert.IsNotNull(m_enemy, "Player requires a reference of enemy");

        InitEntity();
    }

    private void Update()
    {
        UpdateEntity();

        // Some debug
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    Move(MazeCellEdges.Top);
        //}
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    Move(MazeCellEdges.Bottom);
        //}
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    Move(MazeCellEdges.Left);
        //}
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    Move(MazeCellEdges.Right);
        //}
    }

    protected override void CalculateInitialMazePos()
    {
        if (m_world.MapInfo.IsValidCoord(m_world.CurrentLevel.PlayerPos))
        {
            m_mazePos = m_world.CurrentLevel.PlayerPos;
        }
        else
        {
            do
            {
                base.CalculateInitialMazePos();
            }
            while (MazePos == m_enemy.MazePos || !m_world.MapInfo.IsValidCoord(MazePos));
        }
    }
}
