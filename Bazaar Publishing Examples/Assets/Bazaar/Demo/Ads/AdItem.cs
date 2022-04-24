using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdItem : MonoBehaviour
{

    [SerializeField] Ads ads;
    [SerializeField] Ad.Placement placement;
    [SerializeField] RTLTextMeshPro titleText;

    void Start()
    {
        ads.OnUpdate += Ads_onUpdate;
        ads.Request(placement);
        titleText.text = placement.ToString() + " Ad is loading ...";
    }


    void Ads_onUpdate(Ad ad)
    {
        var button = GetComponent<Button>();
        if (ad.placement != placement || ad.state != Ad.State.Load)
        {
            return;
        }

        titleText.text = placement.ToString() + " Ad is ready!";
        button.interactable = ad.IsReady();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ads.Show(placement));
    }
}
