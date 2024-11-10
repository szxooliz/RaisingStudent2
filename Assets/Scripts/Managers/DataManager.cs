using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Experimental.RestService;
using UnityEngine;

using static Define;

public class DataManager : Singleton<DataManager>
{
    DataManager() { }

    public PlayerData playerData;

    public override void Init()
    {
        Debug.Log("DataManager- Init - LoadData 실행");
        LoadData();
    }

    public void LoadData()
    {
        string path;

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        }
        else
        {
            path = Path.Combine(Application.dataPath, "PlayerData.json");
        }

        if (!File.Exists(path))
        {
            Debug.LogError("플레이어 데이터 새로 생성");
            playerData = new PlayerData();
            SaveData();
        }

        FileStream fileStream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);

        Debug.Log("Player data loaded successfully.");
        Debug.Log($"Current Term: {playerData.currentTerm}, Current Turn: {playerData.currentTurn}");
        Debug.Log($"Stress Amount: {playerData.stressAmount}");
        Debug.Log($"Stats Amounts: {string.Join(", ", playerData.statsAmounts)}");
    }

    public void SaveData()
    {
        string path;

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        }
        else
        {
            path = Path.Combine(Application.dataPath, "PlayerData.json");
        }

        string jsonData = JsonUtility.ToJson(playerData, true);
        FileStream fileStream = new FileStream(path, FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
}
