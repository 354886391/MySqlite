using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    bool isStatus;
    List<string> storageData;

    void Awake()
    {
        storageData = new List<string>();
    }

    private async void Start()
    {
        await SaveAsync("", GetLast());
    }

    void Initialize(string filePath)
    {

    }

    async Task SaveAsync(string filePath, string content)
    {
        if (string.IsNullOrEmpty(content)) return;
        using (var writer = new StreamWriter(filePath))
        {
            await writer.WriteAsync(content);
        }
    }

    string GetLast()
    {
        int len = storageData.Count;
        var lastContext = string.Empty;
        if (len > 0)
        {
            lastContext = storageData[len - 1];
            storageData.RemoveAt(len - 1);
        }
        return lastContext;
    }

}
