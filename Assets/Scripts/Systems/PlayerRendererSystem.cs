using Entities;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveSystem))]
public class PlayerRendererSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent _, ref Translation translation, ref Rotation rotation) =>
        {
            Graphics.DrawMesh(EntitiesManager.quad, translation.Value, rotation.Value, EntitiesManager.CurrentPlayerMaterial, 1);
        });
    }
}
