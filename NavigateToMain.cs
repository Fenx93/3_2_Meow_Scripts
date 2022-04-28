using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigateToMain : MonoBehaviour
{
    [SerializeField] private GameObject goToMainButton, goToTestButton;
    [SerializeField] private TextMeshProUGUI title;
    private readonly string[] titleName = new string[] { "", "3", "2", "Meow!" };
    private readonly string[] colors = new string[] { "", "00FF16", "E7552C", "5CC5EF" };
    private string currentTitle = "";
    private int currentIterator = 0;
    public void Start()
    {
        CountdownEnded();
        goToTestButton.SetActive(false);
        ShowTransitionButton(false);
#if UNITY_EDITOR
            ShowTransitionButton(true);
#endif
        SaveGameMediator.OnLoadDataUpdate += ShowTransitionButton;
    }
    public void GoToMainMenu()
    {
        Debug.LogWarning("Transtition to Main Menu invoked!");
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    private void ShowTransitionButton()
    {
        goToTestButton.SetActive(true); 
    }

    public void CallLoadData()
    {
        SaveGameMediator.OnLoadDataUpdate -= ShowTransitionButton;
        SaveGameMediator.OnSucessfullLoadDataUpdate += ShowTransitionButton2;
        SaveGameController.LoadData();
    }
    private void ShowTransitionButton2()
    {
        SaveGameMediator.OnSucessfullLoadDataUpdate -= ShowTransitionButton2;
        ShowTransitionButton(true);
    }

    private void ShowTransitionButton(bool enable)
    {
        goToMainButton.SetActive(enable);
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
