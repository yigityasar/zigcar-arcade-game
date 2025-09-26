using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Text _scoreText;


    private void Update()
    {
        _scoreText.text = _playerTransform.position.z.ToString("0");
    }
}
