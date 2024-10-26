using System;
using UnityEngine;
using System.Collections;
using TMPro;

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
    private float delay = 0.2f;
    private int shootingDamage;
    private int baseDamage = 4;
    public Canvas canvas;
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip shotFlot;
    [SerializeField] protected AudioClip shotMedium;
    [SerializeField] protected AudioClip shotHard;
    void Start()
    {
        shootingDamage = 2;
        deactivateCanvas();
    }

   void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        findTarget();

    }

    void Update()
    {
        //finds the target at shoots at it every few seconds
        
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
        target = null;
        //finds the enemies in the given range around the cannon
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, transform.position, 0f, enemyMask);
        
        if(hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log("Positioon: " +( hits[i].point.x));
                if (hits[i].point.x > 0.0f)
                {
                    target = hits[i].transform;
                    return;
                }
            }
            
        }
        
    }
    
    public int getBaseDamage()
    {
        return baseDamage;
    }
    public void setDamageAsBaseDamage()
    {
        shootingDamage = baseDamage;
        this.updateText();
    }
    private void Aim(/*Vector2 shootingDirection*/)
    {
        findTarget();
        if (target != null)
        {
            Vector2 shootingDirection = target.position;
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
        }
        //return shootingDirection;
    }
    private int getShootingDamage()
    {
        return shootingDamage;

    }
    
    public IEnumerator Shoot()
    {
        //shootingDirection = Aim();

        Aim();
        //shoots a ball towards the current target every two seconds
        //while (target != null)
        if (target != null)
        {
            //Aim(/*target.position*/);
            //wait for the animation to change
            yield return new WaitForSeconds(0.2f) ;
            if (shootingDamage != 0)
            {
                if (shootingDamage > 30)
                    audioSource.PlayOneShot(shotHard, audioSource.volume);
                else
                    audioSource.PlayOneShot(shotMedium, audioSource.volume);
                GameObject new_object = CannonBallPool.SharedInstance.GetPooledObject();
                if (new_object != null)
                {
                    new_object.transform.position = this.transform.position;
                    new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");

                    CannonBallController cb = new_object.GetComponent<CannonBallController>();
                    if (cb != null && target != null)
                    {
                        cb.setDirection(target.position);
                        cb.setDamage(getShootingDamage());
                        Debug.Log("Damage of shot " + getShootingDamage() + " " + this.GetHashCode());
                        Debug.Log("Position: " + target.position);
                    }


                    new_object.SetActive(true);


                }
            }
            else
            {
                audioSource.PlayOneShot(shotFlot, audioSource.volume);
            }
            //wait few seconds before the next shot
            yield return new WaitForSeconds(delay);
        }
        
    }
    /*

    public void Shoot()
    {
        //shootingDirection = Aim();


        //shoots a ball towards the current target every two seconds
        while (target != null)
        {
            Aim(target.position);
            //wait for the animation to change
            //yield return new WaitForSeconds(0.2f);
            GameObject new_object = CannonBallPool.SharedInstance.GetPooledObject();
            if (new_object != null)
            {
                new_object.transform.position = this.transform.position;
                new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");

                CannonBallController cb = new_object.GetComponent<CannonBallController>();
                if (cb != null && target != null)
                {
                    cb.setDirection(target.position);
                    cb.setDamage(getShootingDamage());
                    Debug.Log("Damage of shot " + getShootingDamage() + " " + this.GetHashCode());
                    Debug.Log("Position: " + target.position);
                }

                new_object.SetActive(true);


            }
            //wait few seconds before the next shot
            //yield return new WaitForSeconds(delay);
        }

    }*/

    public void setShootingDamage(int shootingDamage_)
    {
        this.shootingDamage = shootingDamage_;
        Debug.Log("Set damage to " + shootingDamage + " " + this.GetHashCode());
        this.updateText();

    }
    public void updateText()
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = shootingDamage.ToString();
    }
    public void activateCanvas()
    {
        canvas.gameObject.SetActive(true);
    }
    public void deactivateCanvas()
    {
        canvas.gameObject.SetActive(false);
    }

    //.
}

