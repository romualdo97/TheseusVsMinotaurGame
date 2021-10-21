using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the command of the enemy movement
/// </summary>
public class MoveEnemyCommand : Command
{
    private Enemy m_actor;
    private Player m_player;
    private MazeCellEdges m_dir;
    private MazeCellEdges m_undoDir;

    public MoveEnemyCommand(Enemy actor, Player player)
    {
        m_actor = actor;
        m_player = player;
    }

    public override void Execute()
    {
        m_dir = Enemy.CalculateNextStep(m_actor, m_player);
        m_undoDir = m_dir != MazeCellEdges.None ? MazeCellNode.OPPOSITES[m_dir] : MazeCellEdges.None;
        m_actor.Move(m_dir);
        base.Execute();
    }

    public override void UndoExecute()
    {
        m_actor.Move(m_undoDir);
        base.Execute();
    }

    public override bool IsCompleted()
    {
        return !m_actor.IsMoving;
    }
}
