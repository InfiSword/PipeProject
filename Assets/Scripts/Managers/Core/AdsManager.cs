using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager
{
    bool isTest = true;

    string _bannerID = string.Empty; // 배너 광고
    string _interstitialID = string.Empty; // 전면 광고
    string _rewardID = string.Empty; // 보상형 광고

    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    public void Init()
    {
        MobileAds.Initialize(initStatus =>
        {
            // 초기화 완료 후 광고 로드 시작
            RequestBanner();
            LoadInterstitialAd();
            //LoadRewardedAd();
        });

        if (isTest)
        {
            _bannerID = "ca-app-pub-3940256099942544/1033173712";
            _interstitialID = "ca-app-pub-3940256099942544/1033173712";
            _rewardID = "ca-app-pub-3940256099942544/5224354917";
        }
        else
        {
            _bannerID = "ca-app-pub-2697715692240086/8019836729";
            _interstitialID = "ca-app-pub-2697715692240086/8654432989";
            //_rewardID = "ca-app-pub-2697715692240086~8935803587";
        }

    }


    #region 배너 광고

    // 배너 광고 로드
    public void RequestBanner()
    {
        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        // 배너 광고 객체 생성
        _bannerView = new BannerView(_bannerID, adaptiveSize, AdPosition.Bottom);

        // 광고 요청 생성
        AdRequest request = new AdRequest();

        // 광고 로드
        _bannerView.LoadAd(request);
    }

    // 배너 광고 표시
    public void ShowBanner()
    {
        if (_bannerView == null)
            RequestBanner();

        _bannerView.Show();
    }

    // 배너 광고 숨기기
    public void HideBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Hide();
        }
    }

    #endregion

    #region 전면 광고

    // 전면 광고 로드
    private void LoadInterstitialAd()
    {
        // 광고 요청 생성
        AdRequest request = new AdRequest();

        // 전면 광고 로드
        InterstitialAd.Load(_interstitialID, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("전면 광고 로드 실패: " + error.GetMessage());
                return;
            }

            _interstitialAd = ad;
            Debug.Log("전면 광고 로드 성공");

            // 광고 이벤트 핸들러 추가
            _interstitialAd.OnAdFullScreenContentClosed += OnInterstitialAdClosed;
            _interstitialAd.OnAdFullScreenContentFailed += OnInterstitialAdFailedToShow;
            _interstitialAd.OnAdImpressionRecorded += OnInterstitialAdImpressionRecorded;
        });
    }

    // 전면 광고 표시
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.Log("전면 광고가 로드되지 않았습니다.");
            LoadInterstitialAd();
        }
    }

    // 전면 광고 닫힘 이벤트 처리
    private void OnInterstitialAdClosed()
    {
        Debug.Log("전면 광고 닫힘");
        _interstitialAd.Destroy();
        _interstitialAd = null;
        LoadInterstitialAd();
    }

    private void OnInterstitialAdFailedToShow(AdError error)
    {
        Debug.LogError("전면 광고 표시 실패: " + error.GetMessage());
        _interstitialAd = null;
        LoadInterstitialAd();
    }

    private void OnInterstitialAdImpressionRecorded()
    {
        Debug.Log("전면 광고 노출 기록됨");
    }

    #endregion

    #region 보상형 광고

    // 보상형 광고 로드
    private void LoadRewardedAd()
    {
        // 광고 요청 생성
        AdRequest request = new AdRequest();

        // 보상형 광고 로드
        RewardedAd.Load(_rewardID, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("보상형 광고 로드 실패: " + error.GetMessage());
                return;
            }

            _rewardedAd = ad;
            Debug.Log("보상형 광고 로드 성공");

            // 광고 이벤트 핸들러 추가
            _rewardedAd.OnAdFullScreenContentClosed += OnRewardedAdClosed;
            _rewardedAd.OnAdFullScreenContentFailed += OnRewardedAdFailedToShow;
            _rewardedAd.OnAdImpressionRecorded += OnRewardedAdImpressionRecorded;
        });
    }

    // 보상형 광고 표시
    public void ShowRewardedAd(Action<Reward> Rewards)
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Show(Rewards);
        }
        else
        {
            Debug.Log("보상형 광고가 로드되지 않았습니다.");
            LoadRewardedAd();
        }
    }

    private void OnRewardedAdClosed()
    {
        Debug.Log("보상형 광고 닫힘");
        _rewardedAd.Destroy();
        _rewardedAd = null;
        LoadRewardedAd();
    }

    private void OnRewardedAdFailedToShow(AdError error)
    {
        Debug.LogError("보상형 광고 표시 실패: " + error.GetMessage());
        _rewardedAd = null;
        LoadRewardedAd();
    }

    private void OnRewardedAdImpressionRecorded()
    {
        Debug.Log("보상형 광고 노출 기록됨");
    }

    #endregion

    private void OnDestroy()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }

        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
        }
    }
}