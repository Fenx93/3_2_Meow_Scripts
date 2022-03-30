using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] GameObject[] mapItems;

    void Awake()
    {
        foreach (var item in mapItems)
        {
            item.SetActive(false);
        }
    }

    public void SetupRandomMap()
    {
        var mapItem = mapItems[Random.Range(0, mapItems.Length)];
        mapItem.SetActive(true);
    }
}

