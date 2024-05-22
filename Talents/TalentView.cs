using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class TalentView : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI levelLabel;
    [SerializeField] TextMeshProUGUI nameLabel;
    [SerializeField] TextMeshProUGUI descriptionLabel;
    [SerializeField] CurrencyView priceView;
    [SerializeField] Button buyButton;
    [Space]
    [SerializeField] Talent talent;


    public void SetDescription(string description)
    {
        if (descriptionLabel)
            descriptionLabel.text = description;
    }

    public void ConnectBase(Talent talent)
    {
        this.talent = talent;

        icon.sprite = talent.sprite;

        nameLabel.text = talent.name;
       
        talent
            .ObserveDescription()
            .Subscribe( SetDescription )
            .AddTo(this);

        talent.buyableLevel.ware.level.current
            .Subscribe(l => levelLabel.text = $"Lvl {l:0}")
            .AddTo(this);
       
        priceView
            .InitAndSubscribe(talent.buyableLevel.prices[0]);

        talent.buyableLevel
            .ObserveIsAffordable()
            .SubscribeToInteractable(buyButton)
            .AddTo(this);

        buyButton.onClick.AddListener(() => talent.buyableLevel.Buy());
    }
}
