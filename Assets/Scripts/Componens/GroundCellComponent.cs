using Unity.Entities;

[GenerateAuthoringComponent]
public struct GroundCellComponent : IComponentData
{
    public float Durability;
    public float Weight;
    public float Cost;
}
