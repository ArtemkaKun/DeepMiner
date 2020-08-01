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
        private const float RayLength = 0.6f;
        private const int DrillDelay = 3;
        
        private CollisionFilter _groundCollisionFilter;
        private Entity _cellToDrill;
        
        private float _drillSpeedInSeconds = 3;
        private float _timeBuffer;
        
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
                if (drill.IsDrilling)
                {
                    _timeBuffer += Time.DeltaTime;
                    if (_timeBuffer < _drillSpeedInSeconds) return;
                    
                    CompleteDrillAction(ref drill);
                }
                else
                {
                    var horizontalAxisValue = Input.GetAxis("Horizontal");
                    var verticalAxisValue = Input.GetAxis("Vertical");
                    
                    if (verticalAxisValue > 0 || !Input.GetButton("B Button")) return;
                    
                    var axisValues = new float2(horizontalAxisValue, verticalAxisValue);
                    if (TryDrillByZ(axisValues, ref translation, drill.DrillPower) || TryDrillByX(axisValues, ref translation, drill.DrillPower))
                    {
                        drill.IsDrilling = true;
                    }
                }
            });
        }

        private void CompleteDrillAction(ref DrillComponent drill)
        {
            _timeBuffer = 0;
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(_cellToDrill);
            _cellToDrill = Entity.Null;
            drill.IsDrilling = false;
        }

        private bool TryDrillByZ(float2 axisValues, ref Translation translation, float drillPower)
        {
            if (axisValues.y < 0 && Math.Abs(axisValues.x) < 0.5f)
            {
                _cellToDrill = CastRay(translation.Value, new float3(0, -RayLength, 0));
            }
            
            if (_cellToDrill == Entity.Null || !CheckAbilityToDrillCell(_cellToDrill, drillPower)) return false;

            AdjustPlayerPosition(_cellToDrill, ref translation);
            CalculateDrillSpeed(_cellToDrill, drillPower);
            
            return true;
        }
        
        private bool TryDrillByX(float2 axisValues, ref Translation translation, float drillPower)
        {
            if (Math.Abs(axisValues.y) < 0.5f && Math.Abs(axisValues.x) > 0.001f)
            {
                _cellToDrill = CastRay(translation.Value, new float3(Mathf.Sign(axisValues.x) * RayLength, 0, 0));
            }

            CalculateDrillSpeed(_cellToDrill, drillPower);
            
            return _cellToDrill != Entity.Null && CheckAbilityToDrillCell(_cellToDrill, drillPower);
        }

        private bool CheckAbilityToDrillCell(Entity cell, float drillPower)
        {
            return drillPower >= GetGroundCellComponent(cell).Durability;
        }

        private void CalculateDrillSpeed(Entity cell, float drillPower)
        {
            _drillSpeedInSeconds = GetGroundCellComponent(cell).Durability / drillPower * DrillDelay;
        }

        private GroundCellComponent GetGroundCellComponent(Entity cell)
        {
            var cellComponentFromEntity = GetComponentDataFromEntity<GroundCellComponent>(true);
            return cellComponentFromEntity[cell];
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
