using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnManager : MonoBehaviour
{
    public bool IsExecutingTurn { get => m_turn != null; }

    [SerializeField]
    private Player m_player;

    [SerializeField]
    private Enemy m_enemy;

    private TurnCommands m_turn;
    private TurnCommands m_prevTurn;
    private int m_activeCommand = 0;
    private bool m_isUndoing = false;

    private void Update()
    {
        if (m_turn == null) return;

        if (m_turn[m_activeCommand].Started && m_turn[m_activeCommand].IsCompleted())
        {
            UpdateActiveCommand();
            if (!IsNextCommand())
            {
                if (!m_isUndoing) m_prevTurn = m_turn;
                m_turn = null;

                if (PlayerSucceedTurn())
                {
                    Debug.Log("Ok");
                }
                else
                {
                    Debug.Log("Failed");
                }

                return;
            }

            ExecuteNextCommand();
        }
    }

    public void ExecuteTurn(TurnCommands commands)
    {
        m_isUndoing = false;
        m_activeCommand = 0;
        m_turn = commands;
        ExecuteNextCommand();
    }

    public void UndoTurn()
    {
        if (m_prevTurn == null) return;

        m_isUndoing = true;
        m_activeCommand = TurnCommands.COMMANDS_PER_TURN - 1;
        m_turn = m_prevTurn;
        ExecuteNextCommand();
        m_prevTurn = null;
    }

    private bool IsNextCommand()
    {
        if (m_isUndoing)
        {
            return m_activeCommand >= 0;
        }
        else
        {
            return m_activeCommand < TurnCommands.COMMANDS_PER_TURN;
        }
    }

    private void UpdateActiveCommand()
    {
        m_turn[m_activeCommand].Reset();

        // Move pointer
        if (m_isUndoing)
        {
            --m_activeCommand;
        }
        else
        {
            ++m_activeCommand;
        }
    }

    private void ExecuteNextCommand()
    {
        if (m_isUndoing)
        {
            m_turn[m_activeCommand].UndoExecute();
        }
        else
        {
            m_turn[m_activeCommand].Execute();
        }
    }

    private bool PlayerSucceedTurn()
    {
        return m_player.MazePos != m_enemy.MazePos;
    }
}
