using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;
    
    public float maxSize;
    public float growSpeed;
    public bool canGrow;

    private List<Transform> targets = new List<Transform>();
    private void Update()
    {
        if (canGrow)
        {
            Vector2 targetScale = new Vector2(maxSize, maxSize);
            //它表示从transform.localScale到Vector2(maxSize,maxSize)之间，按照Time.time * growSpeed这个百分比来取值
            transform.localScale = Vector2.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);
            if (Vector2.Distance(transform.localScale, targetScale) < 0.01f)
                transform.localScale = targetScale;
            //transform.localScale = Vector2.MoveTowards(transform.localScale,new Vector2(maxSize,maxSize),growSpeed * Time.deltaTime);
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

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.Log("11");
            return;
        }

        GameObject newHotkey = Instantiate(hotkeyPrefab, collision.transform.position + new Vector3(0, 2),
            Quaternion.identity);
            
        //随机产生按键
        KeyCode chosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(chosenKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotkey.GetComponent<Blackhole_HotKey_Controller>();
            
        newHotKeyScript.SetupHotKey(chosenKey,collision.transform,this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
