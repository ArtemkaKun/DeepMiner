using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation, ref MoveComponent moveComponent) =>
        {
            var horizontalAxisValue = Input.GetAxis("Horizontal");
            
            if (Math.Abs(horizontalAxisValue) > 0.001f)
            {
                translation.Value.x += horizontalAxisValue * moveComponent.Speed * Time.DeltaTime;
            }
        });
    }
}
