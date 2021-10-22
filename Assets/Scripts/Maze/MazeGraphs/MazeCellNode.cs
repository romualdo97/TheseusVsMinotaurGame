using UnityEngine;
using System.Collections.Generic;

public enum MazeCellEdges
{
    None = -1,
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
    public static readonly Dictionary<MazeCellEdges, MazeCellEdges> OPPOSITES = new Dictionary<MazeCellEdges, MazeCellEdges>()
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
    public Vector2Int Coord { get => m_coord; set => m_coord = value; }


    // The connections/edges of this node (only 4 for each cell)
    private MazeCellNode[] m_connections = new MazeCellNode[4] { null, null, null, null };
    private Vector2Int m_coord;

    public MazeCellNode(int id, int mazeWidth)
    {
        Id = id;
        m_coord = new Vector2Int(Id % mazeWidth, Mathf.FloorToInt((float)Id / mazeWidth));
    }

    public void Connect(MazeCellEdges edge, MazeCellNode other) // Connect... or join to vertices
    {
        m_connections[(int)edge] = other;
        other.m_connections[(int)OPPOSITES[edge]] = this;

        other.HasConnections = true;
        HasConnections = true;
    }
}
