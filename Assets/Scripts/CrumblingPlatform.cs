using UnityEngine;
using System.Collections;
public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] private Transform visualChild;
    [SerializeField] private float standTime;
    [SerializeField] private float respawnTime;
    [SerializeField] private float shakeAmount;

    private Vector3 originalPosition;
    private SpriteRenderer sr;
    private Collider2D col;
    private bool isCrumbling = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
        if(visualChild == null)
        {
            return;
        }
        sr = visualChild.GetComponent<SpriteRenderer>();
        originalPosition = visualChild.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCrumbling)
        {
            StartCoroutine(CrumblePlatform());
        }
    }

    IEnumerator CrumblePlatform()
    {
        isCrumbling = true;
        float timer = 0f;

        while (timer < standTime)
        {
            float x = originalPosition.x + Random.Range(-shakeAmount, shakeAmount);
            float y = originalPosition.y + Random.Range(-shakeAmount, shakeAmount);
            visualChild.localPosition = new Vector3(x, y, originalPosition.z);

            timer += Time.deltaTime;
            yield return null;
        }

        visualChild.localPosition = originalPosition;

        sr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        sr.enabled = true;
        col.enabled = true;
        isCrumbling = false;
    }
}

