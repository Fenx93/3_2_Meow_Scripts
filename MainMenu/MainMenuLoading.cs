using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLoading : MonoBehaviour
{
    [SerializeField] private GameObject goToMainButton, loadDataButton;
    [SerializeField] private TextMeshProUGUI title;
    private readonly string[] titleName = new string[] { "", "3", "2", "Meow!" };
    private readonly string[] colors = new string[] { "", "00FF16", "E7552C", "5CC5EF" };
    private string currentTitle = "";
    private int currentIterator = 0;
    private bool isFirstLaunch = false;
    [SerializeField] private Slider progressBar;

    void Awake()
    {
#if UNITY_EDITOR
        return;
#endif
        int isFirstLaunchInt = PlayerPrefs.GetInt("FirstLaunch");
        if (isFirstLaunchInt == 1)
        {
            isFirstLaunch = true;
            PlayerPrefs.SetInt("FirstLaunch", -1);
            PlayerStatsTracker.AdsPlayed = 0;
            //CountdownEnded();
            goToMainButton.SetActive(false);
            loadDataButton.SetActive(false);
            //loadDataButton.SetActive(true);
            Debug.LogWarning("MainMenuLoading Awake!");
            progressBar.value = 0.85f;
        }
        else
        {
            isFirstLaunch = false;
        }
    }

    void Start()
    {
        if (!isFirstLaunch) 
        { 
            GoToMainMenu();
        }
        else
        {
            CallLoadData();
        }
    }

    public void GoToMainMenu()
    {
        MainMenuController.current.Initiate();
        RewardsSpinMainMenuUI.current.Initiate();
        SettingsMenu.Instance.Initiate();
        MainMenuUI.current.Initiate();
        UI_Tabs.current.Initiate();
    }

    public void CallLoadData()
    {
        SaveGameMediator.OnSucessfullLoadDataUpdate += ShowGoToMenuButton;
        SaveGameController.LoadData();
    }

    private void ShowGoToMenuButton()
    {
        SaveGameMediator.OnSucessfullLoadDataUpdate -= ShowGoToMenuButton;
        //goToMainButton.SetActive(true);
        GoToMainMenu();
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

