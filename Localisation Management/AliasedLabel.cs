using UnityEngine;
using Zenject;
using TMPro;

public class AliasedLabel : MonoBehaviour
{
    [SerializeField] Alias alias;
    [SerializeField] TextMeshProUGUI label;

    AliasedString aliasedString;

    [Inject] void Construct(AliasParser aliasParser, AliasedString aliasedString)
    {
        this.aliasedString = aliasedString;
        // aliasedString = AliasParser.GetAliasedString(alias);
        this.aliasedString.onLocalisationChange += UpdateText;

        UpdateText();
    }

    public void ChangeAlias(string alias)
    {
        aliasedString.ChangeAlias(alias);

        UpdateText();
    }

    void UpdateText()
    {
        label.text = aliasedString.text;
    }
}
