using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GroundSpawnSystem : MonoBehaviour
{
    private const int MaxGroundCells = 540;
    private const int MaxGroundCellInRow = 18;
    
    private void Start()
    {
        var groundCells = new NativeArray<Entity>(MaxGroundCells, Allocator.Temp);
        EntitiesManager.EntityManager.Instantiate(EntitiesManager.GetEntity("GroundCell"), groundCells);

        var xSpawnOffset = 0;
        var ySpawnOffset = 0;
        for (var i = 0; i < groundCells.Length; i++)
        {
            if (i != 0 && i % MaxGroundCellInRow == 0)
            {
                --ySpawnOffset;
                xSpawnOffset = 0;
            }
            
            EntitiesManager.EntityManager.SetComponentData(groundCells[i], new Translation
            {
                Value = new float3(xSpawnOffset, ySpawnOffset, 0)
            });
            
            ++xSpawnOffset;
        }

        groundCells.Dispose();
    }
}
