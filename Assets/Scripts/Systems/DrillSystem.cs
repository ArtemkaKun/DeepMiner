using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class DrillSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation Translation, ref DrillComponent drill) =>
        {
            var verticalAxisValue = Input.GetAxis("Vertical");

            if (verticalAxisValue >= 0) return;
            
            var physicWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;

            var ray = new RaycastInput
            {
                Start = Translation.Value,
                End = Translation.Value + new float3(0, -1, 0),
                Filter = new CollisionFilter
                {
                    BelongsTo = 2u,
                    CollidesWith = 1u,
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(ray, out var hit))
            {
                Debug.Log(hit.Entity.Index);
            }
        });
    }
}
