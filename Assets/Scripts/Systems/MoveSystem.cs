using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(DrillSystem))]
public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent moveComponent, ref PhysicsVelocity velocity, ref PhysicsMass mass, ref DrillComponent drill, ref Translation translation) =>
        {
            if (drill.IsDrilling) return;

            RestrictPlayerRotation(ref mass);
            MovePlayer(ref velocity, ref moveComponent);
            MoveCamera(ref translation);
        });
    }

    private void RestrictPlayerRotation(ref PhysicsMass mass)
    {
        mass.InverseInertia = new float3(0);
    }

    private void MovePlayer(ref PhysicsVelocity velocity, ref MoveComponent moveComponent)
    {
        var horizontalAxisValue = Input.GetAxis("Horizontal");
        var verticalAxisValue = Input.GetAxis("Vertical");
        
        velocity.Linear += new float3(horizontalAxisValue * moveComponent.Speed,
            verticalAxisValue * moveComponent.HorizontalForce, 0) * Time.DeltaTime;
    }

    private void MoveCamera(ref Translation translation)
    {
        var cameraTransformComponent = Camera.main.transform;
        var cameraPosition = cameraTransformComponent.position;
        cameraTransformComponent.position = new Vector3(cameraPosition.x, translation.Value.y, cameraPosition.z);
    }
}
