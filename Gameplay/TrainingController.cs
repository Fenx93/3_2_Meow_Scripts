using System.Collections;
using UnityEngine;

public class TrainingController : MonoBehaviour
{
    private int currentIteration = 0;

    [SerializeField] GameObject[] windows;
    [SerializeField] ArrowDisplayState[] arrowsStates;

    public static TrainingController current;

    private bool _continueGame = true;

    void Awake()
    {
        current = this;
        foreach (var item in windows)
        {
            item.SetActive(false);
        }
    }
    void Start()
    {
        OpenWindow(currentIteration, true);
    }

    public void NextWindow()
    {
        OpenWindow(currentIteration, false);
        currentIteration++;

        if (windows.Length > currentIteration && windows[currentIteration] != null)
        {
            OpenWindow(currentIteration, true);
        }
        else if (windows.Length == currentIteration)
        {
            GameplayController.current.isTraining = false;
            GameplayController.current.startByDefault = true;
            GameplayController.current.SetupGame();
        }
    }


    private void OpenWindow(int index, bool enabled)
    {
        windows[index].SetActive(enabled);
        ArrowController.current.ResolveArrowDisplay(arrowsStates[index], enabled);
    }


    public void ShowActionClash()
    {
        StartCoroutine(nameof(Countdown));
    }


    public void PauseActionClash()
    {
        StopCoroutine(nameof(Countdown));
        StopCoroutine(nameof(Waiting));
    }

    private IEnumerator Countdown()
    {
        float duration = GameplayController.current.countdownDuration;

        while (duration > 1f)
        {
            if (!GameplayController.current.isPaused)
            {
                duration -= (Time.deltaTime * GameplayController.current.countdownMultiplier);
                UIController.current.UpdateTimer((int)duration);
            }
            yield return null;
        }
        CountdownEnded();
    }

    private IEnumerator Waiting(float duration = 2f)
    {
        float normalizedTime = 0;
        while (normalizedTime <= duration)
        {
            if (!GameplayController.current.isPaused)
            {
                normalizedTime += Time.deltaTime;
            }
            yield return null;
        }
        GameplayController.current.ResetActions();
        if (_continueGame)
        {
            StartCoroutine(nameof(Countdown));
        }
    }

    private void CountdownEnded()
    {
        var gameplay = GameplayController.current;

        // Reset class values
        var rangerClass = (RangerClass)gameplay.player.SelectedCharacterClass;
        rangerClass.HasAmmo = true;
        gameplay.player.Energy = 5;
        gameplay.enemy.Energy = 5;
        UIController.current.SelectedAction(0);

        UIController.current.EnableActionButtons();
        gameplay.enemy.SelectAction();

        while (_continueGame)
        {
            var playerResult = gameplay.ResolveAction(gameplay.player, gameplay.enemy);
            var enemyResult = gameplay.ResolveAction(gameplay.enemy, gameplay.player);
            //animate selected actions
            ActionAnimator.current.UpdateSelectedAction(gameplay.player.SelectedAction, playerResult);
            ActionAnimator.current.UpdateSelectedAction(gameplay.enemy.SelectedAction, enemyResult, false);

            if (!_continueGame)
                break;

            StartCoroutine(nameof(Waiting), 2f);
            break;
        }
    }
}

