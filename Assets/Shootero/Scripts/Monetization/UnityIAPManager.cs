#if IAP_BUILD
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;

using LitJson;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI;

public class CachedProduct
{
    public string productId;
    public string receipt;

    public CachedProduct()
    {
    }
    public CachedProduct(string productId, string receipt)
    {
        this.productId = productId;
        this.receipt = receipt;
    }
}

public class UnityIAPManager : MonoBehaviour, IStoreListener
{
    public static UnityIAPManager instance;
    public static void CreateInstance()
    {
        if (instance != null)
            return;
        instance = GameClient.instance.gameObject.AddComponent<UnityIAPManager>();
    }

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    //private StorePackType packageType;
    //private int packageIndex;
    private bool isInited = false;

    //public List<CachedProduct> purchasedProducts;
    //public string processingProduct = null;
    private List<Product> mListProcessingProduct = new List<Product>();

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    private void Awake()
    {
        instance = this;
        this.isInited = false;
    }

    void Start()
    {
        //// If we haven't set up the Unity Purchasing reference
        //if (m_StoreController == null)
        //{
        //    // Begin to configure our connection to Purchasing
        //    InitializePurchasing();
        //}
    }

//    public bool CheckCachedPurchasedProducts()
//    {
//#if UNITY_EDITOR
//        PlayerPrefs.DeleteKey("products");
//#endif
//        if (purchasedProducts == null)
//        {
//            string str = PlayerPrefs.GetString("products");
//            Debug.Log(str);
//            if (str != null)
//            {
//                try
//                {
//                    purchasedProducts = JsonMapper.ToObject<List<CachedProduct>>(str);
//                }
//                catch
//                {
//                    purchasedProducts = null;
//                }
//            }
//        }
//        if (purchasedProducts == null) purchasedProducts = new List<CachedProduct>();
//        if (purchasedProducts.Count > 0)
//        {
//            if (purchasedProducts[0] != null && purchasedProducts[0].productId != null)
//            {
//                OnPurchaseSuccessful(purchasedProducts[0].productId, purchasedProducts[0].receipt, true);
//                return true;
//            }
//            else
//            {
//                purchasedProducts.RemoveAt(0);
//                return CheckCachedPurchasedProducts();
//            }
//        }
//        return false;
//    }

    private void Update()
    {
        if (!this.isInited && InitializeUGS.instance && InitializeUGS.instance.IsInited)
        {
            this.InitializePurchasing();
        }
        else
        {

        }
    }

    private void InitOffers(ConfigurationBuilder builder, Dictionary<string, OfferStoreConfig> offerCfgs)
    {
        foreach (var pkg in offerCfgs)
        {
            var cfg = pkg.Value;
            var pkgName = pkg.Key;

            if (cfg != null && string.IsNullOrEmpty(cfg.StoreId) == false)
            {
                string fullStoreId = CashShopUtils.GetProductPackage(cfg.StoreId);
                builder.AddProduct(pkgName,
#if UNITY_IOS
                    cfg.IsNonConsum ? ProductType.NonConsumable : ProductType.Consumable,
#else
                    ProductType.Consumable,
#endif
                    new IDs
                    {
                        { fullStoreId, AppleAppStore.Name },
                        { fullStoreId, GooglePlay.Name }
                    }
                    );
            }
        }
    }

