using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [Header("Список всех префабов блоков")]
    public List<GameObject> availablePrefabs;

    private string savePath;
    private void Awake()
    {
        Instance = this;

        savePath = Path.Combine(Application.persistentDataPath, "my_level.json");
    }
    public void SaveLevel() 
    {
        LevelSaveData saveData = new LevelSaveData();
        WorldObjectDrag[] objOnScene = FindObjectsByType<WorldObjectDrag>(FindObjectsSortMode.None);
        foreach (WorldObjectDrag obj in objOnScene) 
        {
            string cleanName = obj.gameObject.name.Replace("(Clone)", "").Trim();

            BlockSaveData blockData = new BlockSaveData
            {
                prefabName = cleanName,
                posX = obj.transform.position.x,
                posY = obj.transform.position.y
            };
            saveData.blocks.Add(blockData);
        }
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Уровень успешно сохранен в: {savePath}");

    }
    public void LoadLevel()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Cannot find save file");
            return;
        }
        WorldObjectDrag[] currentObjects = FindObjectsByType<WorldObjectDrag>(FindObjectsSortMode.None);
        foreach (WorldObjectDrag obj in currentObjects)
        {
            Destroy(obj.gameObject);
        }

        string json = File.ReadAllText(savePath);
        LevelSaveData saveData = JsonUtility.FromJson<LevelSaveData>(json);

        foreach (BlockSaveData blockData in saveData.blocks)
        {
            GameObject prefabToSpawn = availablePrefabs.Find(p => p.name == blockData.prefabName);

            if (prefabToSpawn != null)
            {
                Vector3 position = new Vector3(blockData.posX, blockData.posY, 0f);
                Instantiate(prefabToSpawn, position, Quaternion.identity);
            }
            else
            {
                Debug.LogError($"Префаб с именем {blockData.prefabName} не найден в списке LevelManager!");
            }
        }
        Debug.Log("Уровень успешно загружен!");
    }
}
