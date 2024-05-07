using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Image loadImage;
    [SerializeField] private AnimationCurve loadCurve;

    private Game game;
    private Vector3 initialScale;
    private bool started;

    private void Awake()
    {
        game = FindObjectOfType<Game>();
        loadImage.gameObject.SetActive(true);

        initialScale = loadImage.rectTransform.localScale;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 && !started)
        {
            if (Input.anyKeyDown)
            {
                StartCoroutine(ScaleTween(Vector3.zero, 1, () =>
                {
                    game.InitializeAll();
                    loadImage.GetComponentInChildren<SpriteRenderer>().enabled = false;
                }));

                started = true;
            }
        }          
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            StartLevel();
    }

    public void RestartLevel() 
    {
        game.DeinitializeAll();

        EndLevel(() =>
        {
            game.TeleportPlayerToLastCheckPoint();

            StartLevel();
        });
    }

    public void StartNextLevel()
    {
        game.DeinitializeAll();

        EndLevel(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

    private void StartLevel()
    {
        StartCoroutine(ScaleTween(Vector3.zero, 0, game.InitializeAll));
    }

    private void EndLevel(Action onAnimationEnd) 
    {
        StartCoroutine(ScaleTween(initialScale, 1, onAnimationEnd));
    }

    public IEnumerator ScaleTween(Vector3 finalScale, float duration, Action onAnimationEnd)
    {
        RectTransform imageTransform = loadImage.rectTransform;
        float currentTime = 0f, targetTime = 1f;
        Vector3 initialScale = imageTransform.localScale;

        while (currentTime != targetTime)
        {
            currentTime = Mathf.MoveTowards(currentTime, targetTime, Time.deltaTime / duration);

            imageTransform.localScale = Vector3.Lerp(initialScale, finalScale, loadCurve.Evaluate(currentTime));

            yield return new WaitForEndOfFrame();
        }

        onAnimationEnd.Invoke();
    }
}