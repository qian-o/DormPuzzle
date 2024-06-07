using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace DormPuzzle.Contracts.Views;

public abstract class UPage : Page
{
    protected UPage()
    {
        Margin = new(0, 0, 4, 4);

        // Loaded
        {
            EventTrigger eventTrigger = new(nameof(Loaded));

            InvokeCommandAction action = new();
            BindingOperations.SetBinding(action, InvokeCommandAction.CommandProperty, new Binding("ViewLoadedCommand"));

            eventTrigger.Actions.Add(action);

            Interaction.GetTriggers(this).Add(eventTrigger);
        }

        // Unloaded
        {
            EventTrigger eventTrigger = new(nameof(Unloaded));

            InvokeCommandAction action = new();
            BindingOperations.SetBinding(action, InvokeCommandAction.CommandProperty, new Binding("ViewUnloadedCommand"));

            eventTrigger.Actions.Add(action);

            Interaction.GetTriggers(this).Add(eventTrigger);
        }
    }
}
