using System;
using GoogleMobileAds.Api;
using UnityEngine;
 
public class BannerAd : MonoBehaviour
{
    [SerializeField] private string androidAdUnitId;
    private bool _initialized;
    private BannerView _bannerView;
    public static BannerAd BannerAdInstance { get; private set; }

    private void Awake()
    {
        if (BannerAdInstance == null) {
            BannerAdInstance = this;
            DontDestroyOnLoad (gameObject);
        }
        else if (BannerAdInstance != this)
        {
            Destroy (gameObject);
            return;
        }
        InitializeAds();
    }

    void Start()
    {
        RequestBanner();
    }
    void InitializeAds()
    {
        MobileAds.Initialize(initStatus => { });
    }
    private void RequestBanner()
    {
        #if UNITY_ANDROID
            string adUnitId = androidAdUnitId;
        #elif UNITY_IPHONE
            string adUnitId = "";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.TopRight);
        LoadBanner();
    }

    void LoadBanner()
    {
        _bannerView.OnAdLoaded += HandleOnAdLoaded;
        _bannerView.LoadAd(new AdRequest.Builder().Build());
    }
    void HandleOnAdLoaded(object sender, EventArgs args)
    {
        _initialized = true;
    }
    
    public void ShowBannerAd()
    {
        if (!_initialized)
            return;
        _bannerView.Show();
    }

    public void HideBannerAd()
    {
        if (!_initialized)
            return;
        _bannerView.Hide();
    }
}