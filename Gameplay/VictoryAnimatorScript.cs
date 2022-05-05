using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static InventorySettings;

public class VictoryAnimatorScript : MonoBehaviour
{
    public GameObject cinematographicBars;
    public static VictoryAnimatorScript current;
    [SerializeField] private Sprite defeatedEyes;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private GameObject[] hideableUIObjects;

    private Transform initialCameraPos, _loser;
    private bool _playerWon, _isDraw;
    private string _message;
    private int _money, _exp;

    private void Awake()
    {
        current = this;
        particles.Stop();
        initialCameraPos = Camera.main.transform;
        cinematographicBars.SetActive(false);
    }

    public void TransitBarsToDisplayed()
    {
        cinematographicBars.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.black;
        cinematographicBars.transform.GetChild(1).gameObject.GetComponent<Image>().color = Color.black;
        cinematographicBars.SetActive(true);
    }

    public void TransitBarsToEmpty()
    {
        var image0 = cinematographicBars.transform.GetChild(0).gameObject.GetComponent<Image>();
        var image1 = cinematographicBars.transform.GetChild(1).gameObject.GetComponent<Image>();

        StartCoroutine(CoroutineHelper.SmoothlyChangeColor(image0, Color.black, new Color(0, 0, 0, 0), 0.5f));
        StartCoroutine(CoroutineHelper.SmoothlyChangeColor(image1, Color.black, new Color(0, 0, 0, 0), 0.5f));
    }

    public void SetValues(string message, int money, int exp)
    {
        _message = message;
        _money = money;
        _exp = exp;
    }

    private void CountdownEnded(int status)
    {
        switch (status)
        {
            case 1:
                TransitBarsToDisplayed();
                //wait for some time
                StartCoroutine(Wait(2f, status));
                break;
            case 2:
                cinematographicBars.SetActive(false);
                //zoom to loser's face
                AudioController.current.PlayCameraTransitionSound();
                StartCoroutine(SmoothlyLookAt(_loser, Camera.main.transform, 0.5f, 1.5f, status));
                break;
            case 3:
                cinematographicBars.SetActive(true);
                AudioController.current.PlayCharacterLosingSound();
                //wait for some time
                StartCoroutine(Wait(1f, status));
                break;
            case 4:
                //change loser's eyes to xx
                CharacterCustomizer.current.avatars[_playerWon ? 1 : 0].SetSprite(defeatedEyes, CharacterPart.eyes);
                if (_isDraw)
                {
                    CharacterCustomizer.current.avatars[_playerWon ? 0 : 1].SetSprite(defeatedEyes, CharacterPart.eyes);
                }
                cinematographicBars.SetActive(false);
                //wait for some time
                StartCoroutine(Wait(0.5f, status));
                break;
            case 5:
                particles.Stop();
                // zoom out back
                StartCoroutine(SmoothlyLookAt(initialCameraPos, Camera.main.transform, 1f, 5f, status));
                break;
            case 6:
                cinematographicBars.SetActive(false);
                if (_playerWon)
                {
                    AudioController.current.PlayWinningSound();
                }
                else
                {
                    AudioController.current.PlayLosingSound();
                }
                FinishMatchUI.current.ShowGameEndMessage(_message, _money, _exp);
                break;
            default:
                break;
        }
    }

    public void StartAnimation(Transform winner, Transform loser, bool playerWon, bool isDraw = false)
    {
        _loser = loser;
        _playerWon = playerWon;
        _isDraw = isDraw;
        //Start anime lines
        particles.Play();
        foreach (var gameObj in hideableUIObjects)
        {
            gameObj.SetActive(false);
        }

        AudioController.current.PlayCameraTransitionSound();
        StartCoroutine(SmoothlyLookAt(winner, Camera.main.transform, 0.75f, 1.5f, 0));
    }

    private IEnumerator SmoothlyLookAt(Transform lookAtTarget, Transform camera, float duration, float targetOrthSize, int status)
    {
        Vector3 lookDirection = lookAtTarget.position - camera.position;
        lookDirection.Normalize();
        var orthSize = Camera.main.orthographicSize;

        var time = 0f;
        while (time < duration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(orthSize, targetOrthSize, time);//1.05f;
            camera.rotation = Quaternion.Slerp(camera.rotation, lookDirection != Vector3.zero? Quaternion.LookRotation(lookDirection):  Quaternion.identity, time);
            time += Time.deltaTime;
            yield return null;
        }
        status++;
        CountdownEnded(status);
        yield return null;
    }

    private IEnumerator Wait(float duration, int status)
    {
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        status++;
        CountdownEnded(status);
        yield return null;
    }
}
