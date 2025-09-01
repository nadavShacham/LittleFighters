using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    public float PlayerHealth = 100;
    public float CurrentPlayerhp;

    public Image healthBarFill; // נגרור לכאן את פס החיים מתוך ה-UI

    // Start is called before the first frame update
    void Start()
    {
        CurrentPlayerhp = PlayerHealth;
        UpdateHealthUI();

    }


    void TakingDamge(int NumberOfDamge)
    {
        CurrentPlayerhp = CurrentPlayerhp - NumberOfDamge;
        if (CurrentPlayerhp <= 0)
        {
            //player dead
            Debug.Log("you are dead");
            Destroy(gameObject);
        }
        UpdateHealthUI();

    }

    private void UpdateHealthUI()
    {
        float normalizedHealth = CurrentPlayerhp / PlayerHealth;

        // עוצר כל אנימציה קודמת
        StopAllCoroutines();

        // מתחיל מעבר חדש
        StartCoroutine(AnimateHealthBar(healthBarFill.fillAmount, normalizedHealth));
    }

    // ← מעבר חלק מ-fillAmount נוכחי ליעד
    private IEnumerator AnimateHealthBar(float from, float to)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            healthBarFill.fillAmount = Mathf.Lerp(from, to, t);
            yield return null;
        }

        healthBarFill.fillAmount = to;
    }


    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.P))
            TakingDamge(12);*/
    }
}
