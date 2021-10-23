using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFeedback : MonoBehaviour
{
    enum FeedbackContent
    {
        Success,
        Fail
    }

    [SerializeField]
    UILevelSelector m_levelSelector;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_feedbackText;

    [SerializeField]
    private InputManager m_inputManager;

    private Animator m_animator;
    private FeedbackContent m_feedbackContent;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_animator.SetTrigger("ShowIntro");
    }

    public void ShowSuccess()
    {
        m_feedbackText.text = "Level passed";
        m_animator.SetTrigger("ShowFeedback");
        m_inputManager.InputEnabled = false;
        m_feedbackContent = FeedbackContent.Success;
    }

    public void ShowFail()
    {
        m_feedbackText.text = "Level failed";
        m_animator.SetTrigger("ShowFeedback");
        m_inputManager.InputEnabled = false;
        m_feedbackContent = FeedbackContent.Fail;
    }

    public void EnableInput()
    {
        m_inputManager.InputEnabled = true;
    }

    public void DoLevelFlow()
    {
        if (m_feedbackContent == FeedbackContent.Success)
        {
            m_levelSelector.NextLevel();
        }
        else
        {
            ResetLevel();
        }
    }

    private void ResetLevel()
    {
        m_inputManager.InputEnabled = true;
        MazeWorld.Instance.CurrentLevel = MazeWorld.Instance.CurrentLevel;
    }
}
