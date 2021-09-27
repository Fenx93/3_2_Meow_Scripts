using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAnimator : MonoBehaviour
{
    [SerializeField] private GameObject _playerVisualAction, _enemyVisualAction;

    public static ActionAnimator current;

    void Awake()
    {
        current = this;
    }

    public void UpdateSelectedAction(CombatAction action, CombatResolution resolution, bool isPlayer = true)
    {
        if (action.Classification != ActionClassification.none)
        {
            SpriteRenderer spriteRenderer;
            if (isPlayer)
            {
                _playerVisualAction.transform.position = new Vector3(-5f, -0.5f, 0f);
                _playerVisualAction.SetActive(true);
                spriteRenderer = _playerVisualAction.GetComponent<SpriteRenderer>();
                Vector3 targetPosition = new Vector3(0f, 0f, 0f);
                switch (resolution)
                {
                    case CombatResolution.passive:
                        targetPosition = _playerVisualAction.transform.position;
                        break;
                    case CombatResolution.attack:
                        targetPosition = new Vector3(5f, -0.5f, 0f);
                        break;
                }
                //move it towards the centre
                StartCoroutine(SmoothLerp(1f, _playerVisualAction, targetPosition));
            }
            else
            {
                _enemyVisualAction.transform.position = new Vector3(5f, -0.5f, 0f);
                _enemyVisualAction.SetActive(true);
                spriteRenderer = _enemyVisualAction.GetComponent<SpriteRenderer>();
                spriteRenderer.flipX = true;
                Vector3 targetPosition = new Vector3(0f, 0f, 0f);
                float time = 1f;
                switch (resolution)
                {
                    case CombatResolution.passive:
                        targetPosition = _enemyVisualAction.transform.position;
                        break;
                    case CombatResolution.attack:
                        targetPosition = new Vector3(-5f, -0.5f, 0f);
                        break;
                }
                //move it towards the centre
                StartCoroutine(SmoothLerp(time, _enemyVisualAction, targetPosition));
            }
            spriteRenderer.sprite = action.Visualisation;

            var col = UIController.current.selectedActionColors[action.Classification];
            col.a = 200;
            spriteRenderer.color = col;

            //make it appear


        }
    }

    public void DisableActionVisualisations()
    {
        _playerVisualAction.SetActive(false);
        _enemyVisualAction.SetActive(false);
    }

    private IEnumerator SmoothLerp(float time, GameObject gameObject, Vector3 position)
    {
        Vector3 startingPos = gameObject.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            gameObject.transform.position = Vector3.Lerp(startingPos, position, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
