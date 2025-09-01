using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{
    //movment
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    //animation
    private Animator animator;
    private CharacterController controller;
    //gravition
    private float verticalVelocity = 0f;
    public float gravity = -9.81f;


    Dictionary<KeyCode, bool> firstStep = new Dictionary<KeyCode, bool>();
    Dictionary<KeyCode, float> comboTimer = new Dictionary<KeyCode, float>();
    Dictionary<KeyCode, bool> isRunning = new Dictionary<KeyCode, bool>();
    KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    public float DoubleClickDely = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        foreach (KeyCode key in keys)
        {
            firstStep[key] = false;
            comboTimer[key] = 0f;
            isRunning[key] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (animator.GetBool("FireBall") != true&&animator.GetBool("KPunch")!=true && animator.GetBool("JPunch") != true) // stoping him from moving while doing sepcial attack
        {
            foreach (KeyCode key in keys)
            {
                HandleDoublePress(key);
                if (isRunning.ContainsValue(true))
                {
                    animator.SetBool("Running", true);

                }
            }

            if (!isRunning.ContainsValue(true))
            {
                Movment();
            }


        }
    }

    


    public void Movment()
{
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    Vector3 direction = new Vector3(h, 0, v);
    if (direction.magnitude > 0.1f)
    {
        Quaternion toRotation = Quaternion.LookRotation(direction); // making the charecter move along the player movment, the vector that makes the player move, the same place he will look at
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        animator.SetBool("isWalking", true);

    }


    else
    {
        animator.SetBool("isWalking", false);
    }


    if (controller.isGrounded)
    {
        verticalVelocity = 0f; // אם על הקרקע - איפוס מהירות נפילה
    }
    else
    {
        verticalVelocity += gravity * Time.deltaTime; // אם לא על הקרקע - הוסף כוח כבידה
    }
    Vector3 move = direction.normalized * moveSpeed; // תנועה אופקית   //direction.normalized make the vector always 1, so if ill press w+d it wont be faster
    move.y = verticalVelocity; // הוסף נפילה אנכית



    controller.Move(move * Time.deltaTime);

}




    void HandleDoublePress(KeyCode key)
    {
        if (Input.GetKeyUp(key))
        {
            firstStep[key] = true;
            comboTimer[key] = 0f;

        }
        if (firstStep[key])
        {
            comboTimer[key] += Time.deltaTime;

            if (comboTimer[key] > DoubleClickDely)
            {
                firstStep[key] = false;
            }

            else
            {
                // לחיצה שנייה → הפעלת ריצה
                if (Input.GetKeyDown(key))
                {
                    isRunning[key] = true;
                }
            }
        }
        // אם מצב ריצה פעיל
        if (isRunning[key])
        {
            // אם שחררנו את הכפתור – עצור ריצה
            if (Input.GetKeyUp(key))
            {
                isRunning[key] = false;
            }
            else
            {
                Run(key);
            }
        }
        else
        {
            animator.SetBool("Running", false);
        }
    }  // if you press run and then press with the other keys run as well it make you faster

    void Run(KeyCode key)
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0, v);

        // סיבוב לכיוון התנועה
        if (direction.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // הפעלת אנימציית ריצה

        // טיפול בכבידה
        if (controller.isGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // תנועה
        Vector3 move = direction.normalized * moveSpeed * 2; // ריצה = פי 2 ממהירות רגילה
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    } 

    /*void Jump()
    {

    }*/


}