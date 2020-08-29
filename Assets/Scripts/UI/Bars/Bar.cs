using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour, IBar
{
    [SerializeField] private Slider bar;
    
    public float MaxValue => bar.maxValue;

    public float CurrentValue => bar.value;

    public void InitBar(float maxValue, float currentValue)
    {
        bar.maxValue = maxValue;
        bar.value = currentValue;
    }

    public void IncreaseValue(float amount)
    {
        bar.value += amount;
    }

    public void DecreaseValue(float amount)
    {
        bar.value -= amount;
    }

    public void Clean()
    {
        bar.value = 0;
    }
}
