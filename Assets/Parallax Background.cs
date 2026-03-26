using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    
    [SerializeField] private float parallaxEffect;
    
    private float xPosition;
    private float length;
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    // 使背景跟着角色进行移动，当超出边界时重置背景
    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1-parallaxEffect);
        float distanceMove = cam.transform.position.x * parallaxEffect;
        
        transform.position = new Vector3(xPosition + distanceMove, transform.position.y);

        if (distanceMoved > xPosition + length)
        {
            xPosition = xPosition + length;
        }
        else if (distanceMoved < xPosition - length)
        {
            xPosition = xPosition - length;
        }
    }
}
