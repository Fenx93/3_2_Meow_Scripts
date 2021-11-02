using System;
using UnityEngine;

public class EnemyPresets : MonoBehaviour
{
    public TextAsset testFile;
    [HideInInspector] public EnemyPresetsData data;

    private void Start()
    {
        data = new EnemyPresetsData();
        JsonUtility.FromJsonOverwrite(testFile.text, data);
        if (data != null && data.enemyDatas != null && data.enemyDatas.Length > 0)
        {
            var randomPreset = data.enemyDatas[UnityEngine.Random.Range(0, data.enemyDatas.Length)];
            EnemyPresetsHolder.mainColor = (InventorySettings.current.GetInventoryItem(InventorySettings.CharacterPart.mainColor, randomPreset.mainColorId) as ColorTabItem).color;
            EnemyPresetsHolder.secondaryColor = (InventorySettings.current.GetInventoryItem(InventorySettings.CharacterPart.secondaryColor, randomPreset.secondaryColorId) as ColorTabItem).color;
            EnemyPresetsHolder.clothes = (InventorySettings.current.GetInventoryItem(InventorySettings.CharacterPart.clothes, randomPreset.clothesId) as SpriteTabItem).sprite;
            EnemyPresetsHolder.hat = (InventorySettings.current.GetInventoryItem(InventorySettings.CharacterPart.hat, randomPreset.hatId) as SpriteTabItem).sprite;
        }
    }


}

public static class EnemyPresetsHolder
{
    public static Color mainColor, secondaryColor;
    public static Sprite clothes, hat;
}

    [Serializable]
public class EnemyPresetsData
{
    public EnemyData[] enemyDatas;
}

[Serializable]
public class EnemyData
{
    public string mainColorId, secondaryColorId, hatId, clothesId;
}
