using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public Collider2D coll;

    private enum State { idle, running, jumping, falling, hurt, climbing}
    private State state = State.idle;

    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float jumpforce = 9f;
    [SerializeField] private float hurtForce = 3f;

    public TextMeshProUGUI cherryText;
    private int cherries = 0;

    //Health
    public TextMeshProUGUI healthAmount;
    private int health = 5;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        healthAmount.text = health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(state != State.hurt){
            if(Input.GetKey(KeyCode.A)){
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
            //move to left
            else if(Input.GetKey(KeyCode.D)){
                rb.velocity = new Vector2(speed, rb.velocity.y);
                transform.localScale = new Vector2(1, 1);
            }
            //attack
            if(Input.GetKey(KeyCode.W)){
                Attack();
            }
            //jump
            if(Input.GetKeyDown(KeyCode.Space) && coll.IsTouchingLayers(ground)){
                Jump();
            }
        }
        
        AnimationState();
        anim.SetInteger("state", (int)state); //set animation based on enum
    }

    private void Attack(){
        anim.SetTrigger("Attack");
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.jumping;
    }

    private void AnimationState()
    {
        if(state == State.jumping){
            if (rb.velocity.y < .1f)
            {
                state = State.falling; //if player is jumping then he is falling
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle; //if player touch the ground back to idle
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running; //moving
        }
        else
        {
            state = State.idle; //switch to idle
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            cherries += 1;
            cherryText.text = cherries.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if(other.gameObject.tag == "Enemy")
        {
            if (state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                HandleHealth();
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy is to my right therefore i should be damaged
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //enemy is to my left therefore i should be damaged
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    private void HandleHealth()
    {
        health -= 1;
        healthAmount.text = health.ToString();
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
