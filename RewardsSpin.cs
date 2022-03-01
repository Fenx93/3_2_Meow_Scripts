using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static InventorySettings;

public class RewardsSpin : MonoBehaviour
{
    private Dictionary<ItemQuality, VertexGradient> _itemQualities;

    [SerializeField] private TextMeshProUGUI qualityText, typeText;

    public bool stopped = false;

    private Dictionary<CharacterPart, int> partCount;
    private int totalCount = 0;

    private void Awake()
    {
        _itemQualities = new Dictionary<ItemQuality, VertexGradient>();

        foreach (var item in itemQualities)
        {
            VertexGradient textGradient = new VertexGradient(item.Value, item.Value, Color.white, Color.white);
            _itemQualities.Add(item.Key, textGradient);
        }

        partCount = new Dictionary<CharacterPart, int>();
        List<CharacterPart> types = new List<CharacterPart>(Enum.GetValues(typeof(CharacterPart)).Cast<CharacterPart>());
        types.Remove(CharacterPart.eyes);

        foreach (CharacterPart tab in types)
        {
            int count = InventorySettings.current.CountItemsInTab(tab);
            partCount.Add(tab, count);
            totalCount += count;
        }

    }


    void OnEnable()
    {
        print("RewardSpinWas Called!");
        stopped = false;
        AudioController.current.PlayRewardWheelSpinningSound();
        StartCoroutine(nameof(QualitiesSpinning));
        StartCoroutine(nameof(TypesSpinning));
    }

    public void Stop()
    {
        //"randomly" select quality and type
        bool rotate = true;
        while (rotate)
        {

            var qualityInt = UnityEngine.Random.Range(0, _itemQualities.Count);
            List<ItemQuality> keyList = new List<ItemQuality>(_itemQualities.Keys);

            var quality = keyList[qualityInt];

            var typeInt = UnityEngine.Random.Range(0, totalCount);
            int iterateCount = 0;
            CharacterPart selectedpart = CharacterPart.clothes;
            foreach (var part in partCount)
            {
                if (typeInt <= part.Value)
                {
                    selectedpart = part.Key;
                    break;
                }
                else
                {
                    iterateCount += part.Value;
                }
            }

            var item = InventorySettings.current.GetInventoryItem(selectedpart, quality);
           
            if (item != null /*&& selectedpart != CharacterPart.hat && selectedpart != CharacterPart.clothes*/)
            {
                rotate = false;
                stopped = true;

                qualityText.text = quality.ToString();
                qualityText.colorGradient = _itemQualities[quality];

                typeText.text = selectedpart.ToString();
                item.status = ItemStatus.unlocked;

                AudioController.current.PlayRewardWheelStopSpinningSound();

                if (FinishMatchUI.current)
                {
                    FinishMatchUI.current.ShowUnlockedItemPanel(true);
                    FinishMatchUI.current.ShowRewardsSpin(false);
                    FinishMatchUI.current.SetUnlockedItemPanel(item, selectedpart);
                }

                if (RewardsSpinMainMenuUI.current)
                {
                    RewardsSpinMainMenuUI.current.ShowUnlockedItemPanel(true);
                    RewardsSpinMainMenuUI.current.ShowRewardsSpin(false);
                    RewardsSpinMainMenuUI.current.SetUnlockedItemPanel(item, selectedpart);
                }
                AudioController.current.PlayCelebrationSound();
            }
        }
    }

    public IEnumerator QualitiesSpinning()
    {
        while (!stopped)
        {
            foreach (var quality in _itemQualities)
            {
                qualityText.text = quality.Key.ToString();
                qualityText.colorGradient = quality.Value;
                yield return new WaitForSeconds(0.08f);
            }
        }
    }

    public IEnumerator TypesSpinning()
    {
        while (!stopped)
        {
            foreach (var part in Enum.GetValues(typeof(CharacterPart)))
            {
                typeText.text = part.ToString();
                yield return new WaitForSeconds(0.08f);
            }
        }
    }
}
