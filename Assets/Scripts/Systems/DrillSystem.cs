using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

public class DrillSystem : ComponentSystem
{
    private const float RayLength = 1;
    private CollisionFilter _groundCollisionFilter;

    protected override void OnCreate()
    {
        base.OnCreate();

        _groundCollisionFilter = new CollisionFilter
        {
            BelongsTo = 2u,
            CollidesWith = 1u,
            GroupIndex = 0
        };
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation, ref DrillComponent drill) =>
        {
            var verticalAxisValue = Input.GetAxis("Vertical");

            if (verticalAxisValue >= 0) return;

            var hitGroundCell = CastRayToGround(translation.Value);
            if (hitGroundCell == Entity.Null) return;
            
            var translationComponentFromEntity = GetComponentDataFromEntity<Translation>(true);
            var groundCellTranslation = translationComponentFromEntity[hitGroundCell];
            translation.Value = new float3(groundCellTranslation.Value.x, translation.Value.y, 0);

            var waitTime = 1f;
            do
            {
                waitTime -= Time.DeltaTime;
            } while (waitTime <= 0);
            
            EntitiesManager.EntityManager.DestroyEntity(hitGroundCell);
        });
    }

    private Entity CastRayToGround(float3 playerPosition)
    {
        var physicWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;

        var ray = new RaycastInput
        {
            Start = playerPosition,
            End = playerPosition + new float3(0, -RayLength, 0),
            Filter = _groundCollisionFilter
        };

        collisionWorld.CastRay(ray, out var hit);
        return hit.Entity;
    }
}
