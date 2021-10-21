using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the command of the player movement
/// </summary>
public class MovePlayerCommand : Command
{
    private Player m_actor;
    private MazeCellEdges m_dir;
    private MazeCellEdges m_undoDir;

    public MovePlayerCommand(Player actor, MazeCellEdges dir)
    {
        m_actor = actor;
        m_dir = dir;
        m_undoDir = MazeCellNode.OPPOSITES[dir];
    }

    public override void Execute()
    {
        m_actor.Move(m_dir);
        base.Execute();
    }

    public override void UndoExecute()
    {
        m_actor.Move(m_undoDir);
        base.UndoExecute();
    }

    public override bool IsCompleted()
    {
        return !m_actor.IsMoving;
    }
}
