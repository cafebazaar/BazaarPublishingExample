using System;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class ThroughCodeShop : CodelessShop, IStoreListener
{
    private IStoreController m_StoreController;
    private Dictionary<string, ShopItem> shopItems;
    [SerializeField] private List<Product> products;
    [SerializeField] private ShopItem shopItemTemplate;
    void Start()
    {
        shopItems = new Dictionary<string, ShopItem>();
        InitializePurchasing();
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Your products IDs. They should match the ids of your products in your store.
        //Add products that will be purchasable and indicate its type.
        builder.AddProduct("gas", ProductType.Consumable);
        builder.AddProduct("premium", ProductType.NonConsumable);
        builder.AddProduct("infinite_gas_monthly", ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var p in controller.products.all)
        {
            Log(p.definition.id);
            if (shopItems.ContainsKey(p.definition.id))
            {
                shopItems[p.definition.id].Init(p, m_StoreController.InitiatePurchase);
            }
            else
            {
                shopItems.Add(p.definition.id, Instantiate<ShopItem>(shopItemTemplate, transform).Init(p, m_StoreController.InitiatePurchase));
            }
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Log($"In-App Purchasing initialize failed: {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //Retrieve the purchased product
        var product = args.purchasedProduct;
        UpdateStats(product.definition.id);
        SendAnalyticsEvent(product);
        
        Log($"Purchase Complete - Product: {product.definition.id}");

        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    private void SendAnalyticsEvent(Product product)
    {
        string currency = product.metadata.isoCurrencyCode;
        decimal price = product.metadata.localizedPrice;

        // Creating the instance of the YandexAppMetricaRevenue class.
        YandexAppMetricaRevenue revenue = new YandexAppMetricaRevenue(price, currency);
        if (product.receipt != null)
        {
            // Creating the instance of the YandexAppMetricaReceipt class.
            YandexAppMetricaReceipt yaReceipt = new YandexAppMetricaReceipt();
            Receipt receipt = JsonUtility.FromJson<Receipt>(product.receipt);
#if UNITY_ANDROID
            PayloadAndroid payloadAndroid = JsonUtility.FromJson<PayloadAndroid>(receipt.Payload);
            yaReceipt.Signature = payloadAndroid.Signature;
            yaReceipt.Data = payloadAndroid.Json;
#elif UNITY_IPHONE
            yaReceipt.TransactionID = receipt.TransactionID;
            yaReceipt.Data = receipt.Payload;
#endif
            revenue.Receipt = yaReceipt;
        }
        // Sending data to the AppMetrica server.
        AppMetrica.Instance.ReportRevenue(revenue);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }
}

// Declaration of the Receipt structure for getting information about the IAP.
[System.Serializable]
public struct Receipt
{
    public string Store;
    public string TransactionID;
    public string Payload;
}

// Additional information about the IAP for Android.
[System.Serializable]
public struct PayloadAndroid
{
    public string Json;
    public string Signature;
}
