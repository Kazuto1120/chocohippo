//jorge lata
//composing a c# script to make gameobjects shrink and expand
//the gameobject will continuously shrink and expand


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class changeSize inherits function from MonoBehaviour
public class changeSize : MonoBehaviour
{
    public float minimoScale = 0.8f; // represents Minimum scale value
    public float maximoScale = 2f; // represents Maximum scale value
    public float scaleSpeed = 1f; // represents speed at which the object scales

    private bool isExpanding = true; // used to track if the Gameobject is expanding or shrinking

    private void Update()
    {
        // This line calculates the new scale based on the current scale and the speed
        float newScale = transform.localScale.x + (scaleSpeed * Time.deltaTime * (isExpanding ? 1 : -1));

        // The function does the following:
        //Returns the minimum value if the given float value is less than the minimum value.
        //Returns the maximum value if the given value is greater than the maximum value.
        //Use Clamp to restrict a value to a range that is defined by the minimum and maximum values.
        newScale = Mathf.Clamp(newScale, minimoScale, maximoScale);

        // implements the new scale to all axes of the object
        transform.localScale = new Vector3(newScale, newScale, newScale);

        // Reverses the direction of the scaling
        if (newScale <= minimoScale || newScale >= maximoScale)
            isExpanding = !isExpanding;
    }
} //end
