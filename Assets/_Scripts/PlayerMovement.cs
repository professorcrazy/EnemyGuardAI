using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody rb;
    Vector3 moveDir;
    KeyCode runKay = KeyCode.LeftShift;
    public float walkSpeed, runSpeed;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();

        if (Input.GetKeyDown(runKay))
        {
            speed = runSpeed;
        }
        if (Input.GetKeyUp(runKay))
        {
            speed = walkSpeed;
        }
        if (moveDir != Vector3.zero)
        {
            transform.forward = moveDir;
            //alternative method
            Quaternion toRotate = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, 3*Time.deltaTime);
        }
        rb.velocity = new Vector3(moveDir.x * speed, rb.velocity.y, moveDir.z * speed);
    }
}
