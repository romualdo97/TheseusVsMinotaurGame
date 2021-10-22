//#define ENABLE_SUPREME_KILLER_INSTINCT
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Enemy : CharacterEntity
{
    [SerializeField]
    private Player m_player;

    private EnemyAI m_ai;

    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(m_player, "Enemy requires a reference of player");

        InitEntity();

        // Instance the brain of the enemy
        m_ai = new EnemyAI(m_world);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateEntity();

        // Some debug
        if (Input.GetKeyDown(KeyCode.F))
        {
            Move(CalculateNextStep());
        }
    }

    public MazeCellEdges CalculateNextStep()
    {
        return m_ai.CalculateNextStep(MazePos, m_player.MazePos);
    }

    protected override void CalculateInitialMazePos()
    {
        if (m_world.MapInfo.IsValidCoord(m_world.CurrentLevel.EnemyPos) && m_world.CurrentLevel.EnemyPos != m_world.CurrentLevel.PlayerPos)
        {
            m_mazePos = m_world.CurrentLevel.EnemyPos;
        }
        else
        {
            do
            {
                base.CalculateInitialMazePos();
            }
            while (MazePos == m_player.MazePos || !m_world.MapInfo.IsValidCoord(MazePos));
        }
    }
}
