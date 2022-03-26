using System;
using System.Collections.Generic;
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

    //public void UnlockItem(TabItem item, CharacterPart part)
    //    => GetTabByPart(part).items.Where(x => x.quality == item.quality);

    public TabItem GetInventoryItem(CharacterPart part, string id)
        => GetTabByPart(part).items.Where(x => x.GetID() == id).FirstOrDefault();

    #region Public GetRandomItem Methods
    public TabItem GetRandom(CharacterPart part)
    {
        Tab selectedTab = GetTabByPart(part);
        return selectedTab.items[rand.Next(selectedTab.items.Length)];
    }

    public TabItem GetRandomUnlockedItem(CharacterPart part)
    {
        Tab selectedTab = GetTabByPart(part);
        var items = selectedTab.items.Where(x => x.status == ItemStatus.unlocked);
        return GetRandomFromList(items);
    }

    public TabItem GetRandomLockedItem(CharacterPart part, ItemQuality quality)
    {
        Tab selectedTab = GetTabByPart(part);
        var items = selectedTab.items.Where(x => x.quality == quality && x.status == ItemStatus.locked);
        return GetRandomFromList(items);
    }

    private TabItem GetRandomFromList(IEnumerable<TabItem> items)
    {
        if (items.Any())
        {
            var rand = new System.Random();
            var item = items.ElementAt(rand.Next(items.Count()));
            return item;
        }
        return null;
    }
    #endregion

    public TabItem GetItemByID(CharacterPart part, string id)
    {
        Tab selectedTab = GetTabByPart(part);
        return selectedTab.items.Where(x => x.status == ItemStatus.unlocked && x.GetID() == id).First();
    }


    public static void SelectItem(Sprite sprite, CharacterPart part, string id)
    {
        CharacterCustomizer.current.avatars[0].SetSprite(sprite, part);
        CharacterCustomizer.current.avatars[1].SetSprite(sprite, part);
        MainMenuController.current.selectedItems[part] = id;
        switch (part)
        {
            case CharacterPart.hat:
                CharacterStore.hat = sprite;
                break;
            case CharacterPart.clothes:
                CharacterStore.clothes = sprite;
                break;
            default:
                break;
        }
    }

    public static void SelectColor(Color color, CharacterPart part, string id)
    {
        CharacterCustomizer.current.avatars[0].SetColor(color, part);
        CharacterCustomizer.current.avatars[1].SetColor(color, part);
        MainMenuController.current.selectedItems[part] = id;
        switch (part)
        {
            case CharacterPart.mainColor:
                CharacterStore.mainColor = color;
                break;
            case CharacterPart.secondaryColor:
                CharacterStore.secondaryColor = color;
                break;
            default:
                break;
        }
    }

    private Tab GetTabByPart(CharacterPart part) 
        => Tabs.Where(x => x.editedCharacterPart == part).First();
}
