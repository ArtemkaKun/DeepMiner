﻿using Componens;
using Entities;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class ShopSystemButton : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref ShopComponent shopComponent) =>
        {
            shopComponent.ShowShopButton = false;
        });
    }
}

[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateAfter(typeof(ShopSystemButton))]
public class ShopSystem : JobComponentSystem
{
    private BuildPhysicsWorld _physicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;

    protected override void OnCreate()
    {
        base.OnCreate();

        _physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var triggerJob = new ShopTriggerJob
        {
            ShopComponentGroup = GetComponentDataFromEntity<ShopComponent>()
        }.Schedule(_stepPhysicsWorld.Simulation, ref _physicsWorld.PhysicsWorld, inputDeps);

        return triggerJob;
    }
    
    [BurstCompile]
    private struct ShopTriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<ShopComponent> ShopComponentGroup;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            var entityAIsShop = ShopComponentGroup.HasComponent(entityA);
            var entityBIsShop = ShopComponentGroup.HasComponent(entityB);

            if (entityAIsShop || entityBIsShop)
            {
                var shop = entityAIsShop ? entityA : entityB;
                
                var shopComponent = ShopComponentGroup[shop];
                shopComponent.ShowShopButton = true;
                ShopComponentGroup[shop] = shopComponent;
            }
        }
    }
}

[UpdateAfter(typeof(ShopSystem))]
public class ShopButtonRender : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref ShopComponent shopComponent) =>
        {
            if (shopComponent.ShowShopButton)
            {
                Entities.ForEach((ref ShopButton _, ref Translation translation, ref Rotation rotation) =>
                {
                    Graphics.DrawMesh(EntitiesManager.quad, translation.Value, rotation.Value, EntitiesManager.ShopButtonMaterial, 1);
                });
            }
        });
    }
}