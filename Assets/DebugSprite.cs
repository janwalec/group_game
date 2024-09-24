using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSprite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log($"sprite size # {spriteRenderer.sprite.rect.width}, {spriteRenderer.sprite.rect.height}");
      
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        Debug.Log($"Camera # {width}, {height}");


        Debug.Log("Screen Size : " + Screen.width + "," + Screen.height);
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
