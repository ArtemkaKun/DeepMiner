using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour, IBar
{
    [SerializeField] private Slider bar;
    
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }

    public void InitBar(float maxValue, float currentValue)
    {
        MaxValue = maxValue;
        CurrentValue = currentValue;

        bar.maxValue = maxValue;
        bar.value = currentValue;
    }

    public void IncreaseValue(float amount)
    {
        if (CurrentValue + amount >= MaxValue)
        {
            CurrentValue = MaxValue;
        }
        else
        {
            CurrentValue += amount;
        }

        bar.value = CurrentValue;
    }

    public void DecreaseValue(float amount)
    {
        if (CurrentValue - amount <= 0)
        {
            CurrentValue = 0;
        }
        else
        {
            CurrentValue -= amount;
        }
        
        bar.value = CurrentValue;
    }

    public void Clean()
    {
        CurrentValue = 0;
        bar.value = CurrentValue;
    }

    public void Fill()
    {
        CurrentValue = MaxValue;
        bar.value = CurrentValue;
    }
}
