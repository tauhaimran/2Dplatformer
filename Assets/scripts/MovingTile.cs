using UnityEngine;
using DG.Tweening;

public class MovingTile : MonoBehaviour
{
    [Header("World Positions (exact world coordinates)")]
    public Vector3 startPos = new Vector3(26.76f, 10.64f, 0f);
    public Vector3 endPos = new Vector3(26.76f, -9.2f, 0f);

    [Header("Movement Settings")]
    public float moveTime = 2f;   // time for one way
    public float pauseTime = 0.5f; // pause at each end
    public Ease easeType = Ease.InOutSine;

    private Sequence seq;

    void Start()
    {
        // Stop any existing tween
        DOTween.Kill(transform);

        // Move platform to starting position in WORLD space
        transform.position = startPos;

        // Build the movement sequence (WORLD space only)
        seq = DOTween.Sequence();
        seq.Append(transform.DOMove(endPos, moveTime).SetEase(easeType).SetRelative(false))
           .AppendInterval(pauseTime)
           .Append(transform.DOMove(startPos, moveTime).SetEase(easeType).SetRelative(false))
           .AppendInterval(pauseTime)
           .SetLoops(-1, LoopType.Restart);
    }

    private void OnDestroy()
    {
        if (seq != null && seq.IsActive())
            seq.Kill();
    }

    // Optional (for 2D player sticking)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(null);
    }
}
