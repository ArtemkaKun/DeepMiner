using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public class PhysicsRestrictsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent _, ref PhysicsMass mass) =>
        {
            mass.InverseInertia = new float3(0);
        });
    }
}
