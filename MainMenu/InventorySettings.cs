using System;
using System.Linq;
using UnityEngine;

public class InventorySettings : MonoBehaviour
{
    public enum ItemQuality { common, rare, epic, legendary };
    public enum CharacterPart { mainColor, secondaryColor, hat, clothes, eyes }

    [Serializable] public class MyDictionary2 : SerializableDictionary<ItemQuality, Color> { }
    [SerializeField] private MyDictionary2 _itemQualities;
    public static MyDictionary2 itemQualities;

    [SerializeField] private Tab[] tabs;
    public static Tab[] Tabs;

    public static InventorySettings Instance;

    private System.Random rand;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        rand = new System.Random();
        itemQualities = _itemQualities;
        if (Tabs == null)
            Tabs = tabs;
    }

    public void LoadItemUnlocks(StorableItem[] storableTabs)
    {
        storableTabs.Select(x => GetInventoryItem(x.part, x.id).status = x.status);
    }

    public int CountItemsInTab(CharacterPart part)
        => GetTabByPart(part).items.Length;

    public void UnlockItem(TabItem item, CharacterPart part)
        => GetTabByPart(part).items.Where(x => x.quality == item.quality);

    public TabItem GetRandomInventoryItem(CharacterPart part, ItemQuality quality)
    {
        Tab selectedTab = GetTabByPart(part);
        var items = selectedTab.items.Where(x => x.quality == quality);
        if (items.Count() > 0)
        {
            var rand = new System.Random();
            var item = items.ElementAt(rand.Next(items.Count()));
            return item;
        }
        return null;
    }
    public TabItem GetInventoryItem(CharacterPart part, string id)
        => GetTabByPart(part).items.Where(x => x.GetID() == id).FirstOrDefault();

    public TabItem GetRandom(CharacterPart part)
    {
        Tab selectedTab = GetTabByPart(part);
        return selectedTab.items[rand.Next(selectedTab.items.Length)];
    }

    private Tab GetTabByPart(CharacterPart part) 
        => Tabs.Where(x => x.editedCharacterPart == part).First();
}
