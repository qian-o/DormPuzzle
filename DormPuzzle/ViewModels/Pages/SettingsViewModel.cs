using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DormPuzzle.Contracts.ViewModels;
using DormPuzzle.Views.Pages;
using Wpf.Ui.Appearance;

namespace DormPuzzle.ViewModels.Pages;

public partial class SettingsViewModel(SettingsPage view) : UViewModel<SettingsPage>(view)
{
    [ObservableProperty]
    private string version = string.Empty;

    protected override void ViewLoaded()
    {
        Version = $"Dorm Puzzle - {GetAssemblyVersion()}";

        if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
        {
            View.DarkThemeRadioButton.IsChecked = true;
        }
        else
        {
            View.LightThemeRadioButton.IsChecked = true;
        }
    }

    [RelayCommand]
    private static void OnLightThemeRadioButtonChecked()
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Light);
    }

    [RelayCommand]
    private static void OnDarkThemeRadioButtonChecked()
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);
    }

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString(fieldCount: 3) ?? string.Empty;
    }
}
