public interface IBar
{
    float MaxValue { get; }
    float CurrentValue { get; }

    void InitBar(float maxValue, float currentValue);
    void IncreaseValue(float amount);
    void DecreaseValue(float amount);
    void Clean();
}
