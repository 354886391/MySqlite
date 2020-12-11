using System;
using System.Reflection;

public static class RefUtility
{
    private static PropertyInfo[] _properties;

    public static PropertyInfo[] GetProperties()
    {
        return _properties;
    }

    public static void SetProperties<T>()
    {
        _properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    }

    public static int GetPropertiesLength<T>()
    {
        if (_properties == null)
            SetProperties<T>();
        return _properties.Length;
    }

    public static string GetPropertyName(int index)
    {
        return _properties[index].Name;
    }

    public static Type GetPropertyType(int index)
    {
        return _properties[index].PropertyType;
    }

    public static string GetPropertyTypeName(int index)
    {
        return _properties[index].PropertyType.ToString().Replace("System.", string.Empty);
    }

    public static object GetPropertyValue<T>(int index, T t)
    {
        return _properties[index].GetValue(t);
    }

    public static void SetPropertiesValue<T>(int index, T t, object value)
    {
        _properties[index].SetValue(t, Convert.ChangeType(value, GetPropertyType(index)));
    }

    public static string GetSqliteType(int index)
    {
        var str = string.Empty;
        var type = GetPropertyTypeName(index);
        switch (type)
        {
            case "Byte":
            case "Int16":
            case "Int32":
            case "Int64":
                str = "INTEGER"; break;
            case "Single":
            case "Double":
                str = "REAL"; break;
            case "Boolean":
            case "DateTime":
            case "Decimal":
                str = "NUMERIC"; break;
            case "Char":
            case "String":
                str = "TEXT"; break;
            default:
                str = "BLOB"; break;
        }
        return str;
    }
}

