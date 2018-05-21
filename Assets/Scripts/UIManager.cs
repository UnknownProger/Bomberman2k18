using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            return new GameObject("UIManager").AddComponent<UIManager>();
        }
    }
    public Text timerText;
    public Text levelText;
    public Text resultText;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        resultText.gameObject.SetActive(false);
        StopAllCoroutines();
        timerText.text = string.Format("TIME LEFT: {0} sec",GameManager.GetTimeLeft());
        levelText.text = string.Format("LEVEL: {0}", GameManager.GetLevel());
        StartCoroutine(UiUpdate(1f));
    }
        
    IEnumerator UiUpdate(float rate)
    {
        float time = 1 / rate;
        
        while (true)
        {
            yield return new WaitForSeconds(time);

            timerText.text = string.Format("TIME LEFT: {0} sec",GameManager.GetTimeLeft());
        }
    }

    public static void ShowGameResult(bool isWin)
    {
        instance.resultText.gameObject.SetActive(true);

        if (isWin)
        {
            instance.resultText.text = "YOU WIN!";
            instance.resultText.color = Color.green;
        }
        else
        {
            instance.resultText.text = "YOU LOSE!";
            instance.resultText.color = Color.red;
        }
    }

}
