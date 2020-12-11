using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SQLiteUtil
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
            return "DATA SOURCE = " + FilePath + "/KingKong; Version = 3;";
        }
    }

    private const string _createTable =
        @"CREATE TABLE IF NOT EXISTS Player(            
        id INTEGER,
        bet INTEGER,
        rate INTEGER,
        score INTEGER,
        board TEXT,
        jackpotType INTEGER,
        jackpotValue DECIMAL,
        PRIMARY KEY(id AUTOINCREMENT))";

    private const string _createTrigger =
        @"CREATE TRIGGER IF NOT EXISTS LimitCount AFTER INSERT ON Player 
        BEGIN
	        DELETE FROM Player WHERE Id=((SELECT MIN(id) FROM Player)) AND ((SELECT COUNT(id) FROM Player)>20);
        END;";

    private const string _insertCmd =
        @"INSERT INTO Player(Id, Bet, Rate, Score, Board, JackpotType, JackpotValue)
                      VALUES( @id, @bet, @rate, @score, @board, @jackpotType, @jackpotValue);";

    private const string _updateCmd =
        @"UPDATE Player SET Bet=@bet, Rate=@rate, Score=@score, Board=@board, JackpotType=@jackpotType, JackpotValue=@jackpotValue WHERE Id=@id;";

    private const string _deleteCmd =
        @"DELETE FROM Player WHERE Id=@id;";

    private const string _selectCmd =
        @"SELECT * FROM Player WHERE Id=@id;";

    private const string _getIDCmd =
        @"SELECT {0}(id) FROM Player;";

    private const string _getCountCmd =
        @"SELECT count(id) FROM Player;";

    private static SqliteConnection connection;

    private static void CreateDirectory()
    {
        if (!Directory.Exists(FilePath))
        {
            Directory.CreateDirectory(FilePath);
        }
    }

    public static bool HasDatabase()
    {
        return File.Exists(FilePath + "/KingKong");
    }

    public static bool CreateTable()
    {
        CreateDirectory();
        return Execute(_createTable) == 0;
    }

    public static bool CreateTrigger()
    {
        return Execute(_createTrigger) == 0;
    }

    private static void FindConnection()
    {
        if (connection == null)
        {
            connection = new SqliteConnection(DatabasePath);
            connection.Open();
        }
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public static int Execute(string cmdStr)
    {
        try
        {
            FindConnection();
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = cmdStr;
                return sqlCommand.ExecuteNonQuery();
            }
        }
        catch (SqliteException se)
        {
            Debug.LogError(se.Message + "\n" + se.Source + "\n" + se.StackTrace + "\n" + se.Data);
            return int.MinValue;
        }
        catch (ArgumentException ae)
        {
            Debug.LogError(ae.Message + "\n" + ae.Source + "\n" + ae.StackTrace + "\n" + ae.Data);
            return int.MinValue;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return int.MinValue;
        }
    }

    /// <summary>
    /// 插入一条记录
    /// </summary>
    public static void Save()
    {
        try
        {
            FindConnection();
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = _insertCmd;
                sqlCommand.Parameters.AddWithValue("@id", null);
                sqlCommand.ExecuteNonQuery();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
        }
    }

    public static async void SaveAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                FindConnection();
                using (SqliteCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = _insertCmd;
                    sqlCommand.Parameters.AddWithValue("@id", null);
                    sqlCommand.ExecuteNonQuery();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
        }
    }

    /// <summary>
    /// 更新最近的一条记录
    /// </summary>
    /// <param name="minMax"></param>
    /// <returns></returns>
    public static bool Update()
    {
        try
        {
            FindConnection();
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = _updateCmd;
                sqlCommand.Parameters.AddWithValue("@id", GetID("MAX"));
                return sqlCommand.ExecuteNonQuery() == 1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return false;
        }
    }

    public static async void UpdateAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                FindConnection();
                using (SqliteCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = _updateCmd;
                    sqlCommand.Parameters.AddWithValue("@id", GetID("MAX"));
                    sqlCommand.ExecuteNonQuery();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
        }
    }

    public static bool Delete(int id)
    {
        try
        {
            FindConnection();
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = _deleteCmd;
                sqlCommand.Parameters.AddWithValue("@id", id);
                return sqlCommand.ExecuteNonQuery() == 1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return false;
        }
    }

    public static void Load(int id)
    {
        try
        {
            FindConnection();
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = _selectCmd;
                sqlCommand.Parameters.AddWithValue("@id", id);
                var sqlReader = sqlCommand.ExecuteReader();
                if (sqlReader.Read())
                {
                    //PlayerData.Id = sqlReader.GetInt32(0);


                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            //return null;
        }
    }

    /// <summary>
    /// 获取最近/最旧的一条记录
    /// </summary>
    /// <param name="minMax"></param>
    public static void Load(string minMax = "MAX")
    {
        try
        {
            FindConnection();
            Debug.Log("ConnectionState " + connection.State);
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = _selectCmd;
                sqlCommand.Parameters.AddWithValue("@id", GetID(minMax));
                var sqlReader = sqlCommand.ExecuteReader();
                if (sqlReader.Read())
                {
                    //PlayerData.Id = sqlReader.GetInt32(0);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
        }
    }

    /// <summary>
    /// 获取最大/最小的Id
    /// </summary>
    /// <param name="minMax"></param>
    /// <returns></returns>
    public static int GetID(string minMax)
    {
        try
        {
            FindConnection();
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = string.Format(_getIDCmd, minMax);
                var sqlReader = sqlCommand.ExecuteReader();
                return sqlReader.Read() ? sqlReader.GetInt32(0) : -1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return -1;
        }
    }

    public static async Task<int> GetIDAsync(string minMax)
    {
        try
        {
            return await Task.Run<int>(() =>
            {
                FindConnection();
                using (SqliteCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = string.Format(_getIDCmd, minMax);
                    var sqlReader = sqlCommand.ExecuteReader();
                    return sqlReader.Read() ? sqlReader.GetInt32(0) : -1;
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return -1;
        }
    }

    public static int GetCount()
    {
        try
        {
            FindConnection();
            using (SqliteCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = _getCountCmd;
                var sqlReader = sqlCommand.ExecuteReader();
                return sqlReader.Read() ? sqlReader.GetInt32(0) : -1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.Source + "\n" + e.StackTrace + "\n" + e.Data);
            return -1;
        }
    }
}
