using System;
using UnityEngine;
using System.Collections;
using TMPro;

public class CannonController : MovableItem
{

    public enum STATE {RIGHT = 1, UP=2, DOWN=3};
    private STATE myState = STATE.RIGHT;
    private Animator animator;
    [SerializeField] private GameObject animatedObject;
    private Vector3 shootingDirection = new Vector3 (0f, 0f, 0f);
    //private ArrayList enemies = new ArrayList();
    private Transform target;
    private GameObject enemyTargeted;
    private float wideRange = 20f;
    private float narrowRange = 7f;
    private float mediumRange = 13f;
    [SerializeField] private LayerMask enemyMask;
    private float delay = 0.2f;
    private int shootingDamage;
    private int baseDamage = 1;
    private float slowedSpeed;
    private bool isBouncy;
    private bool hasGoldBonus;
    private bool hasSlowing;
    public Canvas canvas;
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip shotFlot;
    [SerializeField] protected AudioClip shotMedium;
    [SerializeField] protected AudioClip shotHard;
    public GameObject bonus;
    public ParticleSystem shootingParticles;
    protected System.Random rand = new System.Random();


    void Start()
    {
        base.Start();
        shootingDamage = 2;
        deactivateCanvas();
        EraseBonusIcon();
    }

   void Awake()
    {
        animator = animatedObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        findTarget();

    }






    private void findTarget()
    {
        target = null;
        //finds the enemies in the given range around the cannon
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, narrowRange, transform.position, 0f, enemyMask);
        if(hits.Length == 0)
            hits = Physics2D.CircleCastAll(transform.position, mediumRange, transform.position, 0f, enemyMask);
        if(hits.Length == 0)
            hits = Physics2D.CircleCastAll(transform.position, wideRange, transform.position, 0f, enemyMask);


        if (hits.Length > 0)
        {
            // Shuffle the hits array
            hits = ShuffleArray(hits);

            for (int i = 0; i < hits.Length; i++)
            {
                // Get the EnemyController component
                EnemyController enemyController = hits[i].transform.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isEnemyDying())
                {
                    target = hits[i].transform;
                    break; // Found a valid target, break out of the loop
                }
            }
        }
    }
    
    private T[] ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }

    public void resetEffects()
    {
        setHasSlowingEffect(false);
        setHasBonus(false);
        setIsBouncy(false);
    }

    public void setSlowingEffect(float newSpeed)
    {

        this.slowedSpeed = newSpeed;
        //Debug.Log("Slowing enemy for:" + newSpeed);
    }

    public void setHasSlowingEffect(bool hasSlowing)
    {
        this.hasSlowing = hasSlowing;
    }

    public void setIsBouncy(bool isBouncy)
    {
        this.isBouncy = isBouncy;
    }
    
    public void setHasBonus(bool hasBonus)
    {
        this.hasGoldBonus = hasBonus;
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

    private Vector3 calculateParticlePosition(Vector3 cannonPosition) {
        Vector3 particlePosition = cannonPosition;
        if (this.myState == STATE.UP) {
            particlePosition.y = cannonPosition.y + 0.2f;
        } 
        else if (this.myState == STATE.DOWN) {
            particlePosition.y = cannonPosition.y - 0.2f;
        }

        particlePosition.x = cannonPosition.x + 0.4f;
        return particlePosition;

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
            do
            {
                yield return new WaitForSeconds(0.2f);
            }while(GameManager.instance.currentGameState != GameState.GS_BATTLE);
            if (shootingDamage != 0)
            {   
                
                if (shootingDamage > 30)
                    audioSource.PlayOneShot(shotHard, audioSource.volume);
                else if (shootingDamage == 1)
                    audioSource.PlayOneShot(shotFlot, audioSource.volume);
                else audioSource.PlayOneShot(shotMedium, audioSource.volume);
                GameObject new_object = ItemPool.SharedInstance.GetPooledCannonBall();
                if (new_object != null)
                {   
                    Vector3 particlePosition = calculateParticlePosition(this.transform.position);

                    Instantiate(shootingParticles, particlePosition, Quaternion.identity);
                    new_object.transform.position = this.transform.position;
                    new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");

                    CannonBallController cb = new_object.GetComponent<CannonBallController>();
                    if (cb != null && target != null)
                    {
                        cb.setDirection(target);
                        cb.setDamage(getShootingDamage());
                        if(isBouncy) cb.setBouncy(true); //SET BOUNCY EFFECT.
                        if(hasSlowing) cb.setSlowingEffect(calculateSlowingEffect(getShootingDamage())); //Set slowing effect
                        if(hasGoldBonus)cb.SetHasGoldMultiplier(true); //Set gold multiplier.
                        
                        //Debug.Log("Damage of shot " + getShootingDamage() + " " + this.GetHashCode());
                        //Debug.Log("Position: " + target.position);
                    }
                    

                    new_object.SetActive(true);


                }
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

    public void setExtraShootingDamage(int shootingDamage_)
    {
        this.shootingDamage = shootingDamage_ + baseDamage;
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

    private float calculateSlowingEffect(int damage)
    {
        // Define the minimum and maximum slowing factors
        float minSlowingFactor = 0.2f;
        float maxSlowingFactor = 0.8f;

        // Ensure the damage is at least 1 to avoid division by zero or negative values
        damage = Mathf.Max(damage, 1);

        // Calculate the interpolation factor (normalized value between 0 and 1)
        float t = (damage - 1) / 49.0f; // 49 because damage ranges from 1 to 50 (inclusive)

        // Calculate the slowing effect using linear interpolation
        float slowingEffect = Mathf.Lerp(maxSlowingFactor, minSlowingFactor, t);

        return slowingEffect;
    }

    //.
    
    public void SetBonusIcon(ChainGenerator.Direction direction)
    {
        Debug.Log("Setting bonus icon");
        String position = "";
        switch (direction)
        {
            case ChainGenerator.Direction.MIDDLE_LEFT:
                position = "MiddleLeft";
                break;
            case ChainGenerator.Direction.MIDDLE_RIGHT:
                position = "MiddleRight";
                break;
            case ChainGenerator.Direction.BOTTOM_LEFT:
                position = "BottomLeft";
                break;
            case ChainGenerator.Direction.BOTTOM_RIGHT:
                position = "BottomRight";
                break;
            case ChainGenerator.Direction.TOP_LEFT:
                position = "TopLeft";
                break;
            case ChainGenerator.Direction.TOP_RIGHT:
                position = "TopRight";
                break;
        }
        bonus.transform.Find(position).gameObject.SetActive(true);
        Debug.Log("Found bonus icon: " + bonus.transform.Find(position).gameObject.name);
    }

    public void EraseBonusIcon()
    {
        int childrenNum = bonus.transform.childCount;
        for (int i = 0; i < childrenNum; i++)
        {
            bonus.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}

