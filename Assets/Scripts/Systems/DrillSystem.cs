using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Math = System.Math;

namespace Systems
{
    public class DrillSystem : ComponentSystem
    {
        private const float RayLength = 1;
        private const float DrillDelay = 3;
        
        private CollisionFilter _groundCollisionFilter;
        private EntityManager _entityManager;
        private bool _isDrill;
        private Entity _cellToDrill;
        private float _timeBuffer;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
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
                if (drill.IsDrilling)
                {
                    _timeBuffer += Time.DeltaTime;

                    if (_timeBuffer < DrillDelay) return;

                    CompleteDrillAction(ref drill);
                }
                else
                {
                    var horizontalAxisValue = Input.GetAxis("Horizontal");
                    var verticalAxisValue = Input.GetAxis("Vertical");

                    if (verticalAxisValue > 0) return;

                    if (TryDrillByZ(verticalAxisValue, horizontalAxisValue, ref translation) ||
                        TryDrillByX(verticalAxisValue, horizontalAxisValue, ref translation))
                    {
                        drill.IsDrilling = true;
                    }
                }
            });
        }

        private void CompleteDrillAction(ref DrillComponent drill)
        {
            _timeBuffer = 0;
            _entityManager.DestroyEntity(_cellToDrill);
            _cellToDrill = Entity.Null;
            drill.IsDrilling = false;
        }

        private bool TryDrillByZ(float verticalAxisValue, float horizontalAxisValue, ref Translation translation)
        {
            if (verticalAxisValue < 0 && Math.Abs(horizontalAxisValue) < 0.001f)
            {
                _cellToDrill = CastRay(translation.Value, new float3(0, -RayLength, 0));
            }
            
            if (_cellToDrill == Entity.Null) return false;

            AdjustPlayerPosition(_cellToDrill, ref translation);
            
            return true;
        }
        
        private bool TryDrillByX(float verticalAxisValue, float horizontalAxisValue, ref Translation translation)
        {
            if (Math.Abs(verticalAxisValue) < 0.001f && Math.Abs(horizontalAxisValue) > 0.001f)
            {
                _cellToDrill = CastRay(translation.Value, new float3(Mathf.Sign(horizontalAxisValue) * (RayLength - 0.5f), 0, 0));
            }
            
            return _cellToDrill != Entity.Null;
        }

        private Entity CastRay(float3 playerPosition, float3 castVector)
        {
            var physicWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;

            var ray = new RaycastInput
            {
                Start = playerPosition,
                End = playerPosition + castVector,
                Filter = _groundCollisionFilter
            };

            collisionWorld.CastRay(ray, out var hit);
            return hit.Entity;
        }

        private void AdjustPlayerPosition(Entity hitGroundCell, ref Translation translation)
        {
            var translationComponentFromEntity = GetComponentDataFromEntity<Translation>(true);
            var groundCellTranslation = translationComponentFromEntity[hitGroundCell];
            
            translation.Value = new float3(groundCellTranslation.Value.x, translation.Value.y, 0);
        }
    }
}
