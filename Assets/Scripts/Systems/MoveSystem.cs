using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Material = UnityEngine.Material;
using Math = System.Math;

[UpdateAfter(typeof(DrillSystem))]
[UpdateAfter(typeof(PhysicsRestrictsSystem))]

public class MoveSystem : ComponentSystem
{
    private Material _currentMaterial;
    private Material _sidePlayerMaterial;
    private Material _frontPlayerMaterial;
    private Mesh _quad;
    
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        
        _sidePlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterial");
        _frontPlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterialFront");
        
        _currentMaterial = _sidePlayerMaterial;
        
        var quadMesh = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>().sharedMesh;
        _quad = quadMesh;
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent moveComponent, ref PhysicsVelocity velocity, ref DrillComponent drill, ref Translation translation, ref Rotation rotation) =>
        {
            if (drill.IsDrilling) return;
            
            var horizontalAxisValue = Input.GetAxis("Horizontal");
            var verticalAxisValue = Input.GetAxis("Vertical");
        
            if (Math.Abs(horizontalAxisValue) > 0.01f)
            {
                _currentMaterial = _sidePlayerMaterial;
                rotation.Value = Quaternion.Euler(0, horizontalAxisValue < 0 ? 180 : 0, 0);
            } 
            else if (verticalAxisValue < 0f)
            {
                _currentMaterial = _frontPlayerMaterial;
            }

            Graphics.DrawMesh(_quad, translation.Value, rotation.Value, _currentMaterial, 0);
        
            velocity.Linear += new float3(horizontalAxisValue * moveComponent.Speed,
                verticalAxisValue > 0 ? verticalAxisValue * moveComponent.HorizontalForce : 0, 0) * Time.DeltaTime;
        });
    }
}
