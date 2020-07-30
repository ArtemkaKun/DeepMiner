using Systems.GroundSpawnSystem;
using Entities;
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
    private GroundLayer _groundLayer;
    
    private int _xSpawnOffset;
    private int _ySpawnOffset;

    private void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _groundCell = new GroundCell(groundGeneratorData);
        _groundLayer = new GroundLayer(groundGeneratorData);
    }

    private void Start()
    {
        var groundCells = new NativeArray<Entity>(groundGeneratorData.maxGroundCells, Allocator.Temp);
        _entityManager.Instantiate(EntitiesManager.GetEntity("GroundCell"), groundCells);

        SpawnGround(groundCells);
        groundCells.Dispose();
    }

    private void SpawnGround(NativeArray<Entity> groundCells)
    {
        for (var i = 0; i < groundCells.Length; i++)
        {
            CheckForNewRow(i);
            
            _groundLayer.CheckForStoneCellSpawnChanceIncrease(i);
            _groundLayer.CheckForLavaCellSpawnChanceIncrease(i);
                
            if (_groundCell.SetEmptyCell(_ySpawnOffset))
            {
                _entityManager.DestroyEntity(groundCells[i]);
            }
            else
            {
                _groundCell.TrySpawnStoneCell(i, _ySpawnOffset, ref groundCells);
                _groundCell.TrySpawnLavaCell(i, _ySpawnOffset, ref groundCells);
                _groundCell.TrySpawnOre(i, _ySpawnOffset, ref groundCells);
                
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
        if (i == 0 || i % groundGeneratorData.maxGroundCellInRow != 0) return;

        --_ySpawnOffset;
        _xSpawnOffset = 0;
    }
}