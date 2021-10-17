using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CoroutineHelper
{
    public static IEnumerator SmoothLerp(float time, GameObject gameObject, Vector3 position, CombatResolution resolution)
    {
        Vector3 startingPos = gameObject.transform.position;
        float elapsedTime = 0;
        var renderer = gameObject.GetComponent<SpriteRenderer>();

        if (elapsedTime >= time)
        {
            var mainColor = renderer.color;
            var color = mainColor;
            color.a = 0f;
            SmoothlyChangeColor(renderer, mainColor, color, 
                resolution == CombatResolution.neglected ? 0.1f : 0.5f);
        }
        while (elapsedTime < time)
        {
            gameObject.transform.position = Vector3.Lerp(startingPos, position, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = position;
        GameplayController.current.ExecuteDelayedActions();
        if (resolution == CombatResolution.neglected)
        {
            Color col = Color.white;
            col.a = 0f;
            SmoothlyChangeColorAndFade(new SpriteRendererAdapter(renderer), renderer.color, Color.white, col, 0.1f, 0.25f);
        }
    }

    public static IEnumerator SmoothlyChangeColor(SpriteRenderer renderer, Color start, Color end, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            renderer.color = Color.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        renderer.color = end;
        yield return null;
    }

    public static IEnumerator SmoothlyChangeColor(Image renderer, Color start, Color end, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            renderer.color = Color.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        renderer.color = end;
        yield return null;
    }

    public static IEnumerator SmoothlyChangeColorAndFade(SpriteImageAdapter renderer, Color start, Color middle, Color end, float colorChangeDuration, float fadeOutDuration)
    {
        float time = 0;
        while (time < colorChangeDuration)
        {
            renderer.color = Color.Lerp(start, middle, time / colorChangeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        renderer.color = middle;

        time = 0;
        while (time < fadeOutDuration)
        {
            renderer.color = Color.Lerp(middle, end, time / fadeOutDuration);
            time += Time.deltaTime;
            yield return null;
        }
        renderer.color = end;
        yield return null;
    }

    public static IEnumerator SmoothlyChangeColorAndFade(TextMeshProUGUI textRenderer, Color start, Color middle, Color end, float colorChangeDuration, float fadeOutDuration)
    {
        float time = 0;
        while (time < colorChangeDuration)
        {
            textRenderer.color = Color.Lerp(start, middle, time / colorChangeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        textRenderer.color = middle;

        time = 0;
        while (time < fadeOutDuration)
        {
            textRenderer.color = Color.Lerp(middle, end, time / fadeOutDuration);
            time += Time.deltaTime;
            yield return null;
        }
        textRenderer.color = end;
        yield return null;
    }
}
