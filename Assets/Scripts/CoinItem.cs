using UnityEngine;

public class CoinItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.Instance.AddScore(1);
            Destroy(gameObject);
        }
    }
}
