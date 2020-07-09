using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    //protected virtual mean you can take this and make this your own
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void JumpedOn()
    {
        anim.SetTrigger("Death");
        rb.velocity = Vector2.zero; 
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }
}
