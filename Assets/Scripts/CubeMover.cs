using UnityEngine;
using Random = UnityEngine.Random;

public class CubeMover : MonoBehaviour
{
    private Color _wallColor;
    private Renderer _renderer;
    private Vector3 _movementDirection = Vector3.zero;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        RandomWallColor();
    }

    public Color GetColor()
    {
        return _wallColor;
    }
    void FixedUpdate()
    {
        transform.Translate(_movementDirection);
    }
    private void RandomWallColor()
    {
        _wallColor = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
        _renderer.material.color = _wallColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            _movementDirection *= -1;
        }
    }

    public void InitMovement(Vector3 speed)
    {
        _movementDirection = speed;
    }
}
