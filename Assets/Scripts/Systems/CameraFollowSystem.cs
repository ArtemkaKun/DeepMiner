using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveSystem))]
public class CameraFollowSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent _, ref Translation translation) =>
        {
            var cameraTransformComponent = Camera.main.transform;
            var cameraPosition = cameraTransformComponent.position;
            
            cameraTransformComponent.position = new Vector3(cameraPosition.x, translation.Value.y, cameraPosition.z);
        });
    }
}
