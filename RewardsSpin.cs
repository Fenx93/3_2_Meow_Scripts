using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static InventorySettings;

public class RewardsSpin : MonoBehaviour
{
    private Dictionary<ItemQuality, VertexGradient> _itemQualities;

    [SerializeField] private TextMeshProUGUI qualityText, typeText;

    public bool stopped = false;

    private void Awake()
    {
        _itemQualities = new Dictionary<ItemQuality, VertexGradient>();

        foreach (var item in itemQualities)
        {
            VertexGradient textGradient = new VertexGradient(item.Value, item.Value, Color.white, Color.white);
            _itemQualities.Add(item.Key, textGradient);
        }
    }

    void Start()
    {
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

            var types = Enum.GetValues(typeof(CharacterPart));
            var typeInt = UnityEngine.Random.Range(0, types.Length); ;
            var part = (CharacterPart)types.GetValue(typeInt);

            var item = InventorySettings.current.GetInventoryItem(part, quality);
            // TO-DO: remove character path checks
            if (item != null && part != CharacterPart.hat && part != CharacterPart.clothes)
            {
                rotate = false;
                stopped = true;

                qualityText.text = quality.ToString();
                qualityText.colorGradient = _itemQualities[quality];

                typeText.text = part.ToString();
                item.status = ItemStatus.unlocked;
                FinishMatchUI.current.ShowUnlockedItemPanel(true);
                FinishMatchUI.current.ShowRewardsSpin(false);
                FinishMatchUI.current.SetUnlockedItemPanel(item, part);
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
