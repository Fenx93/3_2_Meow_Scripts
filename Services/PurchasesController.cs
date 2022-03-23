using EasyMobile;
using System.Linq;
using UnityEngine;

public class PurchasesController : MonoBehaviour
{
    public static PurchasesController Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (!InAppPurchasing.IsInitialized())
            InAppPurchasing.InitializePurchasing();
    }

    // Subscribe to IAP purchase events
    void OnEnable()
    {
        InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
    }

    // Unsubscribe when the game object is disabled
    void OnDisable()
    {
        InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
    }

    public static bool AdsDisabled()
        => InAppPurchasing.IsInitialized() &&
        InAppPurchasing.IsProductOwned(EM_IAPConstants.Product_Disable_Ads);


    private static string[] GetAllProductNames()
    {
        // Get the array of all products created in the In-App Purchasing module settings
        // IAPProduct is the class representing a product as declared in the module settings
        IAPProduct[] products = InAppPurchasing.GetAllIAPProducts();

        // Print all product names
        foreach (IAPProduct prod in products)
        {
            Debug.Log("Product name: " + prod.Name);
        }
        return products.Select(p => p.Name).ToArray();
    }

    public static void PurchaseAdsDisabler()
    {
        if (InAppPurchasing.IsInitialized())
        {
            if (GetAllProductNames().Contains(EM_IAPConstants.Product_Disable_Ads))
            {
                // Purchase a product using its name
                // EM_IAPConstants.Sample_Product is the generated name constant of a product named "Sample Product"
                InAppPurchasing.Purchase(EM_IAPConstants.Product_Disable_Ads);
            }
            else
            {
                Debug.LogError($"{EM_IAPConstants.Product_Disable_Ads} not found!");
            }
        }
    }

    // Successful purchase handler
    private static void PurchaseCompletedHandler(IAPProduct product)
    {
        // Compare product name to the generated name constants to determine which product was bought
        switch (product.Name)
        {
            case EM_IAPConstants.Product_Disable_Ads:
                if (SettingsMenu.Instance != null)
                {
                    SettingsMenu.Instance.ShowDisableAdsButton();
                }
                NativeUI.Alert("Purchase successful!", $"{EM_IAPConstants.Product_Disable_Ads} was purchased successfully!");
                Debug.Log($"{EM_IAPConstants.Product_Disable_Ads} was purchased. The user should be granted it now.");

                break;
                // More products here...
        }
    }

    // Failed purchase handler
    private static void PurchaseFailedHandler(IAPProduct product, string failureReason)
    {
        NativeUI.Alert($"The purchase of product {product.Name} has failed!", $"Reason: {failureReason}");
        Debug.LogError($"The purchase of product {product.Name} has failed with reason: {failureReason}");
    }
}
