using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button tryAgainButton;
    private CanvasGroup canvasGroup;
    private float fadeDuration = 1f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        tryAgainButton.onClick.AddListener(() =>
        {
            GameManager.Instance.NewGame();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
        });
    }
    private void Start()
    {
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver()
    {
        StartCoroutine(GameOver());
    }
    private IEnumerator GameOver()
    {
        int waitSecond = 1;
        yield return new WaitForSeconds(waitSecond);
        canvasGroup.DOFade(1f, fadeDuration);
        canvasGroup.interactable = true;
    }
}
