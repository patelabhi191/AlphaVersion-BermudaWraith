﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

public class CharacterController_2D : MonoBehaviour {

    Rigidbody2D m_rigidbody;
    Animator m_Animator;
    Transform m_tran;

    private float h = 0;
    private float v = 0;

    public float MoveSpeed = 40;
    public float jumpPower = 300;
    private bool isJumping = false;

    public SpriteRenderer[] m_SpriteGroup;

    public bool Once_Attack = false;


    // Use this for initialization
    void Start () {
        m_rigidbody = this.GetComponent<Rigidbody2D>();
        m_Animator = this.transform.Find("BURLY-MAN_1_swordsman_model").GetComponent<Animator>();
        m_tran = this.transform;
        m_SpriteGroup = this.transform.Find("BURLY-MAN_1_swordsman_model").GetComponentsInChildren<SpriteRenderer>(true);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	// Update is called once per frame
	void Update () {

        //spriteOrder_Controller();

        if (Input.GetKeyDown(KeyCode.J))
        {
            Once_Attack = false;
            Debug.Log("Lclick");
            m_Animator.SetTrigger("Attack");
            m_rigidbody.velocity = new Vector3(0, 0, 0);
        }

        else if (Input.GetKeyDown(KeyCode.K))
        {
            Once_Attack = false;
            Debug.Log("Rclick");
            m_Animator.SetTrigger("Attack2");
            m_rigidbody.velocity = new Vector3(0, 0, 0);
        }

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Die")||
            m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")|| m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            return;

        Move_Fuc();
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        m_Animator.SetFloat("MoveSpeed", Mathf.Abs(h )+Mathf.Abs (v));
    }

    // character Move Function
    void Move_Fuc()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
          //  Debug.Log("Left");
            m_rigidbody.AddForce(Vector2.left * MoveSpeed);
            if (B_FacingRight)
                Filp();
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
          //  Debug.Log("Right");
            m_rigidbody.AddForce(Vector2.right * MoveSpeed);
            if (!B_FacingRight)
                Filp();
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && !isJumping)
        {
            m_rigidbody.AddForce(Vector2.up * jumpPower);
            isJumping = true;
        }
    }

    void Hurt()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");

        ScoreBoardController.instance.HealthDecrease();

        if (ScoreBoardController.health <= 0)
        {
            m_Animator.Play("Die");
            Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer);
            Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, false);
            StartCoroutine(DieSceneIn2Seconds());         
        }
        else
        {
            m_Animator.Play("Hit");
            TriggerHurt();
        }
    }

    IEnumerator DieSceneIn2Seconds()
    {
        yield return new WaitForSeconds(2);
        //ScoreBoardController.ResetStatics(typeof(ScoreBoardController));
        SceneManager.LoadScene(5);
    }

    public void TriggerHurt()
    {
        StartCoroutine(CannotBeHurtIn2Seconds());
    }

    IEnumerator CannotBeHurtIn2Seconds()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer);

        yield return new WaitForSeconds(2);

        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SteakMove steak = collision.collider.GetComponent<SteakMove>();
        EnemyFollowPlayer pumpkin = collision.gameObject.GetComponent<EnemyFollowPlayer>();
        MushroomBehavior mushroom = collision.gameObject.GetComponent<MushroomBehavior>();
        if (steak != null || pumpkin != null || mushroom != null)
        {            
                Hurt();           
        }

        if (collision.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }

    bool B_FacingRight = true;

    void Filp()
    {
        B_FacingRight = !B_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;

        m_tran.localScale = theScale;
    }
}
