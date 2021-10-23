using System.Collections.Generic;

/// <summary>
/// Class used to represent a specific state of the maze.
/// </summary>
public class SolverWalker
{
    public enum MoveResult
    {
        /// <summary>
        /// The walker has arrived to a valid position.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// The walker has arrived to a dead state.
        /// </summary>
        Dead
    }

    public MazeCellNode Curr { get => m_curr; }
    public List<MazeCellNode> Steps { get => m_steps; }
    public bool HasArrivedExit { get => m_curr == m_exit; }

    private MazeCellNode m_curr;
    private MazeCellNode m_enemy;
    private MazeCellNode m_exit;
    private EnemyAI m_enemAI;
    private List<MazeCellNode> m_steps;
    MoveResult m_status = MoveResult.Ok;

    public SolverWalker(MazeCellNode curr, MazeCellNode enemy, MazeCellNode exit, EnemyAI enemyAI)
    {
        m_curr = curr;
        m_enemy = enemy;
        m_exit = exit;
        m_enemAI = enemyAI;
        m_steps = new List<MazeCellNode>();
        m_steps.Add(m_curr);
    }

    private SolverWalker(SolverWalker from) // Private copy constructor
    {
        m_curr = from.m_curr;
        m_enemy = from.m_enemy;
        m_exit = from.m_exit;
        m_enemAI = from.m_enemAI;
        m_steps = new List<MazeCellNode>(from.m_steps);
        m_status = from.m_status;
    }

    public SolverWalker Clone()
    {
        return new SolverWalker(this);
    }

    public MoveResult Move(MazeCellEdges dir)
    {
        int dirIdx = (int)dir;
        MazeCellNode nextPlayerNode = m_curr.Connections[dirIdx];
        MazeCellNode nextEnemyNode = GetNextEnemyNode(m_enemy, nextPlayerNode);

        // Check if blocked by enemy
        bool willHitEnemy = nextPlayerNode == m_enemy;
        bool willBeHuntByEnemy = nextPlayerNode == nextEnemyNode;

        #region Always move
        // Track walked steps
        m_steps.Add(nextPlayerNode);

        // Update the maze because there's still a chance
        m_curr = nextPlayerNode;
        m_enemy = nextEnemyNode;
        #endregion

        #region Check final status
        // Check if has arrived to a dead state
        if (willHitEnemy || willBeHuntByEnemy)
        {
            m_status = MoveResult.Dead;
            return m_status;
        }

        m_status = MoveResult.Ok;
        return m_status;
        #endregion
    }

    private MazeCellNode GetNextEnemyNode(MazeCellNode enemy,  MazeCellNode player)
    {
        MazeCellEdges enemDir = m_enemAI.CalculateNextStep(enemy.Coord, player.Coord);
        if (enemDir != MazeCellEdges.None)
        {
            // Set first step
            enemy = enemy.Connections[(int)enemDir];

            enemDir = m_enemAI.CalculateNextStep(enemy.Coord, player.Coord);
            if (enemDir != MazeCellEdges.None)
            {
                // Set second step
                enemy = enemy.Connections[(int)enemDir];
            }
        }

        return enemy;
    }
}
