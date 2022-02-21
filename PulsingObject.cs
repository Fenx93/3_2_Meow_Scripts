using UnityEngine;

public class PulsingObject : MonoBehaviour
{
    [SerializeField] private float time = 2f;
    private void Start()
    {
        var rect = this.GetComponent<RectTransform>();
        LeanTween.scale(rect, new Vector3(1.2f, 1.2f, 1.2f), time).setLoopPingPong();
    }
}
