using System.Collections;
using UnityEngine;

public class TerrainShake : MonoBehaviour
{
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TriggerShake(float duration, float magnitude)
    {
        Debug.Log("TriggerShake called!");

        StartCoroutine(ShakeWithSound(duration, magnitude));
    }

    IEnumerator ShakeWithSound(float duration, float magnitude)
    {
        audioSource.Play();
        yield return StartCoroutine(Shake(duration, magnitude));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, 0, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}