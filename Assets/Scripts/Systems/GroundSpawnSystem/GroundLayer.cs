namespace Systems.GroundSpawnSystem
{
    public class GroundLayer
    {
        private readonly GroundGeneratorData _groundGeneratorData;
        
        public GroundLayer(GroundGeneratorData groundGeneratorData)
        {
            _groundGeneratorData = groundGeneratorData;
        }
        
        public void CheckForStoneCellSpawnChanceIncrease(int i)
        {
            if (i == 0 || i % _groundGeneratorData.layersForStoneCellSpawnIncrease != 0 || _groundGeneratorData.spawnStoneCellChance >= _groundGeneratorData.maxStoneCellSpawnChance) return;
            _groundGeneratorData.spawnStoneCellChance += 0.05f;
        }
        
        public void CheckForLavaCellSpawnChanceIncrease(int i)
        {
            if (i == 0 || i % _groundGeneratorData.layersForLavaCellSpawnIncrease != 0 || _groundGeneratorData.spawnLavaCellChance >= _groundGeneratorData.maxLavaCellSpawnChance) return;
            _groundGeneratorData.spawnLavaCellChance += 0.05f;
        }
    }
}