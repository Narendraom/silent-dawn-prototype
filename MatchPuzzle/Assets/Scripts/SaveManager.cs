using UnityEngine;
using System.IO;

[System.Serializable]
public class CardState
{
    public int cardId;
    public bool isMatched;
    public bool isFlipped;
}

[System.Serializable]
public class GameSaveData
{
    public int score;
    public int combo;
    public int matchesFound;
    public int level;
    public System.Collections.Generic.List<CardState> cardStates;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    public void SaveGame(GameSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public GameSaveData LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }

    public bool HasSavedGame()
    {
        return File.Exists(saveFilePath);
    }

    public void ClearSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}