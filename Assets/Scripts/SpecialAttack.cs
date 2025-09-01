using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpecialAttack : MonoBehaviour
{
    //animation

    private Animator animator;

    // special attaks FireBall animation
    private bool firstStepDone = false;   // האם כבר לחצנו ושחררנו K
    private bool SecondStepDone = false;   // האם כבר לחצנו ושחררנו K
    private float comboTimer = 0f;        // טיימר כדי לאפס אם עבר יותר מדי זמן
    public float maxComboDelay = 0.05f;
    public GameObject fireballPrefab;
    public Transform firePoint;

    private float[,] originalHeights;


    public Terrain terrain;
    public float raiseSpeed = 0.1f;   // כמה להרים את הקיר בכל שניה (ערך יחסית)
    public float wallWidth = 2f;       // רוחב הקיר במטרים (הקיר ירים את הקרקע מימין ומשמאל עד רוחב זה)
    public float wallDepth = 1f;       // עומק הקיר במטרים (כמה הקיר "נמתח" קדימה)
    public float speed = 1f;           // מהירות העלייה (כדי לשלוט בקצב השינוי)

    private TerrainData terrainData;
    private int heightmapWidth;
    private int heightmapHeight;


    public GameObject dustEffect;

    public Health targetHealth;// tragets hp

    void OnApplicationQuit() //reset area
    {
        terrainData.SetHeights(0, 0, originalHeights);

    }

    void Start()
    {
        animator = GetComponent<Animator>();
        // שולף את ה-terrainData (המידע של הגובה) מה-terrain שהגדרת ב-Inspector
        terrainData = terrain.terrainData;

        // שומר את הרזולוציה של מפת הגבהים (מספר נקודות רוחב ואורך)
        heightmapWidth = terrainData.heightmapResolution;
        heightmapHeight = terrainData.heightmapResolution;
        originalHeights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        Health PlayersHealth = GetComponent<Health>(); // the player hp

    }
    private bool EnterDustOnce = false;
    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        fireball.firePoint = firePoint;
        fireball.fireballPrefab= fireballPrefab;
        Attack(KeyCode.K, KeyCode.J, "FireBall", "FireBallTrigger", () => StartCoroutine(ShootFireBallInDilay()));

        Vector3 wallPosition = transform.position + transform.forward * 2f; // יוצא 2 מטר קדימה מול הפנים


        SpecialAttackThreeKeys(KeyCode.I, KeyCode.O, KeyCode.P, "LowerGround", "LowerTrigger", () => LowerGround(wallPosition));
        SpecialAttackThreeKeys(KeyCode.P, KeyCode.O, KeyCode.I, "RaiseGround", "RaiseGroundTrigger", () => RisingGround(wallPosition));

        if (!animator.GetBool("LowerGround") && !animator.GetBool("RaiseGround"))
        {
            EnterDustOnce = false;
        }
        if (animator.GetBool("RaiseGround"))
        {
            RisingGround(wallPosition);


         
            if (EnterDustOnce == false)
            {
                Vector3 spawnPosition = transform.position + new Vector3(0, 1.5f, 0); // 0.5f למעלה בציר Y
                GameObject dust = Instantiate(dustEffect, spawnPosition, Quaternion.identity);

                Destroy(dust, 1.8f); // מנקה את האובייקט אחרי שהאפקט נגמר
                EnterDustOnce = true;
                if (CameraMovment.instance != null)
                    StartCoroutine(CameraMovment.instance.Shake(0.3f, 0.3f)); // (משך, עוצמה
            }

        }
        if (animator.GetBool("LowerGround"))
        {
            LowerGround(wallPosition);
           
            if (EnterDustOnce == false)
            {
                GameObject dust = Instantiate(dustEffect, transform.position, Quaternion.identity);
                Destroy(dust, 1.8f); // מנקה את האובייקט אחרי שהאפקט נגמר
                EnterDustOnce = true;
                if (CameraMovment.instance != null) // camera shake
                    StartCoroutine(CameraMovment.instance.Shake(0.3f, 0.3f)); // (משך, עוצמה
            }

        }
       
        
        KPunch();
        JPunch();


}



    public void Attack(KeyCode First, KeyCode Second, string Animation, string Trigger, Action Ability)//Attack for two keys. the ability is the function of the abilty
    {
        // שלב 1: לוחצים ומשחררים K
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 0.3f && stateInfo.IsName("KPunch"))
        {
            return;
        }

        bool isPlaying = stateInfo.IsName(Animation) && stateInfo.normalizedTime < 1f;



        if (Input.GetKeyUp(First) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A)  // you need to press only that keys or it wouldnt work
                 && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !isPlaying)
        {
            firstStepDone = true;        // סימון שהשלב הראשון הצליח
            comboTimer = 0f;             // מאפסים את הטיימר

        }

        // אם סיימנו את השלב הראשון – מתחילים לספור זמן
        if (firstStepDone)
        {
            comboTimer += Time.deltaTime;

            // שלב 2: לוחצים ומשחררים J

            // אם עבר יותר מדי זמן → מאפסים
            if (comboTimer > maxComboDelay)
            {

                firstStepDone = false;



            }

            if ((Input.GetKeyUp(Second) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A)  // you need to press only that keys or it wouldnt work
                 && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))) //enters special attack
            {


                if (!animator.GetBool(Animation)) // de entring it from going into a que, if it happend once, wait till the end of it
                {

                    animator.SetTrigger(Trigger); // making animation
                    animator.SetBool(Animation, true);
                    Ability();

                    // alreadyFired = true;

                }
                firstStepDone = false;  // מאפס אחרי ההתקפה

            }


        }


        if (stateInfo.normalizedTime >= 1f && stateInfo.IsName(Animation))//cheacks which animation is working and if it finished changing back to false
        {
            animator.SetBool(Animation, false);

        }
    }
  

    public void SpecialAttackThreeKeys(KeyCode FirstOne, KeyCode SecondOne, KeyCode ThirdOne, string Animation, string Trigger, Action Ability)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = stateInfo.IsName(Animation) && stateInfo.normalizedTime < 1f;

        if (Input.GetKeyUp(FirstOne) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A)  // you need to press only that keys or it wouldnt work
                 && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !isPlaying)
        {
            firstStepDone = true;        // סימון שהשלב הראשון הצליח
            comboTimer = 0f;             // מאפסים את הטיימר


        }

        // אם סיימנו את השלב הראשון – מתחילים לספור זמן
        if (firstStepDone)
        {
            comboTimer += Time.deltaTime;

            // שלב 2: לוחצים ומשחררים J

            // אם עבר יותר מדי זמן → מאפסים
            if (comboTimer > maxComboDelay)
            {
                firstStepDone = false;
            }
            if ((Input.GetKeyUp(SecondOne) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A)  // you need to press only that keys or it wouldnt work
                 && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))) //enters special attack
            {


                SecondStepDone = true;
                comboTimer = 0f;

            }
        }
        if (SecondStepDone)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > maxComboDelay)
            {
                firstStepDone = false;
                SecondStepDone = false;
            }
            if ((Input.GetKeyUp(ThirdOne) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A)  // you need to press only that keys or it wouldnt work
                 && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))) //enters special attack{
            {

                animator.SetTrigger(Trigger); // making animation
                animator.SetBool(Animation, true);
                Ability();


                firstStepDone = false;  // מאפס אחרי ההתקפה
                SecondStepDone = false;  // מאפס אחרי ההתקפה
            }

        }
        if (stateInfo.normalizedTime >= 0.8f && stateInfo.IsName(Animation))//cheacks which animation is working and if it finished changing back to false
        {
            animator.SetBool(Animation, false);



        }
    }



    public void OneAbilityAttack(KeyCode key, string Animation, Action Ability)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = stateInfo.IsName(Animation) && stateInfo.normalizedTime < 1f;

        if (Input.GetKeyDown(key) && !isPlaying)
        {
            animator.SetBool(Animation, true);

            Ability();
        }
        if (stateInfo.normalizedTime >= 0.8f && stateInfo.IsName(Animation))//cheacks which animation is working and if it finished changing back to false
        {

            animator.SetBool(Animation, false);


        }
    }


    public void JPunch()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = stateInfo.IsName("JPunch") && stateInfo.normalizedTime < 1f;

        if (Input.GetKeyDown(KeyCode.J) && !isPlaying && animator.GetBool("isWalking") == false && animator.GetBool("Running") == false)
        {

            animator.SetBool("JPunch", true);
            animator.SetTrigger("JTrigger");
        }

        if (stateInfo.normalizedTime >= 0.8f && stateInfo.IsName("JPunch")/*im doing that so it wont cancle right away the animatorbool because it refernce to the animation that is working right now(idle)*/)//cheacks which animation is working and if it finished changing back to false
        {
            animator.SetBool("KPunch", false);
            animator.ResetTrigger("KTrigger");
            animator.SetBool("JPunch", false);
            animator.ResetTrigger("JTrigger");

        }
        if (animator.GetBool("FireBall"))//if his doing specail ability make everyone false
        {
            animator.SetBool("KPunch", false);
            animator.ResetTrigger("KTrigger");
            animator.SetBool("JPunch", false);
            animator.ResetTrigger("JTrigger");

        }

    }
    public void KPunch()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = stateInfo.IsName("KPunch") && stateInfo.normalizedTime < 1f;

        if (Input.GetKeyDown(KeyCode.K) && !isPlaying && animator.GetBool("isWalking") == false && animator.GetBool("Running") == false)
        {

            animator.SetBool("KPunch", true);
            animator.SetTrigger("KTrigger");
        }

        if (stateInfo.normalizedTime >= 0.8f && stateInfo.IsName("KPunch")/*im doing that so it wont cancle right away the animatorbool because it refernce to the animation that is working right now(idle)*/)//cheacks which animation is working and if it finished changing back to false
        {
            animator.SetBool("KPunch", false);
            animator.ResetTrigger("KTrigger");
            animator.SetBool("JPunch", false);
            animator.ResetTrigger("JTrigger");

        }
         if (animator.GetBool("FireBall"))//if his doing specail ability make everyone false
        {
            animator.SetBool("KPunch", false);
            animator.ResetTrigger("KTrigger");
            animator.SetBool("JPunch", false);
            animator.ResetTrigger("JTrigger");

        }
      
    }
    public void RisingGround(Vector3 worldPosition)
    {
        // ממיר מיקום עולמי (world) למיקום על גבי ה־Terrain
        Vector3 terrainPos = worldPosition - terrain.transform.position;

        TerrainData terrainData = terrain.terrainData;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // ממיר את מיקום העולם ליחס בגובה
        int mapX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * heightmapWidth);
        int mapZ = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * heightmapHeight);

        // כמה נקודות נרים מסביב (קיר קטן)
        int range = 3;

        // מוודא שאנחנו לא חורגים מהמפה
        int startX = Mathf.Clamp(mapX - range, 0, heightmapWidth - 1);
        int startZ = Mathf.Clamp(mapZ - range, 0, heightmapHeight - 1);
        int width = Mathf.Clamp(mapX + range, 0, heightmapWidth) - startX;
        int depth = Mathf.Clamp(mapZ + range, 0, heightmapHeight) - startZ;

        // שואב את הגבהים הקיימים
        float[,] heights = terrainData.GetHeights(startX, startZ, width, depth);

        // מעלה אותם
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                heights[z, x] += 0.008f; // מעלה מעט – תוכל לשנות
            }
        }

        // מחזיר את השינוי
        terrainData.SetHeights(startX, startZ, heights);
      
    }
    public void LowerGround(Vector3 worldPosition)
    {
        // ממיר מיקום עולמי (world) למיקום על גבי ה־Terrain
        Vector3 terrainPos = worldPosition - terrain.transform.position;

        TerrainData terrainData = terrain.terrainData;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // ממיר את מיקום העולם ליחס בגובה
        int mapX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * heightmapWidth);
        int mapZ = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * heightmapHeight);

        // כמה נקודות נרים מסביב (קיר קטן)
        int range = 3;

        // מוודא שאנחנו לא חורגים מהמפה
        int startX = Mathf.Clamp(mapX - range, 0, heightmapWidth - 1);
        int startZ = Mathf.Clamp(mapZ - range, 0, heightmapHeight - 1);
        int width = Mathf.Clamp(mapX + range, 0, heightmapWidth) - startX;
        int depth = Mathf.Clamp(mapZ + range, 0, heightmapHeight) - startZ;

        // שואב את הגבהים הקיימים
        float[,] heights = terrainData.GetHeights(startX, startZ, width, depth);

        // מעלה אותם
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                heights[z, x] -= 0.008f; // מעלה מעט – תוכל לשנות
            }
        }

        // מחזיר את השינוי
        terrainData.SetHeights(startX, startZ, heights);
      
    }
    IEnumerator ShootFireBallInDilay()
    {
        yield return new WaitForSeconds(0.55f); // מחכה 2 שניות
        fireball.Fire();
    }
   public FireBall fireball;
   /* void Fire()
    {
        // 🧙 יצירת כדור האש
        Vector3 offset = firePoint.forward * 1f + firePoint.up * 1.5f; // חצי מטר קדימה וחצי מטר למעלה (שנה לפי הצורך)

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position + offset, firePoint.rotation);
        // 🚀 הוספת מהירות/כיוון
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        rb.useGravity = false;

        rb.velocity = new Vector3(firePoint.forward.x, 0f, firePoint.forward.z).normalized * 20f;
        Destroy(fireball, 1f);
    }*/
}

