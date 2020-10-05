using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveComponent : IComponentData
{
    public float Speed;
    public float FlySpeed;
    public float FuelConsumption;
}