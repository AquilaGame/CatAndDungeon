using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimCtrl : MonoBehaviour
{
    Animator animator;
    CharacterScript cs;
    CharacterScript nowEnemy = null;
    int nowDamage = 0;
    [HideInInspector]public bool isPreView = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cs = GetComponent<CharacterScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPreView)
            return;
        if (cs.data.BaseData.HP > 0)
        {
            if (cs.NavAgent.velocity.magnitude > cs.NavAgent.speed * 0.1f)
                SetWalk(true);
            else
                SetWalk(false);
        }
    }

    public void SetWalk(bool bl)
    {
        animator.SetBool("Walk", bl);
    }

    public void SetInjured()
    {
        animator.SetBool("Injured", true);
    }

    public void GetHit()
    {
        animator.SetBool("Injured", false);
    }

    public void SetDeath(bool bl)
    {
        animator.SetBool("Death", bl);
    }

    public void Attack(CharacterScript enemy, int damage)
    {
        if (enemy != null)
        {
            transform.LookAt(new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z));
            nowEnemy = enemy;
        }
        nowDamage = damage;
        animator.SetBool("Attack", true);
    }

    public void Hit()
    {
        if (nowDamage > 0 && nowEnemy != null)
        {
            nowEnemy.HPGetDamage(nowDamage);
            cs.AfterCauseDamage(nowEnemy, nowDamage);
        }
        nowDamage = 0;
        nowEnemy = null;
        animator.SetBool("Attack", false);
    }




}
