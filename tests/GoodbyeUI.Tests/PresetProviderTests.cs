using GoodbyeUI.Core.Models;
using GoodbyeUI.Infrastructure.GoodbyeDpi;

namespace GoodbyeUI.Tests;

public sealed class PresetProviderTests
{
    private readonly PresetProvider _sut = new();

    [Fact]
    public void GetAll_ReturnsAllFourPresets()
    {
        var all = _sut.GetAll();
        Assert.Equal(4, all.Count);
    }

    [Theory]
    [InlineData(PresetKind.Fast)]
    [InlineData(PresetKind.Balanced)]
    [InlineData(PresetKind.MaximumCompatibility)]
    public void Get_IncludesDnsRedirect(PresetKind kind)
    {
        // Türkiye ISS'leri DNS zehirlemesi yaptığından tüm hazır profiller DNS yönlendirmesi içermeli.
        var preset = _sut.Get(kind);
        Assert.Equal(kind, preset.Kind);
        Assert.Contains("--dns-addr", preset.Arguments);
        Assert.Contains("--dnsv6-addr", preset.Arguments);
    }

    [Fact]
    public void Balanced_MatchesProvenWorkingCommand()
    {
        var preset = _sut.Get(PresetKind.Balanced);
        Assert.Contains("-5", preset.Arguments);
        Assert.Contains("--set-ttl 5", preset.Arguments);
        Assert.Contains("--dns-port 1253", preset.Arguments);
    }

    [Fact]
    public void BuildCustom_TrimsArguments()
    {
        var preset = _sut.BuildCustom("   -5 --max-payload   ");
        Assert.Equal(PresetKind.Custom, preset.Kind);
        Assert.Equal("-5 --max-payload", preset.Arguments);
    }

    [Fact]
    public void Get_UnknownFallsBackToBalanced()
    {
        var preset = _sut.Get((PresetKind)999);
        Assert.Equal(PresetKind.Balanced, preset.Kind);
    }
}
