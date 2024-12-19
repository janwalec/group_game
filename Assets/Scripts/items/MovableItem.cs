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
        if (GameManager.instance.currentGameState != GameState.GS_PREPARE) return;
        if (!moving)
        {
            prevPosition = transform.position;
            ActivateMiniVersion ();
            //gameObject.SetActive(false);
        }
        if(moving)
        {
            //gameObject.SetActive(true);
            PutDown();
            DeactivateMiniVersion ();
        }
        moving = !moving;
    }

    public void Update()
    {
        if(moving)
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
            GameManager.instance.chainControler.deleteChainByElement(this);
            tm.releaseTile(tm.getTileFromMousePosition(prevPosition));
        }
        //Vector3Int gridPosition = tm.WorldToCell(mouseWorldPos);
    }

}