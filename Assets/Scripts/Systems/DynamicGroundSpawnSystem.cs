using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveSystem))]
public class DynamicGroundSpawnSystem : ComponentSystem
{
    private const int SpawnNewLayerOffset = 6;
    
    private GroundSpawnSystem _groundSpawner;
    
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        
        _groundSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GroundSpawnSystem>();
    }
    
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MoveComponent _, ref Translation translation) =>
        {
            if (translation.Value.y <= _groundSpawner.YSpawnOffset + SpawnNewLayerOffset)
            {
                _groundSpawner.SpawnGround();
            }
        });
    }
}
