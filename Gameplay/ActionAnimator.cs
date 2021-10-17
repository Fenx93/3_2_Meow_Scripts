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
            float time = 0.75f;
            var gameObject = isPlayer ?
                _playerVisualAction :
                _enemyVisualAction;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = action.Visualisation;
            var tempColor = UIController.current.selectedActionColors[action.Classification];
            tempColor.a = 0;
            spriteRenderer.color = tempColor;

            var targetColor = spriteRenderer.color;
            targetColor.a = 0.9f;
            
            StartCoroutine(CoroutineHelper.SmoothlyChangeColor(spriteRenderer, tempColor, targetColor, 0.5f));

            Vector3 targetPosition = new Vector3(0f, 0f, 0f);

            if (isPlayer)
            {
                _playerVisualAction.transform.position = new Vector3(-5f, -0.5f, 0f);
                _playerVisualAction.SetActive(true);

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
                StartCoroutine(CoroutineHelper.SmoothLerp(time, _playerVisualAction, targetPosition, resolution));
            }
            else
            {
                _enemyVisualAction.transform.position = new Vector3(5f, -0.5f, 0f);
                _enemyVisualAction.SetActive(true);

                spriteRenderer.flipX = true;

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
                StartCoroutine(CoroutineHelper.SmoothLerp(time, _enemyVisualAction, targetPosition, resolution));
            }
        }
    }

    public void DisableActionVisualisations()
    {
        _playerVisualAction.SetActive(false);
        _enemyVisualAction.SetActive(false);
    }
}
