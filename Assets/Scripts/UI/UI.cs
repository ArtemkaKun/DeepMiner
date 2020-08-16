﻿using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text depth;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Bar fuelBarRef;
    [SerializeField] private Bar occupiedSpaceBarRef;

    public static Bar FuelBar;
    public static Bar OccupiedSpaceBar;
    
    private static float _moneyValue;
    private static int _depthValue;
    private static int _scoreValue;

    private static Action _updateMoneyText;
    private static Action _updateDepthText;
    private static Action _updateScoreText;

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

    private void Awake()
    {
        _moneyValue = 0;
        _depthValue = 0;
        _scoreValue = 0;

        InitBars();

        _updateMoneyText += UpdateMoneyText;
        _updateDepthText += UpdateDepthText;
        _updateScoreText += UpdateScoreText;

        UpdateMoneyText();
        UpdateDepthText();
        UpdateScoreText();
    }

    private void InitBars()
    {
        FuelBar = fuelBarRef;
        OccupiedSpaceBar = occupiedSpaceBarRef;

        FuelBar.InitBar(100, 100);
        OccupiedSpaceBar.InitBar(200, 0);
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
}
