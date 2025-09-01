using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{


    public GameObject fireballPrefab;
    public Transform firePoint;
    SpecialAttack Special;
    public float explosionDuration =7f;          // כמה זמן הפיצוץ יישאר במסך
    public GameObject explosionEffectPrefab;      // prefab של אפקט הפיצוץ


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void OnCollisionEnter(Collision collision)
    {
        // ברגע שכדור האש פוגע במשהו (קיר, אויב, קרקע...)

        if (explosionEffectPrefab != null)
        {
            // מייצר את אפקט הפיצוץ במקום הפגיעה
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            // משמיד את אפקט הפיצוץ לאחר כמה שניות
            Destroy(explosion, explosionDuration);
        }

        // משמיד את כדור האש עצמו
        Destroy(gameObject);
    }
    public void Fire()
    {
        // 🧙 יצירת כדור האש
        Vector3 offset = firePoint.forward * 1f + firePoint.up * 1.5f; // חצי מטר קדימה וחצי מטר למעלה (שנה לפי הצורך)

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position + offset, firePoint.rotation);
        // 🚀 הוספת מהירות/כיוון
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        rb.useGravity = false;

        rb.velocity = new Vector3(firePoint.forward.x, 0f, firePoint.forward.z).normalized * 20f;
        Destroy(fireball, 1f);
    }
}
