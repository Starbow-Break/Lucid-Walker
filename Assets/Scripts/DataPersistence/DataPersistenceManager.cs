using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Called");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded Called");
        SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }
        // if no data can be loaded, initialize to a new game
        if (this.gameData == null)
        {
            Debug.Log("NO DATA FOUND. INITIALIZING DATA TO DEFULTS");
            return;
        }
        else
        {
            // JSON에서 로드한 데이터에 purchasedUpgradeIDs가 없을 수 있으므로, null 검사
            if (this.gameData.purchasedUpgradeIDs == null)
            {
                this.gameData.purchasedUpgradeIDs = new List<string>();
                Debug.Log("비엉ㅆ음");
            }
        }
        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }
    public void SaveGame()

    {
        if (this.gameData == null)
        {
            Debug.LogWarning("No Data was found");
        }
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        // save that data to a file using the data handler
        dataHandler.Save(gameData);

    }

    private void OnApplicationuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().
        OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    // 추가: 현재 gameData를 반환하는 메서드
    public GameData GetCurrentGameData()
    {
        if (gameData == null)
        {
            Debug.LogWarning("GameData가 null입니다. 새 게임 데이터를 생성합니다.");
            NewGame();
        }
        return gameData;
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
