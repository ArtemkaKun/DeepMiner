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
    private const int StartLayerForLavaCells = -19;
    private const int StartLayerForCoalCells = -2;
    
    private const int LayersCountForStoneCellSpawnIncrease = 10;
    private const int LayersCountForLavaCellSpawnIncrease = 20;

    private const float MakeEmptyCellChance = 0.1f;
    private const float MaxStoneCellSpawnChance = 0.8f;
    private const float MaxLavaCellSpawnChance = 0.4f;

    private const float SpawnCoalCellChance = 0.2f;
    private float SpawnStoneCellChance = 0.5f;
    private float SpawnLavaCellChance = 0.1f;
    
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
            CheckForLavaCellSpawnChanceIncrease(i);
                
            if (SetEmptyCell())
            {
                EntitiesManager.EntityManager.DestroyEntity(groundCells[i]);
            }
            else
            {
                SpawnStoneCell(i, ref groundCells);
                SpawnLavaCell(i, ref groundCells);
                
                SpawnOre(i, ref groundCells);
                
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
    
    private void CheckForLavaCellSpawnChanceIncrease(int i)
    {
        if (i == 0 || i % LayersCountForLavaCellSpawnIncrease != 0 || SpawnLavaCellChance >= MaxLavaCellSpawnChance) return;
        SpawnLavaCellChance += 0.05f;
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
            EntitiesManager.EntityManager.DestroyEntity(groundCells[i]);
            groundCells[i] = EntitiesManager.EntityManager.Instantiate(EntitiesManager.GetEntity("StoneCell"));
        }
    }
    
    private void SpawnLavaCell(int i, ref NativeArray<Entity> groundCells)
    {
        var randomFloat = new Unity.Mathematics.Random((uint) _randomSeed.Next()).NextFloat();

        if (_ySpawnOffset < StartLayerForLavaCells && randomFloat < SpawnLavaCellChance)
        {
            EntitiesManager.EntityManager.DestroyEntity(groundCells[i]);
            groundCells[i] = EntitiesManager.EntityManager.Instantiate(EntitiesManager.GetEntity("LavaCell"));
        }
    }

    private void SpawnOre(int i, ref NativeArray<Entity> groundCells)
    {
        var randomFloat = new Unity.Mathematics.Random((uint) _randomSeed.Next()).NextFloat();

        switch (randomFloat)
        {
            case var _ when randomFloat <= SpawnCoalCellChance:
                SpawnCoal(i, ref groundCells);
                break;
        }
    }

    private void SpawnCoal(int i, ref NativeArray<Entity> groundCells)
    {
        if (_ySpawnOffset < StartLayerForCoalCells)
        {
            EntitiesManager.EntityManager.DestroyEntity(groundCells[i]);
            groundCells[i] = EntitiesManager.EntityManager.Instantiate(EntitiesManager.GetEntity("CoalCell"));
        }
    }
}