using System;
using TMPro;
using UniRx;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static Action onGameOver;

    private static readonly (int MaxValue, int Value) StartFuelBarValues = (100, 100);
    private static readonly (int MaxValue, int Value) StartStorageBarValues = (200, 0);

    private static float _moneyValue;
    private static int _depthValue;
    private static int _scoreValue;

    private static Action _updateMoneyText;
    private static Action _updateDepthText;
    private static Action _updateScoreText;
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text depth;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Bar fuelBarObject;
    [SerializeField] private Bar storageBarObject;
    [SerializeField] private Transform gameOverText;

    public static IBar FuelBar { get; private set; }
    public static IBar StorageBar { get; private set; }

    private void Awake()
    {
        InitTextValues();
        InitBars();
        InitUIActions();
        
        InitializeButtonHandlers();
    }

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

    private void InitTextValues()
    {
        _moneyValue = 0;
        _depthValue = 0;
        _scoreValue = 0;

        UpdateMoneyText();
        UpdateDepthText();
        UpdateScoreText();
    }

    private void InitBars()
    {
        FuelBar = fuelBarObject;
        StorageBar = storageBarObject;

        FuelBar.InitBar(StartFuelBarValues.MaxValue, StartFuelBarValues.Value);
        StorageBar.InitBar(StartStorageBarValues.MaxValue, StartStorageBarValues.Value);
    }

    private void InitUIActions()
    {
        _updateMoneyText += UpdateMoneyText;
        _updateDepthText += UpdateDepthText;
        _updateScoreText += UpdateScoreText;

        onGameOver += TurnOnGameOverText;
    }

    private void UpdateMoneyText()
    {
        money.text = _moneyValue.ToString("N");
    }

    private void UpdateDepthText()
    {
        depth.text = _depthValue.ToString();
    }

    private void UpdateScoreText()
    {
        score.text = _scoreValue.ToString();
    }

    private void TurnOnGameOverText()
    {
        gameOverText.gameObject.SetActive(true);
    }

    private void InitializeButtonHandlers()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Escape))
            .Subscribe(_ => Application.Quit());
    }
}