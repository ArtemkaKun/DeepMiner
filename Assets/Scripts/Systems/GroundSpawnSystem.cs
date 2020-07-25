using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;

public class GroundSpawnSystem : MonoBehaviour
{
    private const int MaxGroundCells = 540;
    private const int MaxGroundCellInRow = 18;
    private const int StartLayerForEmptyCells = -2;
    private const int StartLayerForStonesCells = -4;
    private const int LayersCountForStoneCellSpawnIncrease = 10;
    private const float MaxStoneCellSpawnChance = 0.8f;
    
    private const float MakeEmptyCellChance = 0.1f;
    private float SpawnStoneCellChance = 0.5f;
    
    private Random _randomSeed;
    private int _xSpawnOffset;
    private int _ySpawnOffset;

    private void Start()
    {
        _randomSeed = new Random();

        var groundCells = new NativeArray<Entity>(MaxGroundCells, Allocator.Temp);
        EntitiesManager.EntityManager.Instantiate(EntitiesManager.GetEntity("GroundCell"), groundCells);

        SpawnGround(groundCells);
        groundCells.Dispose();
    }

    private void SpawnGround(NativeArray<Entity> groundCells)
    {
        for (var i = 0; i < groundCells.Length; i++)
        {
            CheckForNewRow(i);
            CheckForStoneCellSpawnChanceIncrease(i);
            
            if (SetEmptyCell())
            {
                EntitiesManager.EntityManager.DestroyEntity(groundCells[i]);
            }
            else
            {
                SpawnStoneCell(i, ref groundCells);
                EntitiesManager.EntityManager.SetComponentData(groundCells[i], new Translation
                {
                    Value = new float3(_xSpawnOffset, _ySpawnOffset, 0)
                });
            }

            ++_xSpawnOffset;
        }
    }

    private void CheckForNewRow(int i)
    {
        if (i == 0 || i % MaxGroundCellInRow != 0) return;

        --_ySpawnOffset;
        _xSpawnOffset = 0;
    }

    private void CheckForStoneCellSpawnChanceIncrease(int i)
    {
        if (i == 0 || i % LayersCountForStoneCellSpawnIncrease != 0 || SpawnStoneCellChance >= MaxStoneCellSpawnChance) return;
        SpawnStoneCellChance += 0.05f;
    }

    private bool SetEmptyCell()
    {
        var randomFloat = new Unity.Mathematics.Random((uint) _randomSeed.Next()).NextFloat();
        return _ySpawnOffset < StartLayerForEmptyCells && randomFloat < MakeEmptyCellChance;
    }

    private void SpawnStoneCell(int i, ref NativeArray<Entity> groundCells)
    {
        var randomFloat = new Unity.Mathematics.Random((uint) _randomSeed.Next()).NextFloat();

        if (_ySpawnOffset < StartLayerForStonesCells && randomFloat < SpawnStoneCellChance)
        {
            groundCells[i] = EntitiesManager.EntityManager.Instantiate(EntitiesManager.GetEntity("StoneCell"));
        }
    }
}