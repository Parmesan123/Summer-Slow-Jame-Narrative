using System;
using System.Collections;
using UnityEngine;

public class Abyss : MonoBehaviour
{
    [SerializeField] private AnimationCurve abyssCurve;

    private LevelLoader currentLoader;

    private void Awake()
    {
        currentLoader = FindObjectOfType<LevelLoader>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player)) 
        {
            player.Deinitialize();
            StartCoroutine(ObjectInAbyss(other.gameObject, currentLoader.RestartLevel));
        }
        else 
        {
            StartCoroutine(ObjectInAbyss(other.gameObject,
                () => Destroy(other.gameObject)));
        }     
    }

    private IEnumerator ObjectInAbyss(GameObject @object, Action actionToPerform)
    {
        float currentTime = 0f, targetTime = 1f;
        Transform objectTransform = @object.transform;
        Vector3 initialScale = objectTransform.localScale;

        while (currentTime != targetTime)
        {
            currentTime = Mathf.MoveTowards(currentTime, targetTime, Time.deltaTime);
            objectTransform.localScale = Vector3.Lerp(initialScale, Vector3.zero, abyssCurve.Evaluate(currentTime));
            yield return new WaitForEndOfFrame();
        }

        actionToPerform.Invoke();

        yield return new WaitForSeconds(0.2f);
        objectTransform.localScale = initialScale; 
    }
}
