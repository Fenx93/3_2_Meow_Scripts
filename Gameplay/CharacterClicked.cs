using UnityEngine;

public class CharacterClicked : MonoBehaviour
{
    [SerializeField] private bool isPlayer;

    private Collider2D _collider;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        GameplayController.current.RefreshActionButtons(isPlayer);
    }
}