    public void ReInitializePurchasing()
    {
        if (this.isInited == true && this.IsInitialized() == false)
        {
            InitializePurchasing();
        }
    }
    [Beebyte.Obfuscator.ObfuscateLiterals]
    private void InitializePurchasing()
    {
        if (this.IsInitialized()) return;
        if (ConfigManager.cashShopConfig == null) return;
        //if (SocialManager.Instance.IsAuthenticated == false ||
        //    string.IsNullOrEmpty(SocialManager.Instance.SocialID) == true)
        //{
        //    return;
        //}

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        var flashSaleofferCfgs = CashShopUtils.GetFlashSaleOfferConfigs();
        InitOffers(builder, flashSaleofferCfgs);

        var dailyOfferCfgs = CashShopUtils.GetDailyOfferConfigs();
        InitOffers(builder, dailyOfferCfgs);

        var weeklyofferCfgs = CashShopUtils.GetWeeklyOfferConfigs();
        InitOffers(builder, weeklyofferCfgs);

        var monthlyofferCfgs = CashShopUtils.GetMonthlyOfferConfigs();
        InitOffers(builder, monthlyofferCfgs);

        var gemofferCfgs = CashShopUtils.GetGemOfferConfigs();
        InitOffers(builder, gemofferCfgs);

        var dailyGemPackCfgs = CashShopUtils.GetDailyGemPackConfigs();
        InitOffers(builder, dailyGemPackCfgs);

        var heroesCfgs = CashShopUtils.GetHeroOfferConfig();
        InitOffers(builder, heroesCfgs);

        var premiumCfgs = CashShopUtils.GetPremiumPackConfigs();
        InitOffers(builder, premiumCfgs);

        var targetofferCfgs = CashShopUtils.GetTargetOfferConfigs();
        InitOffers(builder, targetofferCfgs);

        var truongthanhofferCfgs = CashShopUtils.GetTruongThanhOfferConfigs();
        InitOffers(builder, truongthanhofferCfgs);

        var teteventofferCfgs = CashShopUtils.GetTetEventOfferConfigs();
        InitOffers(builder, teteventofferCfgs);

        var limitedTimeCfgs = CashShopUtils.GetLimitedTimeOfferConfig();
        if (limitedTimeCfgs != null)
            InitOffers(builder, limitedTimeCfgs);

//#if UNITY_ANDROID
//        builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlCclnZwwS67QUA5NvKFra+F6eYmuP6+u2z/Q/BM0xhdjOGOLK/DzdcqDYZ+hHjULhYO2NmCdiFEVFq3CB4c3K407LZpo+rQO9JY6T30XQZNkuPqcvp5dOr1WT5xERM+fnAPtAX5z6hPCAay8z+AkQBhO/4yFeZfd+un/tAMn8TAHStHRmNbgXgiMlAgoxWgJxoebF0AKvbASKoY7yyvHGhK0sDKONS21C9GKj1uXrhguWNY/4YS/IWcYQzIhMu0vu+Ud3kbH4MFM9NZBiJFQnytSB8GICPCn4GiiGf7SE/5Xwuz5ta2P0KaJO1iwXDEW70Xz0lCHena9eSc3jSeEfQIDAQAB");
//#endif
        UnityPurchasing.Initialize(this, builder);
        this.isInited = true;
    }

    [GUIDelegate]
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

        Debug.Log("Init IAP success");

        if (Hiker.GUI.Shootero.ScreenMain.instance != null)
        {
            Hiker.GUI.Shootero.ScreenMain.instance.OnInitializedShop();
        }

        OnInitializedShop();
    }

    public void RestoreTransactions()
    {
#if IAP_BUILD
        if (UnityIAPManager.instance.CheckLoginToPurchase() == false)
        {
            return;
        }
#endif
#if UNITY_IOS
        if (IsInitialized())
        {
            m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result) =>
            {
                if (result)
                {
                    Debug.Log("Restoration process succeed");
                    PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("RestorationSuccess"));
                }
                else
                {
                    Debug.Log("Restoration failed");
                    PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("RestorationFail"));
                }
            });
        }
        else
        {
            Debug.LogWarning("BuyProductID FAIL. Not initialized.");
        }
