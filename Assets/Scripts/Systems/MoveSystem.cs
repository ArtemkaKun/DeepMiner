﻿using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Material = UnityEngine.Material;
using Math = System.Math;

[UpdateAfter(typeof(DrillSystem))]
public class MoveSystem : ComponentSystem
{
    private const int SpawnNewLayerOffset = 6;
    
    private GroundSpawnSystem _groundSpawner;
    private Material _currentMaterial;
    private Material _sidePlayerMaterial;
    private Material _frontPlayerMaterial;
    private Mesh _quad;
    
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        
        _groundSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GroundSpawnSystem>();
        _sidePlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterial");
        _frontPlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterialFront");
        
        _currentMaterial = _sidePlayerMaterial;
        
        var quadMesh = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>().sharedMesh;
        _quad = quadMesh;
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref MoveComponent moveComponent, ref PhysicsVelocity velocity, ref PhysicsMass mass, ref DrillComponent drill, ref Translation translation, ref Rotation rotation) =>
        {
            if (drill.IsDrilling) return;
            
            RestrictPlayerRotation(ref mass);
            MovePlayer(ref entity, ref velocity, ref moveComponent, ref rotation, ref translation);
            
            if (translation.Value.y <= _groundSpawner.YSpawnOffset + SpawnNewLayerOffset)
            {
                _groundSpawner.SpawnGround();
            }
        });
    }

    private void RestrictPlayerRotation(ref PhysicsMass mass)
    {
        mass.InverseInertia = new float3(0);
    }

    private void MovePlayer(ref Entity entity, ref PhysicsVelocity velocity, ref MoveComponent moveComponent, ref Rotation rotation, ref Translation translation)
    {
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
                               verticalAxisValue * moveComponent.HorizontalForce, 0) * Time.DeltaTime;
    }
}
