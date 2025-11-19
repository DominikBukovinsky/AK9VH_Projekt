using UnityEngine;

public class FrogRotator : MonoBehaviour
{
    public float rychlost = -10f; 

    void Update()
    {
        transform.Rotate(0, 0, rychlost * Time.unscaledDeltaTime);
    }
}