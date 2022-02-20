using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowToPlayerHp, arrowToEnemyHp,
        arrowToPause, arrowToSettings, 
        arrowToPlayerEnergy, arrowToEnemyEnergy, 
        arrowToActionEnergy, arrowToActionEnergy1, 
        energyRing, energyRing1,
        arrowToReloadIcon, reloadIconRing,
        pointIcon;

    [SerializeField]
    private RectTransform clickableButton;
    [SerializeField]
    private GameObject[] arrowsToActions;

    public static ArrowController current;
    private bool keepPlayingCursorClick;

    // Start is called before the first frame update
    void Awake()
    {
        AnimateArrowsToHealth(false);
        AnimateArrowsToEnergy(false);
        AnimateArrowToPause(false);
        AnimateArrowsToActions(false);
        AnimateArrowsToActionsEnergy(false);
        AnimateActionClick(false);
        AnimateEnergyRing(false);
        current = this;
    }

    public void ResolveArrowDisplay(ArrowDisplayState state, bool enabled)
    {
        switch (state)
        {
            case ArrowDisplayState.none:
                break;
            case ArrowDisplayState.hp:
                AnimateArrowsToHealth(enabled);
                break;
            case ArrowDisplayState.turnsActionSelect:
                AnimateActionClick(enabled);
                break;
            case ArrowDisplayState.turnsActionsClash:
                GameplayController.current.enemy.EnemyAIType = AIType.defensive;
                AnimateActionClick(false);
                TrainingController.current.ShowActionClash();
                break;
            case ArrowDisplayState.energy:
                var ranger = (RangerClass)GameplayController.current.player.SelectedCharacterClass;
                ranger.HasAmmo = true;
                UIController.current.EnableActionButtons(GameplayController.current.player);
                AnimateArrowsToEnergy(enabled); 
                AnimateArrowsToActionsEnergy(enabled);
                AnimateEnergyRing(enabled);
                break;
            case ArrowDisplayState.actions:
                AnimateArrowsToActions(enabled);
                break;
            case ArrowDisplayState.actions_reload:
                AnimateArrowsToActionsReload(enabled);
                break;
            case ArrowDisplayState.pause:
                TrainingController.current.PauseActionClash();
                AnimateArrowToPause(enabled);
                break;
            case ArrowDisplayState.pause2:
                GameplayController.current.Pause();
                AnimateArrowToSettings(enabled);
                AnimateArrowsToActions(enabled);
                break;
        }
    }

    #region AnimateArrows methods
    public void AnimateArrowsToHealth(bool enable)
    {
        arrowToPlayerHp.SetActive(enable);
        arrowToEnemyHp.SetActive(enable);
        if (enable)
        {
            ArrowAnimation(arrowToPlayerHp, new Vector3(200, 200, 0));
            ArrowAnimation(arrowToEnemyHp, new Vector3(-200, 200, 0));
        }
    }

    public void AnimateArrowsToEnergy(bool enable)
    {
        arrowToPlayerEnergy.SetActive(enable);
        arrowToEnemyEnergy.SetActive(enable);
        if (enable)
        {
            ArrowAnimation(arrowToPlayerEnergy, new Vector3(200, 200, 0));
            ArrowAnimation(arrowToEnemyEnergy, new Vector3(-200, 200, 0));
        }
    }

    public void AnimateArrowToPause(bool enable)
    {
        arrowToPause.SetActive(enable);
        if (enable)
        {
            ArrowAnimation(arrowToPause, new Vector3(0, 200, 0));
        }
    }
    
    public void AnimateArrowToSettings(bool enable)
    {
        arrowToSettings.SetActive(enable);
        if (enable)
        {
            ArrowAnimation(arrowToSettings, new Vector3(0, 200, 0));
        }
    }

    public void AnimateArrowsToActionsReload(bool enable)
    {
        AnimateReloadIconRing(enable);
        arrowsToActions[0].SetActive(enable);
        arrowToReloadIcon.SetActive(enable);
        if (enable)
        {
            ArrowAnimation(arrowsToActions[0], new Vector3(0, -200, 0));
            ArrowAnimation(arrowToReloadIcon, new Vector3(200, 200, 0));
        }
    }
    
    public void AnimateArrowsToActions(bool enable)
    {
        foreach (var arrowToAction in arrowsToActions)
        {
            arrowToAction.SetActive(enable);
        }
        if (enable)
        {
            foreach (var arrowToAction in arrowsToActions)
            {
                ArrowAnimation(arrowToAction, new Vector3(0, -200, 0));
            }
        }
    }

    public void AnimateArrowsToActionsEnergy(bool enable)
    {
        arrowToActionEnergy.SetActive(enable);
        arrowToActionEnergy1.SetActive(enable);

        if (enable)
        {
            ArrowAnimation(arrowToActionEnergy, new Vector3(0, -200, 0));
            ArrowAnimation(arrowToActionEnergy1, new Vector3(0, -200, 0));
        }
    }

    public void AnimateEnergyRing(bool enable)
    {
        energyRing.SetActive(enable);
        energyRing1.SetActive(enable);

        if (enable)
        {
            LeanTween.scale(energyRing, new Vector3(1, 1, 0), 0.5f).setLoopPingPong();
            LeanTween.scale(energyRing1, new Vector3(1, 1, 0), 0.5f).setLoopPingPong();
        }
    }

    public void AnimateReloadIconRing(bool enable)
    {
        reloadIconRing.SetActive(enable);

        if (enable)
        {
            LeanTween.scale(reloadIconRing, new Vector3(1, 1, 0), 0.5f).setLoopPingPong();
        }
    }

    #endregion

    #region ActionClickAnimation
    public void AnimateActionClick(bool enable)
    {
        pointIcon.SetActive(enable);
        keepPlayingCursorClick = enable;
        if (enable)
        {
            var rectTransform = pointIcon.GetComponent<RectTransform>();
            var initialPosition = rectTransform.transform.position;
            MoveCursor(rectTransform, initialPosition);
        }
    }

    private void MoveCursor(RectTransform rectTransform, Vector3 initialPosition)
    {
        if (keepPlayingCursorClick)
        {
            UIController.current.UpdateSelectedActionText("");
            var sprite = Resources.Load<Sprite>("cursor");
            rectTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("cursor");
            LeanTween.move(rectTransform, new Vector3(0, -300, 0), 0.5f)
                .setOnComplete(() => AnimateCursorClick(rectTransform, initialPosition));
        }
    }

    private void AnimateCursorClick(RectTransform rectTransform, Vector3 initialPosition)
    {
        if (keepPlayingCursorClick)
        {
            rectTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("hand-pointer");
            AudioController.current.PlayButtonClick();
            UIController.current.SelectedAction(0);
            LeanTween.move(rectTransform, initialPosition, 0.01f).setDelay(1)
                .setOnComplete(() => MoveCursor(rectTransform, initialPosition));
        }
    }

    #endregion

    private void ArrowAnimation(GameObject gObject, Vector3 to)
    {
        LeanTween.move(gObject.GetComponent<RectTransform>(), to, 0.5f).setLoopPingPong();
    }
}

public enum ArrowDisplayState
{
    none,
    hp,
    energy,
    actions,
    actions_reload,
    pause,
    pause2,
    turnsActionSelect,
    turnsActionsClash,
}