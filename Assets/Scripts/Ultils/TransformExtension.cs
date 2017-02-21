using System.Collections;
using UnityEngine;
public static class TransformExtension
{
    public static IEnumerator Move(this Transform t, Vector3 target, float duration)
    {
        Vector3 distanceVector = target - t.position;
        float distance = distanceVector.magnitude;
        distanceVector.Normalize();

        float countTime = 0;
        while (countTime < duration)
        {
            countTime += Time.deltaTime;
            float n = duration / Time.deltaTime;
            Vector3 translation = distance / n * distanceVector;
            t.Translate(translation);
            yield return null;
        }

        t.position = target;
    }

    public static IEnumerator Scale(this Transform t, Vector3 target, float duration)
    {
        Vector3 distanceVector = target - t.localScale;
        float distance = distanceVector.magnitude;
        distanceVector.Normalize();

        float countTime = 0;
        while (countTime < duration)
        {
            countTime += Time.deltaTime;
            float n = duration / Time.deltaTime;
            Vector3 scale = distance / n * distanceVector;
            t.localScale += scale;
            yield return null;
        }

        t.localScale = target;
    }

}
