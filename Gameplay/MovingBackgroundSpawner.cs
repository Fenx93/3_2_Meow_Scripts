using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackgroundSpawner : MonoBehaviour
{
    [SerializeField] private Transform parentObject;
    [SerializeField] private Sprite[] objectSprites;
    [SerializeField] private bool moveRight, flipSprites;
    [SerializeField] private float minY, maxY, 
        minX, maxX,
        minScale, maxScale,
        minSpeed, maxSpeed,
        minSpawnTime = 0.1f, maxSpawnTime = 2.5f;
    [SerializeField] private int sortingOrder = 12;
    private float timeLeft = 0.1f;
    private List<GameObject> bgObjects = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        if (parentObject == null)
        {
            parentObject = this.transform;
        }
        for (int i = 0; i < Random.Range(5, 20); i++)
        {
            SpawnCloud(true);
        }
    }

    private void FixedUpdate()
    {
        for (int i = bgObjects.Count - 1; i >= 0; i--)
        {
            var obj = bgObjects[i];
            if ((obj.transform.localPosition.x > maxX || obj.transform.localPosition.x < minX))
            {
                bgObjects.RemoveAt(i);
                Destroy(obj);
            }
            else
            {
                Vector3 vec = (obj.GetComponent<MovingBackgroundObject>().moveRight) ?
                        Vector3.right 
                        : Vector3.left;

                obj.transform.Translate(vec * obj.GetComponent<MovingBackgroundObject>().speed);
            }
        }
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = Random.Range(minSpawnTime, maxSpawnTime);
            SpawnCloud();
        }
    }

    void SpawnCloud(bool fillScreen = false)
    {
        //Instantiate at random position with random size random objects\\
        GameObject newBackObj = new GameObject("Background Object");
        var sRenderer = newBackObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        sRenderer.sprite = objectSprites[Random.Range(0, (objectSprites.Length))];
        if (flipSprites)
        {
            sRenderer.flipX = Random.Range(0f, 1f) > 0.5f;
        }
        sRenderer.sortingOrder = Random.Range(0f, 1f) > 0.75f? 10 : 0;

        //change opacity
        Color c = sRenderer.color;
        c.a = Random.Range(0.4f, 1f);
        sRenderer.color = c;

        var scale = Random.Range(minScale, maxScale);
        newBackObj.transform.localScale = new Vector3(scale, scale, 1);

        var movBackObj = newBackObj.AddComponent(typeof(MovingBackgroundObject)) as MovingBackgroundObject;
        movBackObj.speed = Random.Range(minSpeed, maxSpeed);
        movBackObj.moveRight = moveRight;

        newBackObj.transform.parent = parentObject;
        if (fillScreen)
        {
            newBackObj.transform.localPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
        }
        else
        {
            var xPos = moveRight ?
                 minX : maxX;
            newBackObj.transform.localPosition = new Vector3(xPos, Random.Range(minY, maxY), 0);
        }

        if (newBackObj.transform.position.y < 0)
        {
            sRenderer.sortingOrder = sortingOrder;
        }
        bgObjects.Add(newBackObj);
    }
}


[System.Serializable]
public class MovingBackgroundObject : MonoBehaviour
{
    public float speed;
    public bool moveRight;
}
