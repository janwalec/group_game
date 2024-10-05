using System;
using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour
{

    public enum STATE {RIGHT = 1, UP=2, DOWN=3};
    private STATE myState = STATE.RIGHT;
    private Animator animator;
    private Vector3 shootingDirection = new Vector3 (0f, 0f, 0f);
    //private ArrayList enemies = new ArrayList();
    private Transform target;
    private float range = 20f;
    [SerializeField] private LayerMask enemyMask;
    private float delay = 2f;
    void Start()
    {
     
    }

   void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //finds the target at shoots at it every few seconds
        if(target == null)
        {
            findTarget();
            if (target != null)
            {
                StartCoroutine(Shoot());
            }
            return;
        }
        //Aim();
        //if(Input.GetMouseButtonDown(0)){
        //    Shoot();
        //}
    }

    private void OnMouseDown()
    {
        //Debug.Log("Mouse down on cannon");
        //Shoot();
    }

    private void findTarget()
    {
        //finds the enemies in the given range around the cannon
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, transform.position, 0f, enemyMask);
        
        if(hits.Length > 0)
        {
            target = hits[0].transform;
        }
        
    }
    
    private void Aim(Vector2 shootingDirection)
    {
       // Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       // shootingDirection = this.transform.position - mouseWorldPos;

        //sets the animator so that the image matches shooting direction
        if (this.transform.position.y - shootingDirection.y > 0.5)
        {
            myState = STATE.DOWN;
        }
        else if (this.transform.position.y - shootingDirection.y < -0.5)
        {
            myState = STATE.UP;
        }
        else
        {
            myState = STATE.RIGHT;
        }
        animator.SetInteger("state", (int)myState);
        
        //return shootingDirection;
    }
    private IEnumerator Shoot()
    {
        //shootingDirection = Aim();
        

        //shoots a ball towards the current target every two seconds
        while (target != null)
        {
            Aim(target.position);
            //wait for the animation to change
            yield return new WaitForSeconds(0.2f) ;
            GameObject new_object = CannonBallPool.SharedInstance.GetPooledObject();
            if (new_object != null)
            {
                new_object.transform.position = this.transform.position;
                new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");

                CannonBallController cb = new_object.GetComponent<CannonBallController>();
                if (cb != null && target != null)
                {
                    cb.setDirection(target.position);
                    Debug.Log("Position: " + target.position);
                }

                new_object.SetActive(true);


            }
            //wait few seconds before the next shot
            yield return new WaitForSeconds(delay);
        }
        
    }

}

