using System.Collections;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            _playerMovement.enabled = false;
            StartCoroutine(DelayedEndGame());
        }
    }
    
    private IEnumerator DelayedEndGame()
    {
        yield return new WaitForSeconds(2.5f); // 3 saniye bekle
        FindFirstObjectByType<GameManager>().EndGame();
    }
}