#endif
    }

    bool IsInitedShop = false;
    public void OnInitializedShop()
    {
        if (IsInitedShop) return;
        IsInitedShop = true;
        //#if IAP_BUILD
        //        if (UnityIAPManager.instance != null)
        //        {
        //            UnityIAPManager.instance.CheckCachedPurchasedProducts();
        //            
        //        }
        //#endif

        //#if CHINA
        //        if (TeeBikSDKManager.instance != null)
        //        {
        //            TeeBikSDKManager.instance.CheckCachedPurchasedProducts();
        //        }
        //#endif
    }
    [GUIDelegate]
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log(error.ToString());
    }
    [GUIDelegate]
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Hiker.GUI.PopupNetworkLoading.Dismiss();
        AnalyticsManager.LogEvent("IAP_CANCEL");
        //throw new NotImplementedException();
    }

    //private string GetGemPackProductID(StorePackType type, int index)
    //{
    //    string id = GameRequests.RequestBuyGemPack + "_" + type.ToString() + "_" + index;

    //    return id;
    //}
    //private string GetSpecialOfferProductID(string type, int index)
    //{
    //    string name = CashShopUtils.GetSpecialOfferName(type, index);
    //    string id = GameRequests.RequestBuySpecialOrder + "_" + type.ToString() + "_" + name;

    //    return id;
    //}

    //private string GetBPPremiumProductID(string productID)
    //{
    //    string id = GameRequests.RequestBuyPremiumBattlePass + "_" + productID;

    //    return id;
    //}

    public void BuyStorePack(string packageName)
    {
        if (CheckPurchaseIsMissing(packageName)) return;
        var product = m_StoreController.products.WithID(packageName);
        this.BuyProduct(product);
    }

    //public void BuyStorePack(StorePackType type, int index)
    //{
    //    if (CheckGemPackIsMissing(type, index))
    //        return;
    //    string id = GetGemPackProductID(type, index);
    //    Product product = m_StoreController.products.WithID(id);
    //    this.BuyProduct(product);
    //}

    //public void BuySpecialOffer(string type, int index)
    //{
    //    if (CheckSpecialOfferIsMissing(type, index))
    //        return;
    //    string id = GetSpecialOfferProductID(type, index);
    //    Product product = m_StoreController.products.WithID(id);
    //    this.BuyProduct(product);
    //}

    //public void BuyBPPremium(string productID)
    //{
    //    if (CheckBPPremiumIsMissing(productID))
    //        return;

    //    string id = productID;
    //    Product product = m_StoreController.products.WithID(id);
    //    this.BuyProduct(product);
    //}

    //public bool CheckBPPremiumIsMissing(string _productID)
    //{
    //    if (this.purchasedProducts == null || this.purchasedProducts.Count <= 0) return false;

    //    string productId = _productID;
    //    return this.purchasedProducts.Find(e => e.productId == productId) != null;
    //}

    public bool CheckPurchaseIsMissing(string productID)
    {
        if (mListProcessingProduct.Count > 0)
        {
            Product p = mListProcessingProduct.Find(e => e.definition.id == productID);

            return p != null;
        }

        return false;
        //if (this.purchasedProducts == null || this.purchasedProducts.Count <= 0) return false;
        //return this.purchasedProducts.Find(e => e.productId == key) != null;
    }

    //public bool CheckGemPackIsMissing(StorePackType type, int index)
    //{
    //    if (this.purchasedProducts == null || this.purchasedProducts.Count <= 0) return false;

    //    string productId = GetGemPackProductID(type, index);
    //    return this.purchasedProducts.Find(e => e.productId == productId) != null;
    //}

    //public bool CheckSpecialOfferIsMissing(string type, int index)
    //{
    //    if (this.purchasedProducts == null || this.purchasedProducts.Count <= 0) return false;

    //    string productId = GetSpecialOfferProductID(type, index);
    //    return this.purchasedProducts.Find(e => e.productId == productId) != null;
    //}

    public bool CheckLoginToPurchase()
    {
        if (IsInitialized() == false)
        {
//            if (SocialManager.Instance.IsAuthenticated == false ||
//                string.IsNullOrEmpty(SocialManager.Instance.SocialID) == true)
//            {
//                string mess = "";
//#if UNITY_ANDROID
//                mess = Localization.Get("Mes_NeedLoginGooglePlayToBuyPackage");
//#elif UNITY_IOS
//                mess = Localization.Get("Mes_NeedLoginGameCenterToBuyPackage");
//#endif
//                PopupConfirm.Create(
//                    mess,
//                    () =>
//                    {
//                        SocialManager.Instance.LinkAccount();

//                        //if (ScreenManager.instance.CurrentScreen == GAME_SCREEN.ShopScreen)
//                        //{
//                        //    ScreenManager.setScreen(GAME_SCREEN.BaseMapScreen);
//                        //}
//                    },
//                    null,
//                    Localization.Get("GUI_Login"));
//            }
//            else
            {
                ReInitializePurchasing();
            }

            return false;
        }

        return true;
    }

    private void BuyProduct(Product product)
    {
        if (IsInitialized())
        {

            if (product != null && product.availableToPurchase)
            {
                Debug.LogWarning(string.Format("Purchasing product asychronously: {0}", product.definition.id));
                m_StoreController.InitiatePurchase(product);
                Hiker.GUI.PopupNetworkLoading.Create(string.Empty);
            }
            else
            {
                Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.TEXT, Localization.Get("iap_not_init"));
                Debug.LogWarning("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.LogWarning("BuyProductID FAIL. Not initialized.");
        }
    }

    [GUIDelegate]
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Hiker.GUI.PopupNetworkLoading.Dismiss();

        if (mListProcessingProduct.Count > 0)
        {
            if (mListProcessingProduct.Exists(u => u.transactionID == e.purchasedProduct.transactionID) == false)
            {
                mListProcessingProduct.Add(e.purchasedProduct);
            }
        }
        else
        {
            mListProcessingProduct.Add(e.purchasedProduct);
        }

        //Debug.Log(e.purchasedProduct.receipt);
        if (e.purchasedProduct.hasReceipt)
        {
            if (GUIManager.Instance.CurrentScreen == "Main" ||
                PopupBuyGem.Instance)
            {
                //AddProductToCache(e.purchasedProduct);
                OnPurchaseSuccessful(e.purchasedProduct);
                //cache successful purchased product to local
            }
        }

        return PurchaseProcessingResult.Pending;
    }

    public bool HaveMissingProduct()
    {
        return mListProcessingProduct.Count > 0;
    }

    public bool CheckListProcessingProduct()
    {
        if (IsInitialized() == false || isInited == false) return false;

        if (mListProcessingProduct.Count > 0)
        {
            OnPurchaseSuccessful(mListProcessingProduct[0]);
            return true;
        }
        return false;
    }

    //public void AddProductToCache(Product product)
    //{
    //    if (purchasedProducts == null)
    //        return;
    //    var exist = purchasedProducts.Find(e => e.productId == product.definition.id) != null;
    //    if (!exist)
    //    {
    //        purchasedProducts.Add(new CachedProduct(product.definition.id, product.receipt));
    //        var str = JsonMapper.ToJson(purchasedProducts);
    //        Debug.Log("cache " + str);
    //        PlayerPrefs.SetString("products", str);
    //    }
    //}

    //public void RemoveProductFromCache(string productId)
    //{
    //    if (purchasedProducts == null)
    //        return;
    //    CachedProduct cachedProduct = purchasedProducts.Find(e => e.productId == productId);
    //    if (cachedProduct != null)
    //    {
    //        purchasedProducts.Remove(cachedProduct);
    //        var str = JsonMapper.ToJson(purchasedProducts);
    //        PlayerPrefs.SetString("products", str);
    //    }
    //}

    public void OnPurchaseSuccessful(Product product, bool missedOrder = false)
    {
        string producId = product.definition.id;
        string receipt = product.receipt;
        string transID = product.transactionID;
        //processingProduct = producId;
        OnPurchaseSuccessful(producId, receipt, transID, missedOrder);
    }

    public void OnPurchaseSuccessful(string producId, string receipt, string transID, bool missedOrder = false)
    {
        if (GameClient.instance == null ||
            GameClient.instance.UInfo == null ||
            GameClient.instance.UInfo.ListOffers == null) return;

        var offerData = GameClient.instance.UInfo.ListOffers.Find(e => e.PackageName == producId);
        if (offerData == null)
        {
            return;
        }

        var config = offerData.config;

        if (config == null)
        {
            config = CashShopUtils.GetOfferConfig(offerData.PackageName, offerData.Type);
        }

        if (offerData.Type == OfferType.GemOffer)
        {
            GameClient.instance.RequestBuyGemPack(producId, transID, receipt);

            //if (config != null)
            //{
            //    string priceStr = config.Price.Substring(1);
            //    var price = float.Parse(priceStr);
            //    AnalyticsManager.LogPurchase("Gem", producId, price);
            //}
        }
        else if (offerData.Type == OfferType.DailyGemPack)
        {
            //GameClient.instance.RequestBuyDailyGemPack(producId, receipt, missedOrder);

            //if (config != null)
            //{
            //    string priceStr = config.Price.Substring(1);
            //    var price = float.Parse(priceStr);
            //    AnalyticsManager.LogPurchase("GemDaily", producId, price);
            //}
        }
        else if (offerData.Type == OfferType.PremiumPack)
        {
            GameClient.instance.RequestBuyPremiumPack(producId, transID, receipt);
        }
        else if (offerData.Type == OfferType.TargetOffer)
        {
            GameClient.instance.RequestBuyTargetOffer(producId, transID, receipt);
        }
        else if (offerData.Type == OfferType.TruongThanhOffer)
        {
            GameClient.instance.RequestBuyTruongThanhOffer(producId, transID, receipt);
        }
        else if (offerData.Type == OfferType.TetEventOffer)
        {
            GameClient.instance.RequestBuyTetEventOffer(producId, transID, receipt);
        }
        else
        {
            //Debug.Log("purchase successfull offer " + producId);
            GameClient.instance.RequestBuyOfferStore(producId, transID, receipt, missedOrder);
        }
    }

    //public string GetGemLocalPrice(string packageName)
    //{
    //    //var unityProducId = GameRequests.RequestBuyGemPack + "_" + StorePackType.Basic.ToString();
    //    var fullProducId = GetGemPackProductID(StorePackType.Basic, index);//unityProducId + "_" + index;
    //    return this.GetLocalPrice(fullProducId);
    //}

    //public string GetGemLocalPrice(int index)
    //{
    //    //var unityProducId = GameRequests.RequestBuyGemPack + "_" + StorePackType.Basic.ToString();
    //    var fullProducId = GetGemPackProductID(StorePackType.Basic, index);//unityProducId + "_" + index;
    //    return this.GetLocalPrice(fullProducId);
    //}

    //public string GetSpecialOfferLocalPrice(string type, int index)
    //{
    //    //var unityProducId = GameRequests.RequestBuySpecialOrder;
    //    var product_id = GetSpecialOfferProductID(type, index);// unityProducId + "_" + type + "_" + index;
    //    return this.GetLocalPrice(product_id);
    //}

    //public string GetBattlePassPremiumLocalPrice()
    //{
    //    var product_ID = ConfigManager.battlePassCfg.ProductID;
    //    return this.GetLocalPrice(product_ID);
    //}

    public string GetLocalPrice(string product_id)
    {
        if (m_StoreController == null)
        {
            //Debug.Log("IAP must be initialized before purchasing");
            return string.Empty;
        }

        var product = m_StoreController.products.WithID(product_id);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }

        return string.Empty;
    }

    public System.Decimal GetLocalPriceValue(string product_id)
    {
        if (m_StoreController == null)
        {
            //Debug.Log("IAP must be initialized before purchasing");
            return 0;
        }

        var product = m_StoreController.products.WithID(product_id);
        if (product != null)
        {
            return product.metadata.localizedPrice;
        }

        return 0;
    }

    public string GetLocalCurrencyCode(string product_id)
    {
        if (m_StoreController == null)
        {
            //Debug.Log("IAP must be initialized before purchasing");
            return string.Empty;
        }

        var product = m_StoreController.products.WithID(product_id);
        if (product != null)
        {
            return product.metadata.isoCurrencyCode;
        }

        return string.Empty;
    }

    public void RemoveProcessingProduct(string transID)
    {
        Product p = mListProcessingProduct.Find(e => e.transactionID == transID);
        if (p != null)
            mListProcessingProduct.Remove(p);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void ConfirmPendingPurchase(string transID, bool isFirstPurchase = true)
    {
        if (mListProcessingProduct.Count > 0)
        {
            Product p = mListProcessingProduct.Find(e => e.transactionID == transID);
            if (p != null)
            {
                m_StoreController.ConfirmPendingPurchase(p);
                mListProcessingProduct.Remove(p);

                var producId = p.definition.id;
                //int index = producId.IndexOf('_');
                //string typeStr = producId.Substring(0, index);
                //string offerName = producId.Substring(index + 1, producId.Length - index - 1);

                //ShopConfig.EShopItemType type = (ShopConfig.EShopItemType)Enum.Parse(typeof(ShopConfig.EShopItemType), typeStr);

                //var offerCfg = HTTP.Instance.UInfo.GetShopItemCfg(offerName, type);
                var offer = GameClient.instance.UInfo.ListOffers.Find(e => e.PackageName == producId);
                var offerCfg = offer.GetConfig();

                string priceStr = offerCfg.Price.Substring(1);
                var price = float.Parse(priceStr);

                string type = "Offer";
                if (CashShopUtils.GetGemOfferConfigs().ContainsKey(producId))
                {
                    type = "Gem";
                }

                if (isFirstPurchase)
                {
                    var revenue = p.metadata.localizedPrice;
                    var netRateRevenue = 0.63m;
                    revenue *= netRateRevenue;

                    AnalyticsManager.LogPurchase(type,
                        producId,
                        revenue.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        p.metadata.isoCurrencyCode);

                    if (GameClient.instance.UInfo.Gamer.PurchaseCount == 0)
                    {
                        AnalyticsManager.LogEvent("FIRST_IAP",
                            new AnalyticsParameter("Price", price));
                        
                        var ts = GameClient.instance.ServerTime - GameClient.instance.UInfo.Gamer.RegisterTime;
                        var d = (int)Mathf.Round((float)ts.TotalDays);
                        AnalyticsManager.SetUserProperty("FirstIAP", d.ToString("D3"));
                    }
                }
            }
        }

        CheckListProcessingProduct();
    }

    [GUIDelegate]
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log(error.ToString() + " " + message);
    }
}
#endif