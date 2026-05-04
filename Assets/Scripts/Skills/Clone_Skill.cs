using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefeb;    //创建一个克隆复制体
    [SerializeField] private float cloneDuration;
    [Space] 
    [SerializeField] private bool canAttack;

    public void CreateClone(Transform _clonePosition,Vector3 _offset)
    {
        GameObject newclone = Instantiate(clonePrefeb);
        
        newclone.GetComponent<Clone_Skill_Coontroller>().SetupClone(_clonePosition,cloneDuration,canAttack,_offset,FindClosetEnemy(newclone.transform));
    }
}
