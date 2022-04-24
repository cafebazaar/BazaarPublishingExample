using System;
using RTLTMPro;
using System.Collections.Generic;
using System.Linq;
using TapsellPlusSDK;
using UnityEngine;

public class Ads : MonoBehaviour
{

    [SerializeField] string tapsellKey;
    [SerializeField] List<Ad> ads;
    [SerializeField] RTLTextMeshPro consoleText;

    public Action<Ad> OnUpdate;

    // Start is called before the first frame update
    void Start()
    {
        TapsellPlus.Initialize(tapsellKey,
            adNetworkName => Debug.Log(adNetworkName + " Initialized Successfully."),
            error => Debug.Log(error.ToString()));

        TapsellPlus.SetGdprConsent(true);
    }

    public void Request(Ad.Placement placement)
    {
        var ad = ads.Single(a => a.placement == placement);

        TapsellPlus.RequestRewardedVideoAd(ad.zoneId,

                  tapsellPlusAdModel =>
                  {
                      Log("on response " + tapsellPlusAdModel.responseId);
                      ad.responseId = tapsellPlusAdModel.responseId;
                      ad.state = Ad.State.Load;
                      OnUpdate?.Invoke(ad);
                  },
                  error =>
                  {
                      Log("Error " + error.message);
                  }
              );

    }

    public void Show(Ad.Placement placement)
    {
        var ad = ads.Single(a => a.placement == placement);
        TapsellPlus.ShowRewardedVideoAd(ad.responseId,

                  tapsellPlusAdModel =>
                  {
                      ad.state = Ad.State.Load;
                      OnUpdate?.Invoke(ad);
                      Log("onOpenAd " + tapsellPlusAdModel.zoneId);
                  },
                  tapsellPlusAdModel =>
                  {
                      ad.state = Ad.State.Reward;
                      OnUpdate?.Invoke(ad);
                      Log("onReward " + tapsellPlusAdModel.zoneId);
                  },
                  tapsellPlusAdModel =>
                  {
                      ad.state = Ad.State.Close;
                      OnUpdate?.Invoke(ad);
                      ad.attemptsCount = 0;
                      Request(ad.placement);
                      Log("onCloseAd " + tapsellPlusAdModel.zoneId);
                  },
                  error =>
                  {
                      ++ad.attemptsCount;
                      if (ad.attemptsCount < Ad.MAX_ATTEMPTS_COUNT)
                      {
                          Request(ad.placement);
                      }
                      Log("onError " + error.errorMessage);
                  }
              );
    }

    void Log(string message)
    {
        consoleText.text = consoleText.OriginalText + "Ads=> " + message + "\n";
    }
}

[Serializable]
public class Ad
{
    static public int MAX_ATTEMPTS_COUNT = 3;
    public enum Placement { Rewarded, Intrestitial, Banner }
    public enum State { Load, Open, Reward, Close }
    public Placement placement;
    internal State state;
    public string zoneId;
    internal string responseId;
    internal int attemptsCount;

    public bool IsReady() => !String.IsNullOrEmpty(responseId);
}
