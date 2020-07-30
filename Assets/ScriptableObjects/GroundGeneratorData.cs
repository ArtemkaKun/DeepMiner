using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GeneratorData", order = 1)]
public class GroundGeneratorData : ScriptableObject
{
    public int maxGroundCellsOnStart = 540;
    public int maxGroundCellInRowOnStart = 18;
    
    public int startLayerForEmptyCells = -2;
    public int startLayerForStonesCells = -4;
    public int startLayerForLavaCells = -19;
    public int startLayerForCoalCells = -2;
    public int startLayerForSilverCells = -4;
    public int startLayerForGoldenCells = -8;

    public float spawnEmptyCellChance = 0.1f;
    public float spawnCoalCellChance = 0.2f;
    public float spawnSilverCellChance = 0.15f;
    public float spawnGoldenCellChance = 0.1f;
    public float spawnStoneCellChance = 0.5f;
    public float spawnLavaCellChance = 0.1f;
}
