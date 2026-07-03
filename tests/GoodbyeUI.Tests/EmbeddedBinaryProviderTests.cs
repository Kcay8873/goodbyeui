using GoodbyeUI.Infrastructure.GoodbyeDpi;
using GoodbyeUI.Tests.TestDoubles;

namespace GoodbyeUI.Tests;

public sealed class EmbeddedBinaryProviderTests
{
    [Fact]
    public async Task EnsureExecutableAsync_ExtractsEmbeddedGoodbyeDpi()
    {
        var sut = new EmbeddedGoodbyeDpiBinaryProvider(new FakeLogger());

        var path = await sut.EnsureExecutableAsync();

        Assert.True(File.Exists(path), $"Beklenen çalıştırılabilir bulunamadı: {path}");
        Assert.EndsWith("goodbyedpi.exe", path, StringComparison.OrdinalIgnoreCase);

        // WinDivert dosyaları da aynı klasöre çıkarılmış olmalı.
        var dir = Path.GetDirectoryName(path)!;
        Assert.True(File.Exists(Path.Combine(dir, "WinDivert.dll")), "WinDivert.dll çıkarılmadı.");
    }

    [Fact]
    public async Task EnsureExecutableAsync_IsIdempotent()
    {
        var sut = new EmbeddedGoodbyeDpiBinaryProvider(new FakeLogger());

        var first = await sut.EnsureExecutableAsync();
        var second = await sut.EnsureExecutableAsync();

        Assert.Equal(first, second);
    }
}
