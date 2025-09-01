using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    public Transform Player;
    public Vector3 offset;

    public static CameraMovment instance;

    private Vector3 shakeOffset = Vector3.zero; // פה נשמור את הרעידה

    private void Awake()
    {
        instance = this;
    }

    void LateUpdate()
    {
        // בסיס המצלמה + רעידה
        transform.position = Player.position + offset + shakeOffset;
        transform.LookAt(Player);
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0); // מוסיף רעידה

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero; // מאפס רעידה
    }
}
