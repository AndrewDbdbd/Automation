using System.Collections.Generic;
[System.Serializable]
public class BlockSaveData
{
    public string prefabName; 
    public float posX;
    public float posY;
}

[System.Serializable]
public class LevelSaveData
{
    public List<BlockSaveData> blocks = new List<BlockSaveData>();
}