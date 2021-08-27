using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tabs : MonoBehaviour
{
    [SerializeField] private string[] tabNames;
    [SerializeField] private List<Button> tabButtons;
    [SerializeField] private List<Transform> tabPanels;
    private GameObject previousTab;
    private Transform tabs;

    private void Awake()
    {
        tabs = this.transform.Find("Tabs");
        if (tabs == null)
        {
            // Create new tabs holder
        }

        for (int i = 0; i < tabNames.Length; i++)
        {
            AddTab(tabNames[i], i);
        }
        tabButtons[0].onClick.Invoke();
    }

    public void AddTab(string name, int index)
    {
        //Add tab
        GameObject tabButton = new GameObject(name+"TabButton", typeof(Image));
        var button = tabButton.AddComponent(typeof(Button)) as Button;
        var c = button.colors;
        c.normalColor = Color.magenta;
        c.disabledColor = Color.gray;
        c.selectedColor = Color.blue;
        c.highlightedColor = Color.cyan;
        button.colors = c;
        button.onClick.AddListener(() => OpenTab(index));
        tabButton.transform.SetParent(tabs);

        GameObject buttonText = new GameObject("Text");
        var text = buttonText.AddComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        var rect = buttonText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);

        text.text = name;
        text.fontSize = 24;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;

        rect.SetParent(tabButton.transform);

        //Add panel
    }


    public void OpenTab(int index)
    {
        //get associated panel, enable it
        if (previousTab != null)
        {
            previousTab.SetActive(false);
        }
        previousTab = tabPanels[index].gameObject;
        previousTab.SetActive(true);
    }
}
