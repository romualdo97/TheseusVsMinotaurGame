using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple storage for all maze node cells
/// </summary>
public class MazeGraph
{
    // Indexer declaration
    public MazeCellNode this[int index]
    {
        get => m_nodes[index];
    }

    MazeCellNode[] m_nodes;

    public MazeGraph(int graphOrder)
    {
        m_nodes = new MazeCellNode[graphOrder];

        for (int i = 0; i < graphOrder; ++i)
        {
            m_nodes[i] = new MazeCellNode(i);
        }
    }
}
