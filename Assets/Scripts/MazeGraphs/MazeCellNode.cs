using System.Collections.Generic;

public enum MazeCellEdges
{
    Top = 0,
    Bottom = 1,
    Left = 2,
    Right = 3
}

/// <summary>
/// Represents a maze cell and its connections
/// </summary>
public class MazeCellNode
{
    // Simple table for opposite edges
    private static readonly Dictionary<MazeCellEdges, MazeCellEdges> OPPOSITES = new Dictionary<MazeCellEdges, MazeCellEdges>()
    {
        { MazeCellEdges.Top, MazeCellEdges.Bottom },
        { MazeCellEdges.Bottom, MazeCellEdges.Top },
        { MazeCellEdges.Left, MazeCellEdges.Right },
        { MazeCellEdges.Right, MazeCellEdges.Left }
    };

    // Public properties
    public int Id { get; private set; }
    public MazeCellNode[] Connections { get => m_connections; }
    public bool HasConnections { get; private set; }

    // The id of this node in the graph...
    private int m_id = -1;

    // The connections/edges of this node (only 4 for each cell)
    private MazeCellNode[] m_connections = new MazeCellNode[4] { null, null, null, null };

    public MazeCellNode(int id)
    {
        m_id = id;
    }

    public void Connect(MazeCellEdges edge, MazeCellNode other) // Connect... or join to vertices
    {
        m_connections[(int)edge] = other;
        other.m_connections[(int)OPPOSITES[edge]] = this;

        other.HasConnections = true;
        HasConnections = true;
    }
}
