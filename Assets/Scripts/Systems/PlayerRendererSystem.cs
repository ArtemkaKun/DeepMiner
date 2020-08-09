using Entities;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveSystem))]
public class PlayerRendererSystem : ComponentSystem
{
    private Mesh _quadMesh;

    protected override void OnCreate()
    {
        base.OnCreate();
        _quadMesh = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>().sharedMesh;
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent _, ref Translation translation, ref Rotation rotation) =>
        {
            Graphics.DrawMesh(_quadMesh, translation.Value, rotation.Value, EntitiesManager.CurrentPlayerMaterial, 1);
        });
    }
}
