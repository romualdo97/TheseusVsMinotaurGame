using System.Collections.Generic;
using UnityEngine;

public class MazeSolver : MonoBehaviour
{
    [SerializeField]
    private uint m_maxRecursiveDepth = 100;

    [SerializeField]
    private MazeWorld m_world;

    [SerializeField]
    private Player m_player;

    [SerializeField]
    private Enemy m_enemy;

    private EnemyAI m_enemyAI;
    private SolverWalker m_result;
    private uint m_currentRecursiveDepth = 0;

    public TurnCommands[] DoSolve()
    {
        return Solve(m_player.MazePos, m_enemy.MazePos);
    }

    private TurnCommands[] Solve(Vector2Int startPlayer, Vector2Int startEnemy)
    {
        // Prepare maze exploration
        m_enemyAI = new EnemyAI(m_world);
        MazeCellNode exitNode = m_world.MapInfo.GetNodeByCoord(m_world.CurrentLevel.Exit);
        MazeCellNode playerStartNode = m_world.MapInfo.GetNodeByCoord(startPlayer);
        MazeCellNode enemyStartNode = m_world.MapInfo.GetNodeByCoord(startEnemy);
        
        // Manage recursion
        m_currentRecursiveDepth = 0;
        m_result = null;

        // Walker are explorer in the maze graph
        SolverWalker firstWalker = new SolverWalker(playerStartNode, enemyStartNode, exitNode, m_enemyAI);
        Traverse(firstWalker);

        // Generate commands for the TurnManager
        if (m_result != null)
        {
            var commands = new TurnCommands[m_result.Steps.Count - 1];
            for (int i = 0; i < m_result.Steps.Count - 1; i++)
            {
                var from = m_result.Steps[i];
                var to = m_result.Steps[i + 1];
                var dir = NodeDirection(from, to);

                commands[i] = new TurnCommands(m_player, m_enemy, dir);
            }

            Debug.LogFormat("<color=green>{0}</color>", "Solver finished...");
            return commands;
        }

        Debug.LogFormat("<color=red>{0}</color>", "Solver finished without answer...");
        return null;
    }

    private void Traverse(SolverWalker walker)
    {
        Queue<SolverWalker> frontier = new Queue<SolverWalker>();
        HashSet<MazeCellNode> visited = new HashSet<MazeCellNode>();
        List<SolverWalker> deadEnds = new List<SolverWalker>();

        frontier.Enqueue(walker);
        visited.Add(walker.Curr);

        // Check frontier elements
        while (frontier.Count > 0)
        {
            SolverWalker currentWalker = frontier.Dequeue();
            MazeCellNode currentNode = currentWalker.Curr;

            // Check if should visit neighbors
            for (int i = 0; i < currentNode.Connections.Length; ++i)
            {
                var nearNode = currentNode.Connections[i];

                // Ignore if no connection
                if (nearNode == null) continue;

                // If near node is not visited... 
                if (!visited.Contains(nearNode)) // Use the keys for tracking if something was visited before
                {
                    // Create a new walker on current cell
                    var newWalker = currentWalker.Clone();
                    SolverWalker.MoveResult moveResult = newWalker.Move((MazeCellEdges)i);

                    if (moveResult != SolverWalker.MoveResult.Dead)
                    {
                        // Walker has found the exit?
                        if (newWalker.HasArrivedExit)
                        {
                            m_result = newWalker;
                            return;
                        }

                        frontier.Enqueue(newWalker); // ... mark for visiting later
                        visited.Add(nearNode);
                    }
                    else
                    {
                        // Add to dead ends only if has no arrived to root node
                        if (currentWalker.Curr != walker.Curr) deadEnds.Add(currentWalker);
                    }
                }
            }
        }

        // Manage recursive depth
        ++m_currentRecursiveDepth;
        if (m_currentRecursiveDepth >= m_maxRecursiveDepth)
        {
            return;
        }

        // If arrived to dead ends... continue exploring again from the dead end state
        for (int i = 0; i < deadEnds.Count; ++i)
        {
            Traverse(deadEnds[i]);
            if (m_result != null)
            {
                return;
            }
        }
    }

    private MazeCellEdges NodeDirection(MazeCellNode from, MazeCellNode to)
    {
        for (int i = 0; i < from.Connections.Length; ++i)
        {
            if (from.Connections[i] == to)
            {
                return (MazeCellEdges)i;
            }
        }

        return MazeCellEdges.None;
    }
}

