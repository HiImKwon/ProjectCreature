using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yBotController : MonoBehaviour
{
    public Animator anim;
    public float speed = 100;
    [SerializeField]
    private Rigidbody rig;
    private CharacterController controller;
    private Vector3 inputVector;
    public float jumpForce = 100;
    public float gravityScale = 1;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            anim.SetBool("isWalking", true);
        }
        else if (Input.GetKey("s"))
        {
            anim.SetBool("isWalkBack", true);
        } else if (Input.GetKey("a"))
        {
            anim.SetBool("isLeftWalk", true);
        } else if (Input.GetKey("d"))
        {
            anim.SetBool("isRightWalk", true);
        }
        else
        {
            anim.SetBool("isLeftWalk", false);
            anim.SetBool("isRightWalk", false);
            anim.SetBool("isWalking", false);
            anim.SetBool("isWalkBack", false);

        }
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        // Let's assign both x and z
        Vector3 movement = new Vector3(moveHorizontal * speed, rig.velocity.y, moveVertical * speed);
        if (Input.GetButtonDown("Jump"))
        {
            movement.y = jumpForce;
        }
        movement.y = movement.y + (Physics.gravity.y * gravityScale);
        rig.velocity = movement;
    }
}
