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
            Graphics.DrawMesh(GameResources.GetQuadMesh(), translation.Value, rotation.Value, GameResources.GetCurrentPlayerMaterial(), 1);
        });
    }
}
