using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// READ ME
/// Attach this script to an empy object under a Canvas
/// To prevent values sliding after releasing the key,
/// </summary>
/// 

[RequireComponent(typeof(TextMeshProUGUI))]
public class ArcadeNameInput : MonoBehaviour
{
    [Header("System")]
    private static char[] _alphabet = new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
    private int[] _slot;
    private int _slotIndex;
    [Tooltip("The name of the horizontal axis to use for inputs - this must be set up in the Input Manager")]
    [SerializeField] private string _horiInput = "P1 Hori";
    [Tooltip("The name of the vertical axis to use for inputs - this must be set up in the Input Manager")]
    [SerializeField] private string _vertiInput = "P1 Verti";
    [Tooltip("The number of letters allowed in the name")]
    [SerializeField] private int _characterLimit = 3;
    [Tooltip("The delay period between inputs")]
    [SerializeField] private float _inputDelayMax = 0.3f;
    [Tooltip("The number of consecutive inputs before delay reduction is applied")]
    [SerializeField] private int _consecutiveInputMax = 3;
    [Tooltip("The percent of the full delay to use when consecutive input reduction is applied (1 = 100% of full delay, 0.3 = 30% of full delay, etc)")]
    [Range(0, 1)]
    [SerializeField] private float _inputDelayReduction = 0.5f;
    private float _inputDelay;
    private int _consecutiveInput;

    public string InputName { get => GetName(); }

    [Header("Display")]
    [Tooltip("The text component of the display")]
    [SerializeField] private TextMeshProUGUI _displayText;
    [Tooltip("The base letter color for the text")]
    [SerializeField] private Color _baseColor = Color.black;
    [Tooltip("The highlighted letter color for the text")]
    [SerializeField] private Color _highlightColor = Color.white;
    private string _baseColHex { get => ColorUtility.ToHtmlStringRGBA(_baseColor); }
    private string _highlightColHex { get => ColorUtility.ToHtmlStringRGBA(_highlightColor); }

    void OnValidate()
    {
        _displayText = GetComponent<TextMeshProUGUI>();
        _slot = new int[_characterLimit];
        UpdateDisplay();
    }

    void Update()
    {
        if (InputReady())
        {
            Vector2 input = GetInput();
            if (input.magnitude > 0)
            {
                UpdateArrayPositions(input);
                UpdateDisplay();
                ResetInputDelay();
            }
        }
        CheckConsecutiveInputReset();
    }

    #region Inputs
    /// <summary>
    /// Get the input from the axes of choice
    /// </summary>
    /// <returns></returns>
    Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;
        input.x = (Input.GetAxis(_horiInput));
        input.y = -1 * (Input.GetAxis(_vertiInput));
        return input;
    }

    /// <summary>
    /// Count down the input delay and return true when it reaches 0
    /// </summary>
    /// <returns></returns>
    bool InputReady()
    {
        if (_inputDelay <= 0)
        {
            return true;
        }
        _inputDelay = Mathf.MoveTowards(_inputDelay, 0, Time.deltaTime);
        return false;
    }

    /// <summary>
    /// Check if the player has let go of the input axis
    /// </summary>
    void CheckConsecutiveInputReset()
    {
        Vector2 input = GetInput();
        if (input.y == 0)
            _consecutiveInput = 0;
    }

    /// <summary>
    /// Reset the input delay based on consecutive inputs and delay reduction
    /// </summary>
    void ResetInputDelay()
    {
        _inputDelay = _consecutiveInput > _consecutiveInputMax ? _inputDelayMax * _inputDelayReduction : _inputDelayMax;
    }
    #endregion

    #region Update Name
    /// <summary>
    /// Update the positions of the array indexes using an input
    /// </summary>
    /// <param name="input"></param>
    void UpdateArrayPositions(Vector2 input)
    {
        if (input.x != 0)
        {
            _slotIndex = Wrapindex(_slotIndex + (int)Mathf.Sign(input.x), 0, _slot.Length - 1);
        }
        else
        {
            _slot[_slotIndex] = Wrapindex(_slot[_slotIndex] + (int)Mathf.Sign(input.y), 0, _alphabet.Length - 1);
            _consecutiveInput++;
        }
    }

    /// <summary>
    /// Update the text component with the string
    /// </summary>
    void UpdateDisplay()
    {
        string name = "";
        for (int i = 0; i < _slot.Length; i++)
        {
            name += $"<color=#{(i == _slotIndex ? _highlightColHex : _baseColHex)}>{_alphabet[_slot[i]]}";
        }
        _displayText.text = name.ToUpper() ;
    }
    #endregion

    /// <summary>
    /// Return the name as a string
    /// </summary>
    /// <returns></returns>
    private string GetName()
    {
        string name = "";
        for (int i = 0; i < _slot.Length; i++)
        {
            name += _alphabet[_slot[i]];
        }
        return name;
    }


    /// <summary>
    /// If n falls below min or above max, it will wrap to the other value
    /// </summary>
    /// <param name="n"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private int Wrapindex(int n, int min, int max)
    {
        if (n < min)
        {
            n = max;
            return n;
        }
        if (n > max)
        {
            n = min;
            return n;
        }
        return n;
    }
}

