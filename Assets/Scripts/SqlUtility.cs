using Mono.Data.Sqlite;
using System;
using System.Text;
using UnityEngine;

public class SqlUtility : MonoBehaviour
{
    public static string FilePath
    {
        get
        {
            return !Application.isEditor ? Application.persistentDataPath + "/GameData" : System.IO.Directory.GetCurrentDirectory() + "/GameData";
        }
    }
    public static string DatabasePath
    {
        get
        {
            return "DATA SOURCE = " + FilePath + "/BookData.db; Version = 3;";
        }
    }

    private SqliteConnection sqlConnection;
    private SqliteCommand sqlCommand;
    private SqliteDataReader sqlReader;

    private void OpenDb()
    {
        if (sqlConnection == null)
        {
            sqlConnection = new SqliteConnection(DatabasePath);
        }
        if (sqlConnection.State != System.Data.ConnectionState.Open)
        {
            sqlConnection.Open();
        }
        if (sqlCommand == null)
        {
            sqlCommand = sqlConnection.CreateCommand();
        }
    }

    private void CloseDb()
    {
        if (sqlReader != null)
        {
            sqlReader.Close();
            sqlReader = null;
        }
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

    private string CreateTableText<T>(string tablename)
    {
        var length = RefUtility.GetPropertiesLength<T>();
        StringBuilder builder = new StringBuilder(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(", tablename));
        for (int i = 0; i < length; i++)
        {
            builder.AppendFormat("{0} {1}{2}", RefUtility.GetPropertyName(i), RefUtility.GetSqliteType(i), i < length - 1 ? ", " : ");");
        }
        return builder.ToString();
    }

    private string InsertTableText<T>(string tablename, T t)
    {
        var length = RefUtility.GetPropertiesLength<T>();
        var builder = new StringBuilder(string.Format(@"INSERT INTO {0} VALUES(", tablename));
        for (var i = 0; i < length; i++)
        {
            builder.AppendFormat("\'{0}\'{1}", RefUtility.GetPropertyValue(i, t), i < length - 1 ? ", " : ");");
        }
        return builder.ToString();
    }

    private string SelectTableText(string tablename, int index)
    {
        return string.Format(@"SELECT * FROM {0} LIMIT 1 OFFSET {1};", tablename, index);
    }

    public bool Save<T>(string tablename, T t)
    {
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

    public void Load<T>(string tablename, T t, int index) where T : class, new()
    {
        try
        {
            OpenDb();
            sqlCommand.CommandText = SelectTableText(tablename, index);
            using (var sqlReader = sqlCommand.ExecuteReader())
            {
                if (sqlReader.Read())
                {
                    int count = sqlReader.FieldCount;
                    for (int i = 0; i < count; i++)
                    {
                        RefUtility.SetPropertiesValue(i, t, sqlReader.GetValue(i));
                        Debug.LogFormat("{0} Value: {1}", i, sqlReader.GetValue(i));
                    }
                }
            }
        }
        catch (Exception e)
        {
            t = new T();
            Debug.LogWarning(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
        }
    }

    private void Start()
    {
        Book book = new Book() { Guid = "ISBN9787111075752", Name = "设计模式-可复用面向对象软件的基础", Price = 35.89m, Press = "机械工业出版社", Classify = (byte)BookType.Education, IsEBook = false };
        Debug.Log(CreateTableText<Book>("data"));
        Debug.Log(InsertTableText<Book>("data", book));
        Debug.Log(SelectTableText("data", 0));
        Save("data", book);
        Load("data", book, 0);
        PrintBook(book);
    }

    private void OnDestroy()
    {
        CloseDb();
    }

    private void PrintBook(Book book)
    {
        Debug.LogFormat(" Guid = {0}, Name = {1}, Price = {2}, Press = {3}, Classify = {4}, IsEBook = {5} ", book.Guid, book.Name, book.Price, book.Press, book.Classify, book.IsEBook);
    }
}
