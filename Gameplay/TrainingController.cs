using UnityEngine;

public class TrainingController : MonoBehaviour
{
    private int currentIteration = 0;

    [SerializeField] GameObject[] windows;
    [SerializeField] ArrowDisplayState[] arrowsStates;

    void Awake()
    {
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
            GameplayController.current.StartGame();
        }
    }


    private void OpenWindow(int index, bool enabled)
    {
        windows[index].SetActive(enabled);
        ArrowController.current.ResolveArrowDisplay(arrowsStates[index], enabled);
    }

}

