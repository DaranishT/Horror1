using UnityEngine;

public class RadioSway : MonoBehaviour
{
    public float swayAmount = 0.05f; // How much the radio sways
    public float swaySpeed = 2f; // How fast the radio sways

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed) * swayAmount;

        transform.localPosition = initialPosition + new Vector3(swayX, swayY, 0);
    }
}
