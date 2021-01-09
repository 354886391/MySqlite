using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

public static class RefUtility<T>
{

    private static PropertyInfo[] properties;

    public static int GetPropertiesLength()
    {
        return Properties.Length;
    }

    public static Type GetPropertyType(int index)
    {
        return Properties[index].PropertyType;
    }

    public static string GetPropertyName(int index)
    {
        return Properties[index].Name;
    }

    public static object GetPropertyValue(int index, T t)
    {
        return Properties[index].GetValue(t);
    }

    public static void SetPropertyValue(int index, T t, object value)
    {
        Properties[index].SetValue(t, Convert.ChangeType(value, GetPropertyType(index)));
    }

    public static PropertyInfo[] Properties
    {
        get
        {
            if (properties == null)
            {
                properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            }
            return properties;
        }
        set
        {
            properties = value;
        }
    }
}

