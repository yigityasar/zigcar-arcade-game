using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody;
    [Header("Settings")]
    [SerializeField] private float sidewaysForce = 900f;
    [SerializeField] private float forwardForce = 2000f; // ileri kuvvet
    [SerializeField] private float tiltAngle = 15f;       // sola/sağa yatma açısı
    [SerializeField] private float tiltSpeed = 5f;        // düzelme hızı

    private float targetTilt = 0f; // hedef açı
    private Vector3 baseEuler = new Vector3(-90f, 0f, 0f);


    private void FixedUpdate()
    {
        _rigidbody.AddForce(0, 0, forwardForce * Time.deltaTime);
        if (Input.GetKey("d"))
        {
            _rigidbody.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            targetTilt = -tiltAngle;
        }

        else if (Input.GetKey("a"))
        {
            _rigidbody.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            targetTilt = tiltAngle;

        }
        else
            targetTilt = 0f;

        Quaternion targetRotation = Quaternion.Euler(baseEuler.x, baseEuler.y, -targetTilt);
        _rigidbody.MoveRotation(Quaternion.Lerp(_rigidbody.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime));


        if (_rigidbody.position.y < -1f)
            FindFirstObjectByType<GameManager>().EndGame();
    }
}
