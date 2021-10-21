using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private MazeWorld m_world;

    [SerializeField]
    private GameTurnManager m_turnManager;

    [SerializeField]
    private Player m_player;

    [SerializeField]
    private Enemy m_enemy;

    private void Start()
    {
        Assert.IsNotNull(m_player, "InputManager requires a reference of player");
        Assert.IsNotNull(m_enemy, "InputManager requires a reference of enemy");
    }

    private void Update()
    {
        if (!m_player.IsInitialized || !m_enemy.IsInitialized || m_turnManager.IsExecutingTurn) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_turnManager.UndoTurn();
            return;
        }

        TurnCommands turn = HandleInput();
        if (turn != null)
        {
            m_turnManager.ExecuteTurn(turn);
        }
    }

    private TurnCommands HandleInput()
    {
        // Check valid input
        MazeCellEdges dir = MazeCellEdges.None;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir = MazeCellEdges.Top;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir = MazeCellEdges.Bottom;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = MazeCellEdges.Left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = MazeCellEdges.Right;
        }

        if (IsInvalidInput(dir)) return null;

        return new TurnCommands(m_player, m_enemy, dir);
    }

    private bool IsInvalidInput(MazeCellEdges dir)
    {
        return m_world.MapInfo.IsWallAt(dir, m_player.MazePos);
    }
}
