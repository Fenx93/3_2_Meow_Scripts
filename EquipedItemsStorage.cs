using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InventorySettings;

public class EquipedItemsStorage : MonoBehaviour
{
    public static EquipedItemsStorage Instance;
    private readonly Dictionary<CharacterPart, string> selectedItems
        = new Dictionary<CharacterPart, string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetItem(CharacterPart part, string id)
    {
        if (selectedItems.ContainsKey(part))
        {
            selectedItems[part] = id;
        }
        else
        {
            selectedItems.Add(part, id);
        }
    }


    public void SetEquipedCharacterItems()
    {
        var itemParts = new CharacterPart[] { CharacterPart.clothes, CharacterPart.hat };
        var colorParts = new CharacterPart[] { CharacterPart.mainColor, CharacterPart.secondaryColor };

        if (selectedItems != null && selectedItems.Any())
        {
            foreach (var pair in selectedItems)
            {
                var part = pair.Key;
                if (itemParts.Contains(part))
                {
                    var item = (SpriteTabItem)InventorySettings.Instance.GetItemByID(pair.Key, pair.Value);
                    InventorySettings.SelectItem(item.sprite, part, item.GetID(), true);
                }
                else if (colorParts.Contains(part))
                {
                    var item = (ColorTabItem)InventorySettings.Instance.GetItemByID(pair.Key, pair.Value);
                    InventorySettings.SelectColor(item.color, part, item.GetID(), true);
                }
            }
        }
        else
        {
            //get random unlocked items
            foreach (var part in itemParts)
            {
                var item = (SpriteTabItem)InventorySettings.Instance.GetRandomUnlockedItem(part);
                InventorySettings.SelectItem(item.sprite, part, item.GetID());
            }
            foreach (var part in colorParts)
            {
                var item = (ColorTabItem)InventorySettings.Instance.GetRandomUnlockedItem(part);
                InventorySettings.SelectColor(item.color, part, item.GetID());
            }
        }
    }


    public void LoadSelectedItems(StorableItem[] items)
    {
        if (items != null)
        {
            foreach (StorableItem item in items)
            {
                SetItem(item.part, item.id);
            }
        }
    }

    public StorableItem[] GetStorableItems()
        => selectedItems.Select(x => new StorableItem(x.Key, x.Value, ItemStatus.unlocked)).ToArray();
}

