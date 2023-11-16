using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class vsTwo : MonoBehaviour
{
    public float interval = 3.0f;
    private Renderer objectRenderer;
    // Start is called before the first frame update
    void Start()
    {
          objectRenderer = GetComponent<Renderer>();

        // Start the coroutine
        StartCoroutine(ToggleVisibilityRoutine());
    }

    IEnumerator ToggleVisibilityRoutine()
    {
        yield return new WaitForSeconds(interval);
         while(true)
        {

            // Wait for specified interval
            yield return new WaitForSeconds(interval);

            // Toggle the Renderer's visibility
            objectRenderer.enabled = !objectRenderer.enabled;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
