using System;
using Entities;
using Unity.Collections;
using Unity.Entities;

namespace Systems.GroundSpawnSystem
{
    public class GroundCell
    {
        private readonly GroundGeneratorData _groundGeneratorData;
        private readonly Random _randomSeed;

        public GroundCell(GroundGeneratorData groundGeneratorData)
        {
            _groundGeneratorData = groundGeneratorData;
            _randomSeed = new Random();
        }

        public bool SpawnNewCell(int i, int ySpawnOffset, ref NativeArray<Entity> groundCells)
        {
            var randomFloat = GetRandomFloat();
            string cellName;

            switch (randomFloat)
            {
                case var _ when ySpawnOffset <= _groundGeneratorData.startLayerForEmptyCells &&
                                randomFloat <= _groundGeneratorData.spawnEmptyCellChance:
                    return false;
                case var _ when ySpawnOffset <= _groundGeneratorData.startLayerForLavaCells &&
                                randomFloat <= _groundGeneratorData.spawnLavaCellChance:
                    cellName = "LavaCell";
                    break;
                case var _ when ySpawnOffset <= _groundGeneratorData.startLayerForStonesCells &&
                                randomFloat <= _groundGeneratorData.spawnStoneCellChance:
                    cellName = "StoneCell";
                    break;
                default:
                    cellName = SpawnOre(ySpawnOffset);
                    break;
            }

            if (cellName != "")
                SpawnCell(cellName, i, ref groundCells);
            else
                SpawnCell("GroundCell", i, ref groundCells);

            return true;
        }

        private string SpawnOre(int ySpawnOffset)
        {
            var randomFloat = GetRandomFloat();

            var cellName = "";

            switch (randomFloat)
            {
                case var _ when randomFloat <= _groundGeneratorData.spawnGoldenCellChance &&
                                ySpawnOffset <= _groundGeneratorData.startLayerForGoldenCells:
                    cellName = "GoldenCell";
                    break;
                case var _ when randomFloat <= _groundGeneratorData.spawnSilverCellChance &&
                                ySpawnOffset <= _groundGeneratorData.startLayerForSilverCells:
                    cellName = "SilverCell";
                    break;
                case var _ when randomFloat <= _groundGeneratorData.spawnCoalCellChance &&
                                ySpawnOffset <= _groundGeneratorData.startLayerForCoalCells:
                    cellName = "CoalCell";
                    break;
            }

            return cellName;
        }

        private void SpawnCell(string cellName, int i, ref NativeArray<Entity> groundCells)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.DestroyEntity(groundCells[i]);
            groundCells[i] = entityManager.Instantiate(GameResources.GetGroundCell(cellName));
        }

        private float GetRandomFloat()
        {
            return new Unity.Mathematics.Random((uint) _randomSeed.Next()).NextFloat();
        }
    }
}