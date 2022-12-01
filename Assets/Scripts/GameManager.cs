using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Pre,
    Play,
    Post
}

public class GameManager : MonoBehaviour
{
    public static GameState State { get; private set; } = GameState.Pre;
    public static bool IsPlaying { get => State == GameState.Play; }
    public static ScoreKeeper scoreKeeper;

    [SerializeField] private GameObject[] _uiGroup;

    private void Start()
    {
        scoreKeeper = GetComponent<ScoreKeeper>();
        NextState();
    }

    void NextState()
    {
        switch (State)
        {
            case GameState.Pre:
                StartCoroutine("Pre");
                break;
            case GameState.Play:
                StartCoroutine("Play");
                break;
            case GameState.Post:
                StartCoroutine("Post");
                break;
            default:
                break;
        }
    }

    IEnumerator Pre()
    {
        while (State == GameState.Pre)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                State = GameState.Play;
            yield return null;
        }
        NextState();
    }

    IEnumerator Play()
    {
        if (_uiGroup.Length > 0)
        {
            _uiGroup[0].SetActive(true);
            _uiGroup[1].SetActive(false);
        }
        while (State == GameState.Play)
        {
            yield return null;
        }
        NextState();
    }

    IEnumerator Post()
    {
        _uiGroup[0].SetActive(false);
        _uiGroup[1].SetActive(true);
        while (State == GameState.Post)
        {
            yield return null;
        }
        NextState();
    }

    public void GoToScene(int i)
    {
        if (SceneManager.GetSceneByBuildIndex(i) != null)
        {
            SceneManager.LoadScene(i);
        }
    }

    public void RestartGame()
    {
        State = GameState.Play;
        ScoreKeeper.Reset();
    }

    public void ResetState()
    {
        State = GameState.Pre;
    }

    public static void EndRound()
    {
        State = GameState.Post;
        scoreKeeper.UpdateDisplay();
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
