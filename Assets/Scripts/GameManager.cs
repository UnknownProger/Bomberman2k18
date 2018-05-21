using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            return new GameObject("GameManager").AddComponent<GameManager>();
        }
    }
    private Transform _mapTransform;
    private Transform _playerTransform;
    private ContentDB _content;
    private float _totalTime;
    private int _level = 0;

    [Header("Рандомная генерация (если ВКЛ - остальные игнор.)")]
    public bool isFullRandom;
    public Vector2 levelSize;
    public int mobCount;
    public float totalTime;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        _content = Resources.Load<ContentDB>("ContentDB");
        Debug.Log("ContentDB was loaded");
    }
        
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();

        if (isFullRandom)
        {
            System.Random rnd = new System.Random();

            totalTime = rnd.Next(1, 6) * 60f;
            mobCount = rnd.Next(5, 11);

            int x = rnd.Next(11, 22);
            x = (x % 2 != 0) ? x : x + 1;  
            int y = rnd.Next(11, 22);
            y = (y % 2 != 0) ? y : y + 1;  

            levelSize = new Vector2(x, y);
        }

        GameInit(totalTime, mobCount, levelSize);
    }

    private void GameInit(float time, int mobCount, Vector2 levelSize)
    {
        _totalTime = time;

        Level.Build((int)levelSize.x, (int)levelSize.y);
        MobSpawner.SpawnMobs(mobCount);

        SpawnPlayer(1, Level.sizeY-2);

        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        while (_totalTime > 0)
        {
            yield return new WaitForSeconds(1f);

            _totalTime--;
        }

        MobSpawner.SpawnBosses();
    }

    private void SpawnPlayer(int x, int y)
    {
        GameObject player = ObjectCreator.CreateView("Player", new Vector2(x, y), 3);
        player.AddComponent<PlayerController>();
        _playerTransform = player.transform;
    }

    public static Transform GetPlayerTransform()
    {
        return instance._playerTransform;
    }

    public static Sprite GetSprite(int id)
    {
        return instance._content.sprites[id];
    }

    public static int GetTimeLeft()
    {
        return (int)instance._totalTime;
    }

    public static int GetLevel()
    {
        return instance._level;
    }

    public static void SpawnPortal()
    {
        System.Random rand = new System.Random();

        int x = rand.Next(1, Level.sizeX - 1);
        int y = rand.Next(1, Level.sizeY - 1);

        Level.map[x, y].view.sprite = GetSprite(15);
        Level.map[x, y].typeId = 5;
    }

    public static void SendGameResult(bool isWin)
    {
        if (isWin)
        {
            instance._level++;
        }
        else
        {
            instance._level = 0;
        }
        instance.StartCoroutine(RestartTimer(3f));
        UIManager.ShowGameResult(isWin); 
    }

    static IEnumerator RestartTimer(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync("game");
    }


}
