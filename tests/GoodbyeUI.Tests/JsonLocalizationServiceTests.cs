using GoodbyeUI.Infrastructure.Localization;

namespace GoodbyeUI.Tests;

public sealed class JsonLocalizationServiceTests
{
    [Fact]
    public void AvailableLanguages_ContainsEnglishAndTurkish()
    {
        var sut = new JsonLocalizationService();
        Assert.Contains(sut.AvailableLanguages, l => l.Code == "en");
        Assert.Contains(sut.AvailableLanguages, l => l.Code == "tr");
    }

    [Fact]
    public void SetLanguage_SwitchesTranslations()
    {
        var sut = new JsonLocalizationService();

        sut.SetLanguage("en");
        Assert.Equal("Connect", sut["Connect_Button"]);

        sut.SetLanguage("tr");
        Assert.Equal("Bağlan", sut["Connect_Button"]);
    }

    [Fact]
    public void GetString_UnknownKey_ReturnsKey()
    {
        var sut = new JsonLocalizationService();
        Assert.Equal("__nope__", sut.GetString("__nope__"));
    }

    [Fact]
    public void SetLanguage_RaisesPropertyChangedForIndexer()
    {
        var sut = new JsonLocalizationService();
        var raised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
                raised = true;
        };

        sut.SetLanguage("tr");
        Assert.True(raised);
    }
}
