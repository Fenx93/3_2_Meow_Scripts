using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SettingsTabs : MonoBehaviour
{
    private List<Transform> contentHolders;

    private void Start()
    {
        contentHolders = new List<Transform>();
        var contentsHolder = this.GetComponent<Transform>();
        foreach (Transform child in contentsHolder)
        {
            contentHolders.Add(child);
        }
        //contentHolders = contentsHolder.GetComponentsInChildren<Transform>();
            //.Where(x => x != contentsHolder).ToArray();
        SelectTab(0);
    }

    public void SelectTab(int selectedTab)
    {
        for (int i = 0; i < contentHolders.Count; i++)
        {
            ShowTab(contentHolders[i], selectedTab == i);
        }
    }

    private void ShowTab(Transform contentHolder, bool enabled)
    {
        contentHolder.gameObject.SetActive(enabled);
    }
}
