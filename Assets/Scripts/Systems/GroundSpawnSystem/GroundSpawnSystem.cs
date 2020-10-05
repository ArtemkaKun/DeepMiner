using Systems.GroundSpawnSystem;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;

public class GroundSpawnSystem : MonoBehaviour
{
    [SerializeField] private GroundGeneratorData groundGeneratorData;
    private EntityManager _entityManager;
    private GroundCell _groundCell;
    private Random _randomSeed;

    private int _xSpawnOffset;
    public int YSpawnOffset { get; private set; }

    private void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _groundCell = new GroundCell(groundGeneratorData);
        _randomSeed = new Random();
    }

    private void Start()
    {
        SpawnGround(true);
    }

    public void SpawnGround(bool startGeneration = false)
    {
        var groundCells =
            new NativeArray<Entity>(
                startGeneration
                    ? groundGeneratorData.maxGroundCellsOnStart
                    : groundGeneratorData.maxGroundCellInRowOnStart, Allocator.Temp);
        var tempObject = new GameObject();

        _entityManager.Instantiate(tempObject, groundCells);

        SpawnGroundCells(groundCells);

        groundCells.Dispose();
        DestroyImmediate(tempObject);
    }

    private void SpawnGroundCells(NativeArray<Entity> groundCells)
    {
        for (var i = 0; i < groundCells.Length; i++)
        {
            CheckForNewRow(i);

            if (_groundCell.SpawnNewCell(i, YSpawnOffset, ref groundCells))
            {
                _entityManager.SetComponentData(groundCells[i], new Translation
                {
                    Value = new float3(_xSpawnOffset, YSpawnOffset, 0)
                });

                _entityManager.SetComponentData(groundCells[i], new Rotation
                {
                    Value = Quaternion.Euler(0, 0, RotateCellInRandomWay())
                });
            }

            ++_xSpawnOffset;
        }

        SetNewRow();
    }

    private void CheckForNewRow(int i)
    {
        if (i == 0 || i % groundGeneratorData.maxGroundCellInRowOnStart != 0) return;

        SetNewRow();
    }

    private void SetNewRow()
    {
        --YSpawnOffset;
        _xSpawnOffset = 0;
    }

    private int RotateCellInRandomWay()
    {
        const int rotateStep = 90;
        var randomInt = new Unity.Mathematics.Random((uint) _randomSeed.Next()).NextInt(4);

        return rotateStep * randomInt;
    }
}