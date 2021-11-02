using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowToPlayerHp, arrowToEnemyHp,
        arrowToPauseHp, 
        arrowToPlayerEnergy, arrowToEnemyEnergy, arrowToActionEnergy, 
        energyRing;
    [SerializeField]
    private GameObject[] arrowsToActions;

    public static ArrowController current;

    // Start is called before the first frame update
    void Awake()
    {
        AnimateArrowsToHealth(false);
        AnimateArrowsToEnergy(false);
        AnimateArrowToPause(false);
        AnimateArrowsToActions(false);
        AnimateArrowsToActionsEnergy(false);
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
            case ArrowDisplayState.energy:
                AnimateArrowsToEnergy(enabled); 
                AnimateArrowsToActionsEnergy(enabled);
                AnimateEnergyRing(enabled);
                break;
            case ArrowDisplayState.actions:
                AnimateArrowsToActions(enabled);
                break;
            case ArrowDisplayState.pause:
                AnimateArrowToPause(enabled);
                break;
        }
    }

    #region AniamteArrows methods
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
        arrowToPauseHp.SetActive(enable);
        if (enable)
        {
            ArrowAnimation(arrowToPauseHp, new Vector3(0, 200, 0));
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

        if (enable)
        {
            ArrowAnimation(arrowToActionEnergy, new Vector3(0, -200, 0));
        }
    }

    public void AnimateEnergyRing(bool enable)
    {
        energyRing.SetActive(enable);

        if (enable)
        {
            LeanTween.scale(energyRing, new Vector3(1, 1, 0), 0.5f).setLoopPingPong();
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
    pause,
}