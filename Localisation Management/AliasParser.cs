using System.Xml;
using System.Xml.Linq;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;

public class AliasParser : MonoBehaviour
{
    static public AliasParser Inst => inst ??= FindObjectOfType<AliasParser>();
    static AliasParser inst;

    [SerializeField] string localisationResourcesPath = "Localisations";
    [SerializeField] public Localisations localisation = Localisations.Ru;

    [SerializeField] [HideInInspector] public List<string> aliases, texts;

    public event Action<Localisations> onLocalisationChanged;

    Dictionary<string, string> dictionary;

    void Awake()
    {
        LoadLocalisation();
    }

    public void SetLocaleEng()
    {
        SwitchLocalisation(Localisations.En);
    }
    public void SetLocaleRus()
    {
        SwitchLocalisation(Localisations.Ru);
    }

    public void SwitchLocalisation(Localisations newLocalisation)
    {
        if (localisation == newLocalisation) return;

        localisation = newLocalisation;

        LoadLocalisation();
    }

    private void LoadLocalisation()
    {
        TextAsset[] xmlAssets =
            Resources.LoadAll<TextAsset>(localisationResourcesPath+$"/{localisation}/")
            .ToArray();

        dictionary ??= new();

        foreach (var item in xmlAssets)
        {
            XElement document = XElement.Parse(item.text);

            var locale = document.Element("Locale");

            if (locale.Value == $"{localisation}")
            {
                locale
                    .ElementsAfterSelf()
                    .Map(el =>
                    {
                        dictionary.TryAdd(el.Attribute("Alias").Value,
                                          el.Attribute("Text").Value);
                    });
            }
        }


        onLocalisationChanged?.Invoke(localisation);
    }


    static public AliasedString GetAliasedString(string alias)
    {
        return new AliasedString(AliasParser.Inst, alias);
    }

    public string GetTextByAlias(string alias)
    {
        if (alias.Contains(' '))
            alias = StringExtension.ReplaceWhitespace(alias, "");

        if (!dictionary.ContainsKey(alias))
        {
            string message = $"Alias '{alias}' not found";
            Debug.LogWarning(message);
            return message;
        }
        return dictionary[alias];
    }
}

    public enum Localisations
        {
            Ru, En
        }
