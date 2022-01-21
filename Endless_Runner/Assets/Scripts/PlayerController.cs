using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private CapsuleCollider col;
    private Animator anim;
    private Vector3 dir;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private int coins;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Text coinsText;
    [SerializeField] private Score scoreScript;

    private bool IsSliding;

    private int lineToMove = 1;
    public float lineDistance = 4;
    private float maxSpeed = 110;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        col = GetComponent<CapsuleCollider>();
        StartCoroutine(SpeedIncrease());
        Time.timeScale = 1;
        coinsText.text = "Coins: " + coins.ToString();
    }

    private void Update()
    {
        if(SwipeController.swipeRight)
        {
            if(lineToMove < 2)
            lineToMove++;
        }

        if(SwipeController.swipeLeft)
        {
            if(lineToMove > 0)
            lineToMove--;
        }

        if(SwipeController.swipeUp)
        {
            if(controller.isGrounded)
            Jump();
        }

        if (SwipeController.swipeDown)
        {
            StartCoroutine(Slide());
        }

        if (controller.isGrounded && !IsSliding)
        anim.SetBool("IsRunning", true);
        else
        anim.SetBool("IsRunning", false);
        
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if(lineToMove == 0)
        targetPosition += Vector3.left * lineDistance;
        else if(lineToMove == 2)
        targetPosition += Vector3.right * lineDistance;

        if(transform.position == targetPosition)
        return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if(moveDir.sqrMagnitude < diff.sqrMagnitude)
        controller.Move(moveDir);
        else
        controller.Move(diff);     
    }

    private void Jump()
    {
        dir.y = jumpForce;
        anim.SetTrigger("IsJumping");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dir.z = speed;
        dir.y += gravity * Time.fixedDeltaTime;
        controller.Move(dir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "obstacle")
        {
           losePanel.SetActive(true);
           int lastRunScore = int.Parse(scoreScript.scoreText.text.ToString());
           PlayerPrefs.SetInt("lastRunScore", lastRunScore);
           Time.timeScale = 0;  
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "coins")
        {
            coins++;
            coinsText.text = "Coins: " + coins.ToString();
            Destroy(other.gameObject);
        }
    }

    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(3);
        if(speed < maxSpeed)
        {
           speed += 2;
            StartCoroutine(SpeedIncrease()); 
        }
    }

    private IEnumerator Slide()
    {
        col.center = new Vector3(0, -0.5f, 0);
        col.height = 2;
        IsSliding = true;
        anim.SetTrigger("IsSliding");

        yield return new WaitForSeconds(1);

        col.center = new Vector3(0, 0, 0);
        col.height = 4.420422f;
        IsSliding = false;
    }
}
