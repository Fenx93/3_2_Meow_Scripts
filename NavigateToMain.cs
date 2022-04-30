using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigateToMain : MonoBehaviour
{
    //[SerializeField] private GameObject nextSceneButton;
    [SerializeField] private TextMeshProUGUI title;
    private readonly string[] titleName = new string[] { "", "3", "2", "Meow!" };
    private readonly string[] colors = new string[] { "", "00FF16", "E7552C", "5CC5EF" };
    private string currentTitle = "";
    private int currentIterator = 0;
    AsyncOperation loadingOperation;
    [SerializeField] private Slider progressBar;

    void Start()
    {
#if UNITY_EDITOR
        GoToMainMenu();
#endif
        //CountdownEnded();
        SaveGameMediator.OnLoadDataUpdate += GoToMainMenu;
    }
    void Update()
    {
        if (loadingOperation != null)
        {
            float progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f) * 0.85f;
            progressBar.value = progressValue;
        }
    }

    public void GoToMainMenu()
    {
        Debug.LogWarning("Transition to Main Menu invoked!");
        PlayerPrefs.SetInt("FirstLaunch", 1);
        loadingOperation = SceneManager.LoadSceneAsync("MainMenuScene", LoadSceneMode.Single);
    }

    private IEnumerator Countdown()
    {
        float duration = 2f;
        while (duration > 1f)
        {
            duration -= (Time.deltaTime * 1.5f);
            yield return null;
        }
        CountdownEnded();
    }

    private void CountdownEnded()
    {
        if (currentIterator < titleName.Length - 1)
        {
            currentIterator++;

            var output = currentIterator != titleName.Length - 1 ?
                $"{titleName[currentIterator]}..." 
                : titleName[currentIterator];

            currentTitle = $"<color=#{colors[currentIterator]}>{output}</color>";
        }
        else
        {
            currentIterator = 0;
            currentTitle = "3 2 Meow!";
        }
        title.text = currentTitle;
        StartCoroutine(nameof(Countdown));
    }
}
