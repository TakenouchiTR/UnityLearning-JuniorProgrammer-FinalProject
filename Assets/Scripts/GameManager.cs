using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string SaveFile => $"{Application.persistentDataPath}/savefile.json";
    public static GameManager Instance { get; private set; }
    public int HighScore { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        Load();
    }

    public void Save()
    {
        SaveData data = new SaveData
        {
            score = HighScore
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SaveFile, json);
    }

    public void Load()
    {
        if (!File.Exists(SaveFile))
            return;

        string json = File.ReadAllText(SaveFile);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        HighScore = data.score;
    }

    [System.Serializable]
    class SaveData
    {
        public int score;
    }
}
