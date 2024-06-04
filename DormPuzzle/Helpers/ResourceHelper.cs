using System.Windows;

namespace DormPuzzle.Helpers;

public static class ResourceHelper
{
    public static T? GetResource<T>(string resourceKey)
    {
        if (Application.Current.TryFindResource(resourceKey) is T resource)
        {
            return resource;
        }

        return default;
    }
}
