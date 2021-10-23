using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class UILevelSelector : MonoBehaviour
{
    [SerializeField]
    private LevelsList m_levels;

    [SerializeField]
    private MazeWorld m_world;

    [Tooltip("UI References")]
    [SerializeField]
    private TMPro.TextMeshProUGUI m_levelName; 

    private int m_currentLevelIndex = 0;

    private void Start()
    {
        m_levelName.text = m_world.CurrentLevel.Name;
    }

    public void NextLevel()
    {
        ++m_currentLevelIndex;
        m_currentLevelIndex = m_currentLevelIndex % m_levels.Levels.Count;
        m_world.CurrentLevel = m_levels.Levels[m_currentLevelIndex];
        m_levelName.text = m_world.CurrentLevel.Name;
    }

    public void PrevLevel()
    {
        --m_currentLevelIndex;
        if (m_currentLevelIndex < 0) m_currentLevelIndex = m_levels.Levels.Count - 1;
        m_world.CurrentLevel = m_levels.Levels[m_currentLevelIndex];
        m_levelName.text = m_world.CurrentLevel.Name;
    }
}
