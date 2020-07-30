using Entities;
using Unity.Collections;
using Unity.Entities;
using Random = System.Random;

namespace Systems.GroundSpawnSystem
{
    public class GroundCell
    {
        private EntityManager _entityManager;
        private readonly GroundGeneratorData _groundGeneratorData;
        private readonly Random _randomSeed;

        public GroundCell(GroundGeneratorData groundGeneratorData)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _groundGeneratorData = groundGeneratorData;
            _randomSeed = new Random();
        }
        
        public bool SetEmptyCell(int ySpawnOffset)
        {
            return ySpawnOffset < _groundGeneratorData.startLayerForEmptyCells && GetRandomFloat() < _groundGeneratorData.spawnEmptyCellChance;
        }

        public void TrySpawnStoneCell(int i, int ySpawnOffset, ref NativeArray<Entity> groundCells)
        {
            if (ySpawnOffset < _groundGeneratorData.startLayerForStonesCells && GetRandomFloat() < _groundGeneratorData.spawnStoneCellChance)
            {
                SpawnCell("StoneCell", i, ref groundCells);
            }
        }
    
        public void TrySpawnLavaCell(int i, int ySpawnOffset, ref NativeArray<Entity> groundCells)
        {
            if (ySpawnOffset < _groundGeneratorData.startLayerForLavaCells && GetRandomFloat() < _groundGeneratorData.spawnLavaCellChance)
            {
                SpawnCell("LavaCell", i, ref groundCells);
            }
        }

        public void TrySpawnOre(int i, int ySpawnOffset, ref NativeArray<Entity> groundCells)
        {
            var randomFloat = GetRandomFloat();

            string cellName;
            switch (randomFloat)
            {
                case var _ when randomFloat <= _groundGeneratorData.spawnGoldenCellChance && ySpawnOffset <= _groundGeneratorData.startLayerForGoldenCells:
                    cellName = "GoldenCell";
                    break;
                case var _ when randomFloat <= _groundGeneratorData.spawnSilverCellChance && ySpawnOffset <= _groundGeneratorData.startLayerForSilverCells:
                    cellName = "SilverCell";
                    break;
                case var _ when randomFloat <= _groundGeneratorData.spawnCoalCellChance && ySpawnOffset <= _groundGeneratorData.startLayerForCoalCells:
                    cellName = "CoalCell";
                    break;
                default:
                    return;
            }
            SpawnCell(cellName, i, ref groundCells);
        }

        private void SpawnCell(string cellName, int i, ref NativeArray<Entity> groundCells)
        {
            _entityManager.DestroyEntity(groundCells[i]);
            groundCells[i] = _entityManager.Instantiate(EntitiesManager.GetEntity(cellName));
        }

        private float GetRandomFloat()
        {
            return new Unity.Mathematics.Random((uint) _randomSeed.Next()).NextFloat();
        }
    }
}