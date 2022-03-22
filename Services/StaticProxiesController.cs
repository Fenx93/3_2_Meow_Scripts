using System.Linq;
using UnityEngine;

public class StaticProxiesController : MonoBehaviour
{
    private PurchasesController purchasesController;
    private SaveGameController saveGameController;

    public static StaticProxiesController Instance;

    private void Awake()
    {
        Instance = this;
    }
        // Start is called before the first frame update
    void Start()
    {
        purchasesController = FindObjectsOfType<PurchasesController>().First();
        saveGameController = FindObjectsOfType<SaveGameController>().First();
    }

    public void PurchaseAdsDisabler()
    {
        if (purchasesController != null)
        {
            purchasesController.PurchaseAdsDisabler();
        }
        else
        {
            Debug.LogWarning("Purchase Controller from static GameObject not found!");
        }
    }

    public void SaveGame()
    {
        if (saveGameController != null)
        {
            saveGameController.SaveData();
        }
        else
        {
            Debug.LogWarning("Save Game Controller from static GameObject not found!");
        }
    }

    //public bool SaveAvailable()
    //{
    //    if (saveGameController != null)
    //    {
    //        return saveGameController.SaveAvailable;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Save Game Controller from static GameObject not found!");
    //        return false;
    //    }
    //}
}
