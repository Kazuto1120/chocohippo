using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerashake : MonoBehaviour
{
     public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalpos = transform.localPosition;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalpos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
