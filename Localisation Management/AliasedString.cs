using System;

public class AliasedString
{
    public string alias, text;

    public event Action onLocalisationChange;

    AliasParser aliasParser;

    public AliasedString(AliasParser aliasParser, string alias)
    {
        this.aliasParser = aliasParser;
        this.alias = alias;

        LoadLocalisation(aliasParser.localisation);

        aliasParser.onLocalisationChanged += LoadLocalisation;
    }

    public string ChangeAlias(string newAlias)
    {
        this.alias = newAlias;
        text = aliasParser.GetTextByAlias(alias);

        return text;
    }

    public void Unsubscribe()
    {
        aliasParser.onLocalisationChanged -= LoadLocalisation;
        onLocalisationChange = null;
    }

    void LoadLocalisation(Localisations localisation)
    {
        text = aliasParser.GetTextByAlias(alias);

        onLocalisationChange?.Invoke();
    }
}
