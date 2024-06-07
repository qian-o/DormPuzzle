using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DormPuzzle.Contracts.ViewModels;

public abstract partial class UViewModel<TView>(TView view) : ObservableRecipient where TView : FrameworkElement
{
    public TView View { get; } = view;

    [RelayCommand]
    protected virtual void ViewLoaded()
    {
    }

    [RelayCommand]
    protected virtual void ViewUnloaded()
    {
    }
}
