using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary 
{
    public Vector2 Bounds{get; private set;}
    public void CalculateScreenRestrictions(){
        // Get the corners of the screen based on the viewport points
        Vector3 upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Bounds = new Vector2(upperRight.x, upperRight.y);
    }
}
