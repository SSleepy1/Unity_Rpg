using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;
    
    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreatedHotKeys = true;
    private bool canAttackReleased;
    
    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> CreatedHotkeys = new List<GameObject>();

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed,int _amountOfAttacks,float _cloneAttackCooldown)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        
        CloneAttackLogic();
        
        if (canGrow && !canShrink)
        {
            Vector2 targetScale = new Vector2(maxSize, maxSize);
            //它表示从transform.localScale到Vector2(maxSize,maxSize)之间，按照Time.time * growSpeed这个百分比来取值
            transform.localScale = Vector2.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);
            if (Vector2.Distance(transform.localScale, targetScale) < 0.01f)
                transform.localScale = targetScale;
            //transform.localScale = Vector2.MoveTowards(transform.localScale,new Vector2(maxSize,maxSize),growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            Vector2 targetScale = new Vector2(0,0);
            transform.localScale = Vector2.Lerp(transform.localScale, targetScale, Time.deltaTime * shrinkSpeed);
            if (Vector2.Distance(transform.localScale, targetScale) < 0.01f)
            {
                transform.localScale = targetScale;
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count == 0)
        {
            FinishBlackHoleAbility();
            DestroyHotkeys();
            return;
        }
        DestroyHotkeys();
        canAttackReleased = true;
        canCreatedHotKeys = false;
        
        PlayerManager.instance.player.MakeTransparent(true);
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && canAttackReleased && targets.Count != 0)
        {
            cloneAttackTimer = cloneAttackCooldown;
            
            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;

            if (Random.Range(0, 2) == 1)
                xOffset = 2;
            else
                xOffset = -2;
            
            if(amountOfAttacks > 0)
                SkillManager.instance.clone.CreateClone(targets[randomIndex],new Vector3(xOffset,0));
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility",.1f);
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        PlayerManager.instance.player.ExitBlackHoleAbility();
        canShrink = true;
        canAttackReleased = false;
    }

    private void DestroyHotkeys()
    {
        if (CreatedHotkeys.Count <= 0)
        {
            return;
        }

        foreach (GameObject hotkey in CreatedHotkeys)
        {
            Destroy(hotkey);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.Log("11");
            return;
        }

        if (!canCreatedHotKeys)
            return;

        GameObject newHotkey = Instantiate(hotkeyPrefab, collision.transform.position + new Vector3(0, 2),
            Quaternion.identity);
        CreatedHotkeys.Add(newHotkey);
            
        //随机产生按键
        KeyCode chosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(chosenKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotkey.GetComponent<Blackhole_HotKey_Controller>();
            
        newHotKeyScript.SetupHotKey(chosenKey,collision.transform,this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
