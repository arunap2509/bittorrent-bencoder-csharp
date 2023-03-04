namespace BEncoder;

using System;
using System.Collections.Generic;
using System.Reflection;

public static class ObjectExtensions
{
    public static Dictionary<string, object> ToDictionary(this object obj)
    {
        var dictionary = new Dictionary<string, object>();

        if (obj == null) return dictionary;

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            object value = property.GetValue(obj, null);
            if (value != null)
            {
                Type propertyType = property.PropertyType;
                if (propertyType.IsValueType || propertyType == typeof(string))
                {
                    dictionary.Add(property.Name, value);
                }
                else
                {
                    dictionary.Add(property.Name, value.ToDictionary());
                }
            }
        }

        return dictionary;
    }
}
