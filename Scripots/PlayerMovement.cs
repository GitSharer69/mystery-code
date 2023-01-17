using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Collider _collider;
    public bool IsDIe;

    public MouseLook mouseLook;

    public GameObject pressE;
    public GameObject pressEtoInteract;

    public bool LookingAtPickupable;

    public float Health = 100f;

    public float Speed = 12;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = -0.4f;
    public LayerMask groundMask;

    public Transform CamHolder;
    public GameObject Ragdoll;

    private Animator animator;
    private bool IsDancing;
    private bool TorchOn;
    public GameObject Torch;

    public GameObject AnimeWoman;

    public Transform NextBot;

    public float Stamina;
    public float sprintSpeed;

    public bool IsSprinting;
    public bool sprintable;

    public bool NotisGroundedExacuta;
    public float StamIncreaseBy;
    public float StamDecreaseBy;

    bool isGrounded;
    public float jumpHeight = 3.5f;

    bool StamHitZero;

    private Vector3 velocity;

    public GameObject DeathScrnContainer;
    public GameObject BarHolder;
    public GameObject OptionsContainer;
    public GameObject Buttons;
    

    private bool IsPaused;
    

    private Image Black;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        IsDIe = false;

        Torch.SetActive(false);

        Black = DeathScrnContainer.transform.Find("Image").GetComponent<Image>();
    }

    void Update()
    {
        if (IsDIe) return;
        AnimeWoman.transform.position = groundCheck.position;

        if (Health <= 0)
        {
            Die();
        }

        if (Health < 25f)
        {
            Health += 0.5f * Time.deltaTime;
        }

        if (Stamina <= 25)
        { StamIncreaseBy = 6f * Time.deltaTime; }
        else
        { StamIncreaseBy = 3f * Time.deltaTime; }

        if (Stamina <= 1f)
        {
            sprintable = false;
            StamHitZero = true;
        }
        else
        { sprintable = true; }

        if (Stamina >= 50f)
        {
            StamHitZero = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && sprintable && !StamHitZero && !IsDancing && isGrounded)
        {
            Speed = 32f;
            Stamina -= StamDecreaseBy * Time.deltaTime;
            IsSprinting = true;


        }
        else
        {
            Speed = 18f;
            IsSprinting = false;
            if (Stamina < 100f)
            {
                Stamina += StamIncreaseBy;
            }

        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        if (!IsDancing)
        {
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * Speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !IsDancing)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!IsDancing)
            {
                IsDancing = true;
            }
            else
            {
                IsDancing = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!TorchOn)
            {
                Torch.SetActive(true);
                TorchOn = true;
            }
            else
            {
                Torch.SetActive(false);
                TorchOn = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit1;
            if (Physics.Raycast(CamHolder.position, CamHolder.transform.forward, out hit1, 10f))
            {

                if (hit1.collider.tag == "HealthBox" && Health < 100f)
                {


                    float Remainder = 100f - Health;

                    if (Remainder < 10f)
                    {
                        Health += Remainder;
                    }
                    else
                    {
                        Health += 10f;
                    }

                    Destroy(hit1.transform.gameObject);
                }
                else if (hit1.collider.tag == "StaminaBox" && Stamina < 100f)
                {
                    float Remainder = 100f - Stamina;

                    if (Remainder < 10f)
                    {
                        Stamina += Remainder;
                    }
                    else
                    {
                        Stamina += 10f;
                    }

                    Destroy(hit1.collider.gameObject);
                }
                
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {

                Resume();
            }

            else
            {
                Buttons.SetActive(true);
                IsPaused = true;
                mouseLook.enabled = false;

                Cursor.lockState = CursorLockMode.None;
                
            }
            
        }
        pressE.SetActive(false);
        pressEtoInteract.SetActive(false);
        RaycastHit hit;
        if (Physics.Raycast(CamHolder.position, CamHolder.transform.forward, out hit, 10f))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                pressE.SetActive(true);
                pressEtoInteract.SetActive(false);
            }
            else if (hit.collider.gameObject.layer == 8)
            {
                pressEtoInteract.SetActive(true);
                pressE.SetActive(false);
            }
            else
            {
                pressE.SetActive(false);
                pressEtoInteract.SetActive(false);
            }


        }



        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);





    }


    private void LateUpdate()
    {
        if (IsDIe) return;

        if (Stamina > 100f)
        {
            Stamina = 100f;
        }
        else if (Stamina < 0f)
        {
            Stamina = 1f;
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (x != 0 || z != 0)
        {

            if (IsSprinting)
            {
                animator.SetBool("runTrigger", true);
                animator.SetBool("walkTrigger", false);
                animator.SetBool("danceTrigger", false);
            }
            else
            {

                animator.SetBool("walkTrigger", true);
                animator.SetBool("runTrigger", false);
                animator.SetBool("danceTrigger", false);

            }


        }
        else
        {
            animator.SetBool("idleTrigger", true);
            animator.SetBool("walkTrigger", false);
            animator.SetBool("runTrigger", false);
            animator.SetBool("danceTrigger", false);
        }

        if (IsDancing)
        {
            animator.SetBool("danceTrigger", true);
            animator.SetBool("idleTrigger", false);
            animator.SetBool("walkTrigger", false);
            animator.SetBool("runTrigger", false);

        }



        if (Vector3.Distance(transform.position, NextBot.position) <= 7f)
        {
            Health -= 5f * Time.deltaTime;
        }
    }

    private void Die()
    {
        _collider.enabled = false;
        controller.enabled = false;

        GameObject Doll = Instantiate(Ragdoll, groundCheck.position, groundCheck.rotation);

        Doll.transform.SetParent(transform);

        CamHolder.SetParent(Doll.transform.Find("Armature/Hips/Spine/Chest/Neck/Head"));

        Doll.transform.Find("Armature/Hips/Spine/Chest/Neck/Head/CamHolder").GetComponent<MouseLook>().enabled = false;


        DeathScrnContainer.SetActive(true);
        StartCoroutine(FadeTo(0.9f, 150));
        StartCoroutine(openOptions());
        AnimeWoman.SetActive(false);
        BarHolder.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        IsDIe = true;
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = Black.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            Black.color = newColor;
            yield return null;
        }


    }

    IEnumerator openOptions()
    {
        yield return new WaitForSeconds(5f);

        OptionsContainer.SetActive(true);
    }

    public void Resume()
    {
        Buttons.SetActive(false);
        IsPaused = false;
        mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

}


