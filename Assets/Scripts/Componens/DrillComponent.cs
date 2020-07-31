using Unity.Entities;

[GenerateAuthoringComponent]
public struct DrillComponent : IComponentData
{
    public float DrillPower;
    public bool IsDrilling;
}
