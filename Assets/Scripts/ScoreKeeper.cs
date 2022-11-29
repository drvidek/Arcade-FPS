using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreKeeper : MonoBehaviour
{
    public static int Score { get; private set; }
    public float GameTime { get; private set; }

    [SerializeField] private TextMeshProUGUI _scoreDisplay;

    public static void IncreaseScore(int val)
    {
        Score += val;
    }

    private void ResetScore()
    {
        Score = 0;
    }

    public void UpdateDisplay()
    {
        _scoreDisplay.text = "Slimes killed: " + Score; 
    }

    public static void Reset()
    {
        Score = 0;
    }

}

