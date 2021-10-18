namespace SuperScrabble.Utilities
{
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;

    public static class AttributesGetter
    {
        public static string DisplayName<T>(string propertyName)
             => typeof(T).GetProperty(propertyName).GetCustomAttribute<DisplayAttribute>()?.Name;
    }
}
