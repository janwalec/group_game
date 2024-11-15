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
    public void OnMouseDown()
    {
        Debug.Log("Modifier clicked!.!");
        if (!moving)
        {
            prevPosition = transform.position;
        }
        if(moving)
        {
            PutDown();
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