using UnityEngine;
using TMPro;

public class EntityProximityText : MonoBehaviour
{
    public Transform entity;
    public Transform player;
    public TextMeshProUGUI horrorText;

    public float farDistance = 25f;
    public float midDistance = 15f;
    public float closeDistance = 7f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, entity.position);

        if (distance < closeDistance)
        {
            horrorText.text = "It is here";
        }
        else if (distance < midDistance)
        {
            horrorText.text = "It sees you";
        }
        else if (distance < farDistance)
        {
            horrorText.text = "It is coming";
        }
        else
        {
            horrorText.text = ""; // Hide text when too far
        }
    }
}
