using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    //vector2 has two flow variables stored in it: 1 for x [left and right] 1 for y [up and down]
    Vector2 movementInput;

    SpriteRenderer spriteRenderer;

    Rigidbody2D rb;

    Animator animator;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    bool canMove = true;

    // Start is called before the first frame update
    // get component is going to be looking for a type, once it has that type it will store it into rigidbody
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // when we have the movement input, we want to miltiply it by  a speed and time of sorts in order to figure out how fast our character is moving and in what direction
    // no input:idle
    private void FixedUpdate() {
        if(canMove){
            // If movement input is not 0, try to move
            if(movementInput != Vector2.zero){
                bool success = TryMove(movementInput);

                if(!success) {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }
                if(!success) {
                        success = TryMove(new Vector2(0, movementInput.y));
                }

                animator.SetBool("isMoving", success);
            }
            else{
                animator.SetBool("isMoving", false);
            }

            //set direction of sprite to movement direction
            //facing left
            if(movementInput.x < 0){
                spriteRenderer.flipX = true;
            }
            //facing right
            else if(movementInput.x > 0){
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction){
        if(direction != Vector2.zero){
        //checks for potential collisions
        int count = rb.Cast(
                direction,      //x nad y values between -1 and 1 that reperesent the direction from th by to look for collisions
                movementFilter,     //the settings that determine where a collisin can occur on such as layes to collide with
                castCollisions,     //list of collisions to store the found collisiosn into after the cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset   //the amount to cast equal to the movement plus an offset
            );
            if(count == 0){
                rb.MovePosition(rb.position + direction * Time.fixedDeltaTime);
                return true;
            }
            else{
                return false;
            }
        }
        else{
            //cant move if there's no diretion to move in 
            return false;
        }
    }

    //this is going to receive a argument from the message(the xy direction when player presses key), receive it as an Inputvalue
    //take inputvalue and store it into a variable
    //take the movement input and set it to the value that we are going to get back from the input value
    void OnMove(InputValue movementValue){
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        LockMovement();

        if(spriteRenderer.flipX == true){
            swordAttack.AttackLeft();
        } else {
            swordAttack.AttackRight();
        }
    }

    public void EndSwordAttack() {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
}

