using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

public static class SqlUtility
{

    private const string dbname = "KingKong";

    private static string filePath
    {
        get
        {
            return !Application.isEditor ? "/data/cys/GameData" : Directory.GetCurrentDirectory() + "/GameData";
        }
    }
    public static string DatabasePath
    {
        get
        {
            return "DATA SOURCE = " + Path.Combine(filePath, dbname) + "; Version = 3;";
        }
    }

    private static SqliteConnection sqlConnection;
    private static SqliteCommand sqlCommand;

    private static void OpenDb()
    {
        if (sqlConnection == null)
        {
            sqlConnection = new SqliteConnection(DatabasePath);
            sqlConnection.Open();
        }
        if (sqlConnection.State != ConnectionState.Open)
        {
            sqlConnection.Open();
        }
        if (sqlCommand == null)
        {
            sqlCommand = sqlConnection.CreateCommand();
        }
    }

    private static void CloseDb()
    {
        if (sqlCommand != null)
        {
            sqlCommand.Dispose();
            sqlCommand = null;
        }
        if (sqlConnection != null)
        {
            sqlConnection.Close();
            sqlConnection = null;
        }
    }

    public static bool HasDb()
    {
        return File.Exists(Path.Combine(filePath, dbname));
    }

    public static string CreateTableText<T>(string tablename)
    {
        var length = RefUtility.GetPropertiesLength<T>();
        StringBuilder builder = new StringBuilder(string.Format(@"CREATE TABLE IF NOT EXISTS {0} (", tablename));
        for (int i = 0; i < length; i++)
        {
            builder.AppendFormat("{0} {1}{2}", RefUtility.GetPropertyName(i), RefUtility.GetSqliteType(i), i < length - 1 ? ", " : ")");
        }
        return builder.ToString();
    }

    public static string InsertTableText<T>(string tablename, T t)
    {
        var length = RefUtility.GetPropertiesLength<T>();
        var builder = new StringBuilder(string.Format(@"INSERT INTO {0} VALUES (", tablename));
        for (var i = 0; i < length; i++)
        {
            builder.AppendFormat("\'{0}\'{1}", RefUtility.GetPropertyValue(i, t), i < length - 1 ? ", " : ")");
        }
        return builder.ToString();
    }

    public static string SelectTableText(string tablename, int index)
    {
        return string.Format(@"SELECT * FROM {0} LIMIT 1 OFFSET {1}", tablename, index);
    }

    public static string SelectTableText<T>(string tablename, int index)
    {
        var length = RefUtility.GetPropertiesLength<T>();
        StringBuilder builder = new StringBuilder(@"SELECT ");
        for (int i = 0; i < length; i++)
        {
            builder.AppendFormat("{0}{1}", RefUtility.GetPropertyName(i), i < length - 1 ? ", " : " ");
        }
        builder.AppendFormat("FROM {0} LIMIT 1 OFFSET {1}", tablename, index);
        return builder.ToString();
    }

    public static string UpdateTableText<T>(string tablename, T t, int index)
    {
        var length = RefUtility.GetPropertiesLength<T>();
        var builder = new StringBuilder(string.Format(@"UPDATE {0} SET ", tablename));
        for (var i = 0; i < length; i++)
        {
            builder.AppendFormat("{0}=\'{1}\'{2}", RefUtility.GetPropertyName(i), RefUtility.GetPropertyValue(i, t), i < length - 1 ? ", " : " WHERE ROWID=" + (index + 1));
        }
        return builder.ToString();
    }

    public static string GetRowCountText(string tablename)
    {
        return string.Format(@"SELECT COUNT (ROWID) FROM {0}", tablename);
    }

    public static bool Save<T>(string tablename, T t)
    {
        //Debug.Log(InsertTableText(tablename, t));
        try
        {
            OpenDb();
            sqlCommand.CommandText = InsertTableText(tablename, t);
            return sqlCommand.ExecuteNonQuery() == 1;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return false;
        }
    }

    /// <summary>
    /// index [0, count-1]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tablename"></param>
    /// <param name="t"></param>
    /// <param name="index"></param>
    public static void Load<T>(string tablename, T t, int index) where T : class, new()
    {
        //Debug.Log(SelectTableText<T>(tablename, index));
        try
        {
            OpenDb();
            sqlCommand.CommandText = SelectTableText<T>(tablename, index);
            using (var sqlReader = sqlCommand.ExecuteReader())
            {
                if (sqlReader.Read())
                {
                    int count = sqlReader.FieldCount;
                    for (int i = 0; i < count; i++)
                    {
                        RefUtility.SetPropertiesValue(i, t, sqlReader.GetValue(i));
                        //Debug.LogFormat("{0} Value: {1}", i, sqlReader.GetValue(i));
                    }
                }
            }
        }
        catch (Exception e)
        {
            t = new T();
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
        }
    }

    public static void Load<T>(string tablename, T t) where T : class, new()
    {
        Load<T>(tablename, t, GetCount(tablename) - 1);
    }

    /// <summary>
    /// rowid [1, count-1]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tablename"></param>
    /// <param name="t"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool Update<T>(string tablename, T t, int index)
    {
        //Debug.Log(UpdateTableText(tablename, t, index));
        try
        {
            OpenDb();
            sqlCommand.CommandText = UpdateTableText(tablename, t, index);
            return sqlCommand.ExecuteNonQuery() == 1;

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return false;
        }
    }

    public static int GetCount(string tablename)
    {
        //Debug.Log(GetRowCountText(tablename));
        try
        {
            OpenDb();
            sqlCommand.CommandText = GetRowCountText(tablename);
            using (var sqlReader = sqlCommand.ExecuteReader())
            {
                return sqlReader.Read() ? sqlReader.GetInt32(0) : -1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return -1;
        }
    }

    public static void PrintT<T>(T t)
    {
        var builder = new StringBuilder();
        var length = RefUtility.GetPropertiesLength<T>();
        for (int i = 0; i < length; i++)
        {
            builder.AppendFormat("{0} {1}{2}", RefUtility.GetPropertyName(i), RefUtility.GetPropertyValue(i, t), i < length - 1 ? ", " : ";");
        }
        Debug.Log(builder.ToString());
    }

    public static void Dispose()
    {
        CloseDb();
    }
}
