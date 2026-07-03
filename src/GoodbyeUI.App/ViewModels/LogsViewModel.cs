using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;
using GoodbyeUI.Infrastructure.Common;

namespace GoodbyeUI.App.ViewModels;

/// <summary>
/// Günlük ekranının ViewModel'i. Canlı log akışını (<see cref="ILogFeed"/>) gösterir;
/// klasörü açma ve görünümü temizleme komutları sunar.
/// </summary>
public sealed partial class LogsViewModel : ObservableObject, IDisposable
{
    private const int MaxDisplayed = 500;

    private readonly ILogFeed _feed;
    private readonly IAppLogger _logger;

    public LogsViewModel(ILogFeed feed, ILocalizationService loc, IAppLogger logger)
    {
        _feed = feed;
        Loc = loc;
        _logger = logger;

        Entries = new ObservableCollection<LogEntry>(_feed.Snapshot());
        _feed.Emitted += OnEmitted;
    }

    public ILocalizationService Loc { get; }

    public ObservableCollection<LogEntry> Entries { get; }

    public bool HasEntries => Entries.Count > 0;

    private void OnEmitted(object? sender, LogEntry entry)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            Append(entry);
        else
            dispatcher.Invoke(() => Append(entry));
    }

    private void Append(LogEntry entry)
    {
        Entries.Add(entry);
        while (Entries.Count > MaxDisplayed)
            Entries.RemoveAt(0);
        OnPropertyChanged(nameof(HasEntries));
    }

    [RelayCommand]
    private void Clear()
    {
        Entries.Clear();
        OnPropertyChanged(nameof(HasEntries));
    }

    [RelayCommand]
    private void OpenFolder()
    {
        try
        {
            AppPaths.EnsureFolder(AppPaths.LogsFolder);
            Process.Start(new ProcessStartInfo
            {
                FileName = AppPaths.LogsFolder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _logger.Error("Günlük klasörü açılamadı.", ex);
        }
    }

    public void Dispose() => _feed.Emitted -= OnEmitted;
}
