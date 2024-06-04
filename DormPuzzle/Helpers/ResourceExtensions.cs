using System.Windows;

namespace DormPuzzle.Helpers;

public static class ResourceExtensions
{
    public static void SetDynamicResource(this DependencyObject dependencyObject, DependencyProperty property, string resourceKey)
    {
        dependencyObject.SetValue(property, new DynamicResourceExtension(resourceKey).ProvideValue(null));
    }
}
