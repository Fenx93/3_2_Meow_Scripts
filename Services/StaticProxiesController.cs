using UnityEngine;

public class StaticProxiesController : MonoBehaviour
{
    //private PurchasesController purchasesController;
    //private SaveGameController saveGameController;

    //public static StaticProxiesController Instance;

    //private void Awake()
    //{
    //    Instance = this;
    //}
        // Start is called before the first frame update
    //void Start()
    //{
    //    purchasesController = FindObjectsOfType<PurchasesController>().First();
    //    saveGameController = FindObjectsOfType<SaveGameController>().First();
    //}

    public void PurchaseAdsDisabler()
    {
        PurchasesController.PurchaseAdsDisabler();
    }

    public void SaveGame()
    {
        SaveGameController.SaveData();
    }
}
