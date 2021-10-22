using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnManager : MonoBehaviour
{
    public bool IsExecutingTurn { get => m_turn != null; }

    [SerializeField]
    private MazeWorld m_world;

    [SerializeField]
    private Player m_player;

    [SerializeField]
    private Enemy m_enemy;

    private Queue<TurnCommands> m_allTurns = new Queue<TurnCommands>();
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

            // Commands in turns finished...
            if (!IsNextCommand())
            {
                // Avoid cyclic undo/redo
                if (!m_isUndoing) m_prevTurn = m_turn;
                m_turn = null;

                // Trigger end turn events
                TriggerEndTurnEvents();

                // Dequeues a new turn if prev already completed
                if (m_allTurns.Count > 0) ExecuteTurn(m_allTurns.Dequeue());
                return;
            }

            ExecuteNextCommand();
        }
    }

    public void AddTurns(params TurnCommands[] turns) // Schedules a sequence of turns... useful for the solver
    {
        if (turns.Length == 0) throw new System.InvalidOperationException("You must specify at least one turn to execute.");

        // Enqueue a sequence of turns
        foreach (var turn in turns)
        {
            m_allTurns.Enqueue(turn);
        }

        // Execute the first turn
        ExecuteTurn(m_allTurns.Dequeue());
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

    private void TriggerEndTurnEvents()
    {
        if (m_player.MazePos != m_enemy.MazePos)
        {
            Debug.LogFormat("<color=yellow>{0}</color>", "Ok!!!");

            if (m_player.MazePos == m_world.CurrentLevel.Exit)
            {
                Debug.LogFormat("<color=green>{0}</color>", "Level passed!!!");
            }
        }
        else
        {
            Debug.LogFormat("<color=red>{0}</color>", "Failed!");
        }
    }

}
