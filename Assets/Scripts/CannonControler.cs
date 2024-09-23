using UnityEngine;

public class CannonController : MonoBehaviour
{

    public enum STATE {RIGHT = 1, UP=2, DOWN=3};
    private STATE myState = STATE.RIGHT;
    private Animator animator;
    private Vector3 shootingDirection = new Vector3 (0f, 0f, 0f);

    void Start()
    {
        Aim();
    }

   void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Aim();
        if(Input.GetMouseButtonDown(0)){
            Shoot();
        }
    }

    private void OnMouseDown()
    {
        //Debug.Log("Mouse down on cannon");
        //Shoot();
    }

    private Vector3 Aim()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        shootingDirection = this.transform.position - mouseWorldPos;

        if (shootingDirection.y > 1.0)
        {
            myState = STATE.DOWN;
        }
        else if (shootingDirection.y < -1.0)
        {
            myState = STATE.UP;
        }
        else
        {
            myState = STATE.RIGHT;
        }
        animator.SetInteger("state", (int)myState);
        Debug.Log(myState);

        
        return shootingDirection;
    }
    private void Shoot()
    {
        //shootingDirection = Aim();


        GameObject new_object = CannonBallPool.SharedInstance.GetPooledObject();
        if (new_object != null)
        {
            new_object.transform.position = this.transform.position;
            new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");

            CannonBallController cb = new_object.GetComponent<CannonBallController>();
            if (cb != null)
            {
                cb.setDirection(shootingDirection);
            }

            new_object.SetActive(true);
          
        }
    }
}

