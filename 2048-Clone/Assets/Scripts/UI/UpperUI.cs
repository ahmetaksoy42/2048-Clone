using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpperUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private Button newGameButton;
    private void Awake()
    {
        newGameButton.onClick.AddListener(() =>
        {
            GameManager.Instance.NewGame();
        });
    }
    void Start()
    {
        LoadBestScoreText();
        GameManager.Instance.OnScoreUpdate += GameManager_OnScoreUpdate;
        GameManager.Instance.OnBestScoreUpdate += GameManager_OnBestScoreUpdate;
    }

    private void GameManager_OnScoreUpdate(int score)
    {
        scoreText.text = score.ToString();
    }
    private void GameManager_OnBestScoreUpdate()
    {
        LoadBestScoreText();
    }

    private void LoadBestScoreText()
    {
        bestScoreText.text = GameManager.Instance.LoadBestScore().ToString();
    }
}
