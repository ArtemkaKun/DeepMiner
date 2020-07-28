using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Math = System.Math;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent moveComponent, ref PhysicsVelocity velocity, ref PhysicsMass mass) =>
        {
            var horizontalAxisValue = Input.GetAxis("Horizontal");
            var verticalAxisValue = Input.GetAxis("Vertical");

            mass.InverseInertia = new float3(0);
            velocity.Linear += new float3(horizontalAxisValue * moveComponent.Speed , verticalAxisValue * moveComponent.HorizontalForce, 0) * Time.DeltaTime;
        });
    }
}
