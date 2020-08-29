﻿using Systems;
using Entities;
using Entities.Enums;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Math = System.Math;

[UpdateAfter(typeof(DrillSystem))]
[UpdateAfter(typeof(PhysicsRestrictsSystem))]

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent moveComponent, ref PhysicsVelocity velocity, ref DrillComponent drill, ref Rotation rotation) =>
        {
            if (drill.IsDrilling || UI.FuelBar.CurrentValue <= 0f) return;
            
            var horizontalAxisValue = Input.GetAxis("Horizontal");
            var verticalAxisValue = Input.GetAxis("Vertical");
        
            if (Math.Abs(horizontalAxisValue) > 0.01f)
            {
                GameResources.SwitchCurrentPlayerMaterial(PlayerViewPosition.Side);
                rotation.Value = Quaternion.Euler(0, horizontalAxisValue < 0 ? 180 : 0, 0);
            }
            
            if (Math.Abs(horizontalAxisValue) > 0.01f || verticalAxisValue > 0.01f)
            {
                UI.FuelBar.DecreaseValue(moveComponent.FuelConsumption);
            }
            
            velocity.Linear += new float3(horizontalAxisValue * moveComponent.Speed,
                verticalAxisValue > 0 ? verticalAxisValue * moveComponent.FlySpeed : 0, 0) * Time.DeltaTime;
        });
    }
}
