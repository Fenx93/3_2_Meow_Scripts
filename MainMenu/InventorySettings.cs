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

    [SerializeField] private Tab[] _tabs;
    public static Tab[] tabs;

    public static InventorySettings current;

    private System.Random rand;

    private void Awake()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(gameObject);

        rand = new System.Random();
        itemQualities = _itemQualities;
        tabs = _tabs;
    }

    public int CountItemsInTab(CharacterPart part)
    {
        Tab selectedTab = tabs.Where(x => x.editedCharacterPart == part).First();
        return selectedTab.items.Length;
    }

    public void UnlockItem(TabItem item, CharacterPart part)
    {
        Tab selectedTab = tabs.Where(x => x.editedCharacterPart == part).First();
        var tabItem = selectedTab.items.Where(x => x.quality == item.quality);
    }

    public TabItem GetInventoryItem(CharacterPart part, ItemQuality quality)
    {
        Tab selectedTab = tabs.Where(x => x.editedCharacterPart == part).First();
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
    {
        Tab selectedTab = tabs.Where(x => x.editedCharacterPart == part).First();
        return selectedTab.items.Where(x => x.GetID() == id).FirstOrDefault();
    }
    public TabItem GetRandom(CharacterPart part)
    {
        Tab selectedTab = tabs.Where(x => x.editedCharacterPart == part).First();
        return selectedTab.items[rand.Next(selectedTab.items.Length)];
    }
}
