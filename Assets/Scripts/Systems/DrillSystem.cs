using Entities;
using Entities.Enums;
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
            Entities.ForEach((ref Translation translation, ref DrillComponent drill,
                ref PlayerComponent playerComponent) =>
            {
                if (UI.FuelBar.CurrentValue <= 0f) return;

                if (drill.IsDrilling)
                {
                    _timeBuffer += Time.DeltaTime;
                    if (_timeBuffer < _drillSpeedInSeconds) return;

                    CompleteDrillAction(ref drill, ref playerComponent);
                }
                else
                {
                    var axisValues = GetInputAxisValues();

                    if (axisValues.VerticalAxis > 0 || !Input.GetButton("B Button")) return;
                    if (Mathf.Abs(axisValues.HorizontalAxis) < 0.01f && Mathf.Abs(axisValues.VerticalAxis) < 0.01f) return;
                    
                    if (!TryDrillByZ(axisValues, ref translation, drill.DrillPower) &&
                        !TryDrillByX(axisValues, ref translation, drill.DrillPower)) return;

                    drill.IsDrilling = true;
                    UI.FuelBar.DecreaseValue(drill.FuelConsumption);
                }
            });
        }

        private void CompleteDrillAction(ref DrillComponent drill, ref PlayerComponent playerComponent)
        {
            _timeBuffer = 0;

            var cellComponent = GetGroundCellComponent(_cellToDrill);

            playerComponent.TempMoney += cellComponent.Cost;

            UpdateUI(cellComponent);

            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(_cellToDrill);
            _cellToDrill = Entity.Null;

            drill.IsDrilling = false;
        }

        private (float HorizontalAxis, float VerticalAxis) GetInputAxisValues()
        {
            return (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        private void UpdateUI(GroundCellComponent cellComponent)
        {
            UI.IncreaseScore(cellComponent.ScorePoints);
            UI.StorageBar.IncreaseValue(cellComponent.Weight);
        }

        private bool TryDrillByZ((float HorizontalAxis, float VerticalAxis) axisValues, ref Translation translation, float drillPower)
        {
            var (horizontalAxis, verticalAxis) = axisValues;
            if (Mathf.Abs(verticalAxis) < 0.01f || Math.Abs(horizontalAxis) > 0.5f) return false;

            _cellToDrill = CastRay(translation.Value, new float3(0, -RayLength, 0));

            if (_cellToDrill == Entity.Null || !CheckAbilityToDrillCell(_cellToDrill, drillPower)) return false;

            AdjustPlayerPosition(_cellToDrill, ref translation);
            GameResources.SwitchCurrentPlayerMaterial(PlayerViewPosition.Front);
            CalculateDrillSpeed(_cellToDrill, drillPower);

            return true;
        }

        private bool TryDrillByX((float HorizontalAxis, float VerticalAxis) axisValues, ref Translation translation, float drillPower)
        {
            var (horizontalAxis, verticalAxis) = axisValues;
            if (Mathf.Abs(horizontalAxis) < 0.01f || Math.Abs(verticalAxis) > 0.5f) return false;
            
            _cellToDrill = CastRay(translation.Value, new float3(Mathf.Sign(horizontalAxis) * RayLength, 0, 0));

            if (_cellToDrill == Entity.Null || !CheckAbilityToDrillCell(_cellToDrill, drillPower)) return false;

            CalculateDrillSpeed(_cellToDrill, drillPower);

            return _cellToDrill != Entity.Null && CheckAbilityToDrillCell(_cellToDrill, drillPower);
        }

        private bool CheckAbilityToDrillCell(Entity cell, float drillPower)
        {
            var cellComponent = GetGroundCellComponent(cell);
            return drillPower >= cellComponent.Durability &&
                   UI.StorageBar.CurrentValue + cellComponent.Weight <= UI.StorageBar.MaxValue;
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