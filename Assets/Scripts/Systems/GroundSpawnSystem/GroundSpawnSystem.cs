using Systems.GroundSpawnSystem;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class GroundSpawnSystem : MonoBehaviour
{
    [SerializeField] private GroundGeneratorData groundGeneratorData;
    
    private EntityManager _entityManager;
    private GroundCell _groundCell;

    private int _xSpawnOffset;
    private int _ySpawnOffset;

    private void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _groundCell = new GroundCell(groundGeneratorData);
    }

    private void Start()
    {
        var groundCells = new NativeArray<Entity>(groundGeneratorData.maxGroundCellsOnStart, Allocator.Temp);
        _entityManager.Instantiate(new GameObject(), groundCells);

        SpawnGround(groundCells);
        groundCells.Dispose();
    }

    private void SpawnGround(NativeArray<Entity> groundCells)
    {
        for (var i = 0; i < groundCells.Length; i++)
        {
            CheckForNewRow(i);
            
            if (_groundCell.SpawnNewCell(i, _ySpawnOffset, ref groundCells))
            {
                _entityManager.SetComponentData(groundCells[i], new Translation
                {
                    Value = new float3(_xSpawnOffset, _ySpawnOffset, 0)
                });
            }
            
            ++_xSpawnOffset;
        }
    }

    private void CheckForNewRow(int i)
    {
        if (i == 0 || i % groundGeneratorData.maxGroundCellInRowOnStart != 0) return;

        --_ySpawnOffset;
        _xSpawnOffset = 0;
    }
}