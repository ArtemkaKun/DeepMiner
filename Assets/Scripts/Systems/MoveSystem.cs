using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Math = System.Math;

[UpdateAfter(typeof(DrillSystem))]
public class MoveSystem : ComponentSystem
{
    private const int SpawnNewLayerOffset = 6;
    private GroundSpawnSystem _groundSpawner;
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        
        _groundSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GroundSpawnSystem>();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent moveComponent, ref PhysicsVelocity velocity, ref PhysicsMass mass, ref DrillComponent drill, ref Translation translation, ref Rotation rotation) =>
        {
            if (drill.IsDrilling) return;
            
            RestrictPlayerRotation(ref mass);
            MovePlayer(ref velocity, ref moveComponent, ref rotation);
            
            if (translation.Value.y <= _groundSpawner.YSpawnOffset + SpawnNewLayerOffset)
            {
                _groundSpawner.SpawnGround();
            }
            
            MoveCamera(ref translation);
        });
    }

    private void RestrictPlayerRotation(ref PhysicsMass mass)
    {
        mass.InverseInertia = new float3(0);
    }

    private void MovePlayer(ref PhysicsVelocity velocity, ref MoveComponent moveComponent, ref Rotation rotation)
    {
        var horizontalAxisValue = Input.GetAxis("Horizontal");
        var verticalAxisValue = Input.GetAxis("Vertical");

        if (Math.Abs(horizontalAxisValue) > 0.01f)
        {
            rotation.Value = Quaternion.Euler(0, horizontalAxisValue < 0 ? 180 : 0, 0);
        }

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
