using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Explosive crystal")]
    [SerializeField] private bool CanExplode;
    
    [Header("Moving crystal")]
    [SerializeField] private bool CanMoveToEnemy;
    [SerializeField] private float MoveSpeed;
    public override void UseSkill()
    {
        base.UseSkill();

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            Crystal_Skill_Controller currentCrystalScrit = currentCrystal.GetComponent<Crystal_Skill_Controller>();
            
            currentCrystalScrit.SetupCrystal(crystalDuration,CanExplode,CanMoveToEnemy,MoveSpeed);           
        }
        else
        {
            Vector2 playerPos = player.transform.position;
            
            player.transform.position = currentCrystal.transform.position;
            
            currentCrystal.transform.position = playerPos;
            currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
        }
    }
}
