using UnityEngine;
using UnityEngine.UI;

public abstract class MovableItem : MonoBehaviour
{
    //private Canvas rearrangeCanvas;
    bool moving = false;
    [SerializeField] public MarketItemController selector;
    Vector3 prevPosition = Vector3.zero;
    public void Start()
    {
        
       
    }

    public abstract void ActivateMiniVersion();
    public abstract void DeactivateMiniVersion();
    public void OnMouseDown()
    {
        Debug.Log("Modifier clicked!.!");
        if (GameManager.instance.currentGameState != GameState.GS_PREPARE && GameManager.instance.currentGameState != GameState.GS_WAIT)
        {
            return;
        }

        if (!moving)
        {
            // Remove the chain immediately when picking up the item
            GameManager.instance.chainControler.deleteChainByElement(this);

            prevPosition = transform.position;
            ActivateMiniVersion();
            // gameObject.SetActive(false);  // If you want to hide the original item, uncomment this
        }

        if (moving)
        {
            //gameObject.SetActive(true); // If you hide the original, you can make it visible again here
            PutDown();
            DeactivateMiniVersion();
        }

        moving = !moving;
    }

    public void Update()
    {
        if (moving)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z += Camera.main.nearClipPlane;
            transform.position = mouseWorldPos;
        }
    }

    private void PutDown()
    {
        TIleMapGenerator tm = GameManager.instance.getTilemap();
        tm.selectObject(selector);

        if (!tm.PlaceAnItem(this.gameObject))
        {
            transform.position = prevPosition;
        }
        else
        {
            // When placing the item, no need to delete the chain again since it's already deleted on pick up
            tm.releaseTile(tm.getTileFromMousePosition(prevPosition));
        }
    }


}