using System;
using System.Linq;
using UnityEngine;
using static InventorySettings;

public class EnemyPresets : MonoBehaviour
{
    public TextAsset enemyPresetsDataFile, colorPresetsFile;
    [HideInInspector] public EnemyPresetsData enemyOutfitsData;
    [HideInInspector] public ColorPresetsData colorsData;

    private void Start()
    {
        colorsData = new ColorPresetsData();
        JsonUtility.FromJsonOverwrite(colorPresetsFile.text, colorsData);

        enemyOutfitsData = new EnemyPresetsData();
        JsonUtility.FromJsonOverwrite(enemyPresetsDataFile.text, enemyOutfitsData);

        if (enemyOutfitsData != null && enemyOutfitsData.enemyDatas != null && enemyOutfitsData.enemyDatas.Length > 0)
        {
            var randomPreset = enemyOutfitsData.enemyDatas[UnityEngine.Random.Range(0, enemyOutfitsData.enemyDatas.Length)];

            var mainColorItem = current.GetInventoryItem(CharacterPart.mainColor, randomPreset.mainColorId);

            TabItem mCItem = (mainColorItem != null) ?
                mainColorItem 
                : current.GetRandom(CharacterPart.mainColor);

            EnemyPresetsHolder.mainColor = (mCItem as ColorTabItem).color;

            var secondaryColorItem = current.GetInventoryItem(CharacterPart.secondaryColor, randomPreset.secondaryColorId);

            if (secondaryColorItem != null)
            {
                EnemyPresetsHolder.secondaryColor = (secondaryColorItem as ColorTabItem).color;
            }
            else
            {
                var rand = UnityEngine.Random.Range(0, 3);
                switch (rand)
                {
                    case 0:
                        EnemyPresetsHolder.secondaryColor = Color.white;
                        break;
                    case 1:
                        var temp = colorsData.colors.Where(x => x.mainColor == ColorUtility.ToHtmlStringRGB(EnemyPresetsHolder.mainColor)).FirstOrDefault();
                        EnemyPresetsHolder.secondaryColor = (ColorUtility.TryParseHtmlString("#"+temp.secondaryColor, out Color newCol)) ?
                            newCol : Color.white;
                        break;
                    case 2:
                        EnemyPresetsHolder.secondaryColor = ColorUtility.TryParseHtmlString("#CDCDCD", out Color newCol1)?
                             newCol1 : Color.gray;
                        break;
                }
            }

            var clothesItem = current.GetInventoryItem(CharacterPart.clothes, randomPreset.clothesId);
            if (clothesItem != null)
            {
                EnemyPresetsHolder.clothes = (clothesItem as SpriteTabItem).sprite;
            }
            else if (!string.IsNullOrEmpty(randomPreset.clothesId))
                Debug.LogError("Incorrect : " + randomPreset.clothesId);

            var hatItem = current.GetInventoryItem(CharacterPart.hat, randomPreset.hatId);
            if (hatItem != null)
            {
                EnemyPresetsHolder.hat = (hatItem as SpriteTabItem).sprite;
            }
            else if (!string.IsNullOrEmpty(randomPreset.hatId))
                Debug.LogError("Incorrect : " + randomPreset.hatId);
            
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

[Serializable]
public class ColorPresetsData
{
    public ColorData[] colors;
}

[Serializable]
public class ColorData
{
    public string mainColor, secondaryColor;
}
