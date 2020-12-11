﻿public class Singleton<T> where T : class, new()
{
    private static T _instance;
    private readonly static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}
