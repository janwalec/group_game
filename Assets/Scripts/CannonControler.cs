using UnityEngine;

public class CannonController : MonoBehaviour
{

    public enum STATE {RIGHT = 1, UP=2, DOWN=3};
    private STATE myState = STATE.RIGHT;
    private Animator animator;
    

    void Start()
    {

    }

   void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    private void OnMouseDown()
    {
        Debug.Log("Mouse down on cannon");
        Shoot();
    }
    private void Shoot()
    {
        GameObject new_object = CannonBallPool.SharedInstance.GetPooledObject();
        if (new_object != null)
        {
            new_object.transform.position = this.transform.position;
            new_object.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("OnLand");
            new_object.SetActive(true);

            //To be deleted later
            myState = (STATE)((int)(myState + 1) % 3 + 1);
            animator.SetInteger("state", (int)myState);
            Debug.Log(myState);
        }
    }
}

