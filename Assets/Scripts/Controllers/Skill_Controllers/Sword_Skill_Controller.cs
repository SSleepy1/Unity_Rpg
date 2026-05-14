using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
   private Animator anim;
   private Rigidbody2D rb;
   private CircleCollider2D cd;
   private Player player;

   private bool canRotate = true;
   private bool isReturning;


   private float freezeTimeDuration;
   private float returnSpeed = 12;
   
   [Header("Pierce info")] 
   private float pierceAmount;
   
   [Header("Bounce info")]
   private float bounceSpeed;
   private bool isBouncing;
   private int amountOfBounce;
   private List<Transform> enemyTarget;
   private int targetIndex;

   [Header("Spin info")] 
   private float maxTravelDistance;
   private float spinDuration;
   private float spinTimer;
   private bool wasStopped;
   private bool isSpinning;

   private float hitTimer;
   private float hitCooldown;

   private float spinDirection;
   
   private void Awake()
   {
      anim = GetComponentInChildren<Animator>();
      rb = GetComponent<Rigidbody2D>();
      cd = GetComponent<CircleCollider2D>();
   }

   private void DestroyMe()
   {
      Destroy(gameObject);
   }
   

   public void SetupSword(Vector2 _dir, float _gravityScale, Player _player,float _freezeTimeDuration,float _returnSpeed)
   {
      player = _player;
      freezeTimeDuration = _freezeTimeDuration;
      returnSpeed = _returnSpeed;

      rb.velocity = _dir;
      rb.gravityScale = _gravityScale;

      if(pierceAmount <= 0)
         anim.SetBool("Rotate", true);
      
      spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);    //限定方向值为-1~1之间
      
      Invoke("DestroyMe",5);
   }

   public void SetupBounce(bool _isBouncing, int _amountOfBounce,float _bounceSpeed)
   {
      isBouncing = _isBouncing;
      amountOfBounce = _amountOfBounce;
      bounceSpeed = _bounceSpeed;

      enemyTarget = new List<Transform>();
   }

   public void SetupPierce(int _pierceAmount)
   {
      pierceAmount = _pierceAmount;
   }

   public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration,float _hitCooldown)
   {
      isSpinning = _isSpinning;
      maxTravelDistance = _maxTravelDistance;
      spinDuration = _spinDuration;
      hitCooldown = _hitCooldown;
   }

   public void ReturnSword()
   {
      rb.constraints = RigidbodyConstraints2D.FreezeAll;
      //rb.isKinematic = false;
      transform.parent = null;
      isReturning = true;
   }

   private void Update()
   {
      if (canRotate)
      {
         transform.right = rb.velocity;
      }

      if (isReturning)
      {
         transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

         if (Vector2.Distance(transform.position, player.transform.position) < 1)
         {
            player.CatchTheSword();
         }
      }

      BounceLogic();

      SpinLogic();
   }

   private void SpinLogic()
   {
      if (isSpinning)
      {
         if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
         {
            StopWhenSpinning();
         }

         if (wasStopped)
         {
            spinTimer -= Time.deltaTime;

            //使剑往发射的方向缓慢向前移动
            transform.position = Vector2.MoveTowards(transform.position,
               new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
            if (spinTimer < 0)
            {
               isReturning = true;
               isSpinning = false;
               //wasStopped = false;   收回后对象销毁，不需要再设置状态
            }
            
            hitTimer -= Time.deltaTime;
            if (hitTimer < 0)
            {
               hitTimer = hitCooldown;
               
               Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,1);    //剑旋转碰撞体积

               foreach (var hit in colliders)
               {
                  if(hit.GetComponent<Enemy>() != null)
                     SwordSkillDamage(hit.GetComponent<Enemy>());
               }
            }
         }
      }
   }

   private void StopWhenSpinning()
   {
      wasStopped = true;
      rb.constraints = RigidbodyConstraints2D.FreezePosition;
      spinTimer = spinDuration;
   }

   private void BounceLogic()
   {
      if (isBouncing && enemyTarget.Count > 0)
      {
         transform.position = Vector2.MoveTowards(transform.position,enemyTarget[targetIndex].position,bounceSpeed * Time.deltaTime);

         if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
         {
            SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
            
            targetIndex++;
            amountOfBounce--;

            if (amountOfBounce <= 0)
            {
               isBouncing = false;
               isReturning = true;
            }

            if (targetIndex >= enemyTarget.Count)
            {
               targetIndex = 0;
            }
         }
      }
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (isReturning)
      {
         return;
      }
      if (collision.GetComponent<Enemy>() != null)
      {
         Enemy enemy = collision.GetComponent<Enemy>();

         SwordSkillDamage(enemy);
      }
      
      SetupTargetForBounce(collision);
      
      StuckInto(collision);
   }

   private void SwordSkillDamage(Enemy enemy)
   {
      player.stats.DoDamage(enemy.GetComponent<CharacterStats>());
      enemy.StartCoroutine("FreezeTimeFor",freezeTimeDuration);
   }

   private void SetupTargetForBounce(Collider2D collision)
   {
      if (collision.GetComponent<Enemy>() != null)
      {
         if (isBouncing && enemyTarget.Count <= 0)
         {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,10);
            
            foreach(var hit in colliders)
            {
               if (hit.GetComponent<Enemy>() != null)
               {
                  enemyTarget.Add(hit.transform);
               }
            }
         }
      }
   }

   private void StuckInto(Collider2D collision)
   {
      if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
      {
         pierceAmount--;
         return;
      }

      if (isSpinning)
      {
         StopWhenSpinning();
         return;
      }

      canRotate = false;
      cd.enabled = false;  //关闭碰撞体

      rb.isKinematic = true;  //设置rigidbody为Kinematic
      rb.constraints = RigidbodyConstraints2D.FreezeAll;    //冻结XYZ方向

      if (isBouncing && enemyTarget.Count > 0)
         return;
      
      anim.SetBool("Rotate",false);
      transform.parent = collision.transform;   //设置为被碰撞物体的子类
   }
}
