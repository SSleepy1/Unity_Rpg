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

    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool canCreateCloneOnCounterAttack;
    [Header("Chance can duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of clone")]
    public bool crystalInsteadOfClone;
    public void CreateClone(Transform _clonePosition,Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newclone = Instantiate(clonePrefeb);
        newclone.GetComponent<Clone_Skill_Coontroller>().SetupClone(_clonePosition,
            cloneDuration,canAttack,_offset,FindClosetEnemy(newclone.transform),canDuplicateClone,chanceToDuplicate,player);
    }

    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
        {
            CreateClone(player.transform,Vector3.zero);
        }
    }

    public void CreateClongOnDashOver()
    {
        if (createCloneOnDashOver)
        {
            CreateClone(player.transform,Vector3.zero);
        }
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)
        {
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
        }
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform,Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            CreateClone(_transform,_offset);
    }
}
