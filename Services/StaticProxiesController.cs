using UnityEngine;

public class StaticProxiesController : MonoBehaviour
{
    public void PurchaseAdsDisabler()
    {
        PurchasesController.PurchaseAdsDisabler();
    }

    public void SaveGame()
    {
        SaveGameController.SaveData();
    }
}
