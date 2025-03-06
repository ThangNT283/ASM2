using UnityEngine;

public class Bridge : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            GameController.Instance.IsLose = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Plane"))
        {
            GameController.Instance.IsLose = true;
        }
    }
}

