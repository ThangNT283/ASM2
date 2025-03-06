using UnityEngine;

public class StarItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.Instance.AddScore(3);
            Destroy(gameObject);
        }
    }
}
