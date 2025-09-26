using UnityEngine;

public class Camera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Rigidbody _playerRigidbody;

    [Header("Settings")]
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        transform.position = _playerTransform.position + offset;
    }



}
