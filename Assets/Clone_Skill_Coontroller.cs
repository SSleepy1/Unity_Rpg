using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_Coontroller : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float cloneLosingSpeed;
    
    private float cloneTimer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1,1,1,sr.color.a - (Time.deltaTime * cloneLosingSpeed));   //颜色渐渐变透明
        }
    }

    public void SetupClone(Transform _newTransform,float _cloneDuration)
    {
        transform.position = _newTransform.position;
        cloneTimer = _cloneDuration;
    }
}
