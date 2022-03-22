using UnityEngine;

public class PulsingObject : MonoBehaviour
{
    [SerializeField] private float time = 2f;
    [SerializeField] private Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);
    private void Start()
    {
        var rect = this.GetComponent<RectTransform>();
        LeanTween.scale(rect, targetScale, time).setLoopPingPong();
    }
}
