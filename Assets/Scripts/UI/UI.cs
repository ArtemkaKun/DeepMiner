using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text depth;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Slider fuelBar;
    [SerializeField] private Slider occupiedSpaceBar;
    
    private static float _moneyValue;
    private static float _fuelBarValue;
    private static float _occupiedSpaceBarValue;
    private static int _depthValue;
    private static int _scoreValue;

    private static Action _updateMoneyText;
    private static Action _updateDepthText;
    private static Action _updateScoreText;
    private static Action _updateFuelBar;
    private static Action _updateOccupiedSpaceBar;
    
    public static void AddMoney(float amount)
    {
        _moneyValue += amount;
        _updateMoneyText.Invoke();
    }

    public static void IncreaseDepth()
    {
        _depthValue++;
        _updateDepthText.Invoke();
    }

    public static void IncreaseScore(int amount)
    {
        _scoreValue += amount;
        _updateScoreText.Invoke();
    }

    public static void DecreaseFuel(float amount)
    {
        _fuelBarValue -= amount;
        _updateFuelBar.Invoke();
    }

    public static void IncreaseOccupiedSpace(float amount)
    {
        _occupiedSpaceBarValue += amount;
        _updateOccupiedSpaceBar.Invoke();
    }
    
    private void Awake()
    {
        _moneyValue = 0;
        _depthValue = 0;
        _scoreValue = 0;
        _occupiedSpaceBarValue = 0;
        _fuelBarValue = 100;
        
        _updateMoneyText += UpdateMoneyText;
        _updateDepthText += UpdateDepthText;
        _updateScoreText += UpdateScoreText;
        _updateFuelBar += UpdateFuelBar;
        _updateOccupiedSpaceBar += UpdateOccupiedSpaceBar;
        
        UpdateMoneyText();
        UpdateDepthText();
        UpdateScoreText();
        UpdateFuelBar();
        UpdateOccupiedSpaceBar();
    }

    private void UpdateMoneyText()
    {
        money.text = _moneyValue.ToString(CultureInfo.InvariantCulture);
    }
    
    private void UpdateDepthText()
    {
        depth.text = _depthValue.ToString();
    }
    
    private void UpdateScoreText()
    {
        score.text = _scoreValue.ToString();
    }

    private void UpdateFuelBar()
    {
        fuelBar.value = _fuelBarValue;
    }

    private void UpdateOccupiedSpaceBar()
    {
        occupiedSpaceBar.value = _occupiedSpaceBarValue;
    }
}
