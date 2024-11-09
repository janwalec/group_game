using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using System.Linq;

public class Chain
{
    public int chainID;
    public LinkedList<MyTile> tileChain;
    public List<PooledChain> pooledObjectChain;
    public int chainSum;
    public LineRendererController lineRenderer;



    public Chain(LinkedList<MyTile> _tileChain, List<PooledChain> _pooledObjectChain, int ID, int chainSum_ = 0)
    {
        tileChain = _tileChain;
        pooledObjectChain = _pooledObjectChain;
        chainID = ID;
        chainSum = chainSum_;
        if (_tileChain != null)
        {
            lineRenderer = ItemPool.SharedInstance.GetPooledLineRenderer().GetComponent<LineRendererController>();
            lineRenderer.gameObject.SetActive(true);
            Debug.Log("Line rendered pooling");
            RenderLine();
        }
    }

    public void RenderLine()
    {
        if (lineRenderer != null)
        {
            Vector3[] points = new Vector3[tileChain.Count];
            for (int i = 0; i < tileChain.Count; i++)
            {
                points[i] = GameManager.instance.getTilemap().getWorldPosition(tileChain.ElementAt(i).tilePosition);
            }
            lineRenderer.SetUpLine(points);
        }
    }

    public Chain changeSum(int newSum)
    {
        return new Chain(this.tileChain, this.pooledObjectChain, this.chainID, newSum);
    }

    /*
    public Chain setSum(int newSum)
    {
        chainSum = newSum;
        Debug.Log("Chain sum " + chainSum);
        return this;
    }
    */
    //modifiers
}

public class ChainControler : MonoBehaviour
{
    private ChainPool chainPool;
    private ChainGenerator chainGenerator;

    private List<Chain> myChains = new List<Chain>();

    private int currID = 0;

    public delegate void KeyPressAction();

    private float rollingDelay = 1f;

    private int maxChainLeght = 0;

    private float timer = 0f;

    void Start()
    {
        chainPool = ChainPool.SharedInstance;
        chainGenerator = FindObjectOfType<ChainGenerator>();
        chainGenerator.OnChainComplete += UpdateObjectsOnChainComplete;
    }

    //be notified if Chain is added
    private void UpdateObjectsOnChainComplete(LinkedList<MyTile> receivedChain)
    {
        //Debug.Log("Chain completed with " + receivedChain.Count + " tiles.");

        processAndSaveGivenChain(receivedChain);
        AddOperationSigns(receivedChain);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (myChains.Count > 0)
            {
                Chain tmp = myChains[myChains.Count - 1];
                deleteChain(tmp.chainID);
            }
        }

        if (timer >= (maxChainLeght + 1) * (rollingDelay))
        {

            findMaxLength();
            rollAllModifiers();
            timer = 0.0f;
        }
        /*
        else if (Input.GetKeyDown(KeyCode.C))
        {
            findMaxLength();
            rollAllModifiers();
            
        }
        */
    }

    private LinkedList<MyTile> searchForInteresctionsAndChangeChain(LinkedList<MyTile> receivedChain)
    {
        bool removing = false;
        MyTile intersection = new MyTile(0, 0, null, MyTile.TileType.WATER);

        LinkedList<MyTile> outputChain = new LinkedList<MyTile>();

        foreach (MyTile t in receivedChain)
        {
            outputChain.AddLast(t);
            if (t.usedForChain)
            {
                intersection = t;
                removing = true;
                Debug.Log("found intersection");
                break;
            }

        }

        Chain chainThatContainsIntersection = new Chain(null, null, 0); //here

        if (removing)
        {
            foreach (Chain ch in myChains)
            {
                if (ch.tileChain.Contains(intersection))
                {
                    chainThatContainsIntersection = ch;
                    break;
                }
            }

            bool foundIntersection = false;

            foreach (MyTile t in chainThatContainsIntersection.tileChain)
            {
                if (t == intersection)
                {
                    foundIntersection = true;
                    continue;
                }
                if (foundIntersection)
                    outputChain.AddLast(t);
            }
        }

        return outputChain;
    }

    private void processAndSaveGivenChain(LinkedList<MyTile> _receivedChain)
    { // get completed Chain with its tiles
        int i = 0;
        LinkedList<MyTile> receivedChain = searchForInteresctionsAndChangeChain(_receivedChain);

        int amount = receivedChain.Count;

        // get (transparent) object to indicate where Chain is
        List<PooledChain> pooledObjects = chainPool.GetObjects(amount);

        // init random color for a Chain
        System.Random rnd = new System.Random();
        int r = rnd.Next(256), g = rnd.Next(256), b = rnd.Next(256);
        Color color = new Color(r / 255f, g / 255f, b / 255f, 1f);

        foreach (MyTile t in receivedChain)
        {
            MyTile currTile = t;

            // set random color
            SpriteRenderer spriteRenderer = pooledObjects[i].gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;

            // transform position of game object and its canvas (text on object)
            Vector3 position = chainGenerator.getGenPosition(currTile);
            pooledObjects[i].gameObject.transform.position = position;
            pooledObjects[i].canvas.transform.position = position;

            i += 1;

        }

        chainGenerator.markUsedForChainTiles(receivedChain, true);
        // create new Chain, set the text to it and save
        Chain newChain = new Chain(receivedChain, pooledObjects, currID++);
        setTextForChain(newChain);
        myChains.Add(newChain);
        if (newChain.tileChain.First.Value.tileType != MyTile.TileType.CANNON)
        {
            deleteChain(newChain.chainID);
        }
        if (newChain.tileChain.Count > maxChainLeght)
        {
            maxChainLeght = newChain.tileChain.Count;
        }

    }

    private void findMaxLength()
    {
        maxChainLeght = 0;
        foreach (Chain ch in myChains)
        {
            if (ch.tileChain.Count > maxChainLeght)
            {
                maxChainLeght = ch.tileChain.Count;
            }
        }
    }
    private void setTextForChain(Chain Chain)
    {
        int i = 0;
        foreach (PooledChain chainPart in Chain.pooledObjectChain)
        {
            if (i == 0)
            {
                chainPart.changeText("h" + myChains.Count);
            }
            else if (i == Chain.pooledObjectChain.Count - 1)
            {
                chainPart.changeText("t" + myChains.Count);
            }
            else
            {
                chainPart.changeText("-" + myChains.Count);
            }
            i++;
        }
    }

    public void updateNotDeletedChains()
    {
        foreach (Chain ch in myChains)
        {
            chainGenerator.markUsedForChainTiles(ch.tileChain, true);
        }
    }

    private void deleteChain(int ID)
    {
        Chain toDelete = new Chain(null, null, -1);

        // search for a chain with given ID
        foreach (Chain ch in myChains)
        {
            if (ch.chainID == ID)
            {
                toDelete = ch;
                break;
            }
        }


        // no chain with this ID found
        if (toDelete.chainID == -1)
            return;

        toDelete.lineRenderer.gameObject.SetActive(false);
        RemoveOperationSigns(toDelete);

        int length = toDelete.pooledObjectChain.Count;
        int deletedCount = chainPool.SetChainInactive(toDelete.pooledObjectChain);

        //this generates problems
        //updateNotDeletedChains checks chains that are still on the board and makes sure that they are active
        chainGenerator.markUsedForChainTiles(toDelete.tileChain, false);

        myChains.Remove(toDelete);
        updateNotDeletedChains();

    }

    public void rollAllModifiers()
    {

        foreach (Chain ch in myChains)
        {
            StartCoroutine(rolling(ch));
        }

    }

    //for now: cannons base damage works if it is only the cannon in the chain
    //otherwise damage is computed ignoring cannon base damage

    private IEnumerator rolling(Chain currChain)
    {

        if (GameManager.instance.currentGameState == GameState.GS_BATTLE)
        {

            float slowing_effect;
            LinkedListNode<MyTile> curr = currChain.tileChain.Last;
            LinkedListNode<MyTile> temp = currChain.tileChain.Last;
            float minSlowingFactor = 0.2f;
            float maxSlowingFactor = 0.8f;
            float newSpeed = 1.5f;
            bool isSlowed = false;

            while (curr != null)
            {

                if (curr.Value.tileType == MyTile.TileType.COIN || curr.Value.tileType == MyTile.TileType.DICE)
                {

                    curr.Value.modifier.Roll();


                    if (curr != null && curr.Next != null)
                    {
                        if (curr.Value.tileType == MyTile.TileType.DICE && curr.Next.Value.tileType == MyTile.TileType.DICE)
                        {
                            isSlowed = true;
                            Debug.Log("Found 2 dice");

                            slowing_effect = (float)currChain.chainSum;

                            float slowingFactor = Mathf.Lerp(minSlowingFactor, maxSlowingFactor, (slowing_effect - 2) / 10.0f);
                            if (slowing_effect == 0)
                            {
                                newSpeed = 1.5f;
                            }
                            else
                            {
                                newSpeed = 1.5f * (1.0f - slowingFactor);
                            }
                            // this shouldn't be hardcoded
                            Debug.Log($"Dice sum:{slowing_effect}, new speed:{newSpeed}");
                        }
                    }
                    curr.Value.modifier.calculateCurrentTotal(curr.Next == null ? null : curr.Next.Value.modifier);
                    curr.Value.modifier.ChangeAnimation();
                    curr.Value.modifier.activateCanvas();


                    do
                    {
                        yield return new WaitForSeconds(rollingDelay);
                    } while (GameManager.instance.currentGameState != GameState.GS_BATTLE);

                    curr.Value.modifier.ChangeAnimation();
                    curr.Value.modifier.deactivateCanvas();

                }

                if (curr.Previous == null)
                {
                    if (curr.Value.tileType == MyTile.TileType.CANNON)
                    {
                        do
                        {
                            yield return new WaitForSeconds(rollingDelay * (maxChainLeght + 1 - currChain.tileChain.Count));
                        } while (GameManager.instance.currentGameState != GameState.GS_BATTLE);
                        //curr.Value.cannon.setShootingDamage(myChains[myChains.Count - 1].chainSum);

                        if (currChain.tileChain.Count == 1)
                        {
                            curr.Value.cannon.setDamageAsBaseDamage();
                            curr.Value.cannon.setSlowingEffect(newSpeed);
                        }
                        else
                        {
                            curr.Value.cannon.setShootingDamage(currChain.chainSum);
                            curr.Value.cannon.setSlowingEffect(newSpeed);
                        }
                        //StartCoroutine(curr.Value.cannon.Shoot());
                        //curr.Value.cannon.Shoot();
                        curr.Value.cannon.activateCanvas();
                        StartCoroutine(curr.Value.cannon.Shoot());
                        do
                        {
                            yield return new WaitForSeconds(rollingDelay);
                        } while (GameManager.instance.currentGameState != GameState.GS_BATTLE);
                        curr.Value.cannon.deactivateCanvas();
                    }
                    //curr.Value.cannon.setShootingDamage(10);

                }
                else if (curr.Previous.Previous == null)
                {
                    currChain.chainSum = curr.Value.modifier.getCurrentTotal();
                }
                curr = curr.Previous;
            }
        }
        //}
        /*
        foreach (LinkedListNode<MyTile> t in myChains[myChains.Count - 1].tileChain)
        {
            if (t.tileType == MyTile.TileType.COIN || t.tileType == MyTile.TileType.DICE)
            {
                t.modifier?.Roll();
                if(t == myChains[myChains.Count - 1].tileChain.First.Value)
                {
                    t.modifier.setCurrentTotal(t.modifier.getCurrentValue());
                }
                else
                {
                    t.modifier.calculateCurrentTotal(t.)
                }
                yield return new WaitForSeconds(rollingDelay);
            }
        }*/
    }

    private void ManageTransitions(Chain currChain)
    {
        LinkedListNode<MyTile> curr = currChain.tileChain.Last;
        while (curr != null)
        {
            if (curr.Value.tileType == MyTile.TileType.COIN || curr.Value.tileType == MyTile.TileType.DICE)
            {

            }
            curr = curr.Previous;
        }

    }

    private void AddOperationSigns(LinkedList<MyTile> tempList)
    {
        LinkedListNode<MyTile> curr = tempList.Last;
        ChainGenerator.Direction direction = ChainGenerator.Direction.TOP_LEFT;
        ChainGenerator.Operation operation = ChainGenerator.Operation.ADDITION;
        while (curr.Previous != null)
        {
            if (curr.Previous.Value.tileType == MyTile.TileType.CANNON)
                break;

            if (curr.Value.tileType == MyTile.TileType.DICE && curr.Previous.Value.tileType == MyTile.TileType.DICE)
                operation = ChainGenerator.Operation.ADDITION;
            else
                operation = ChainGenerator.Operation.MULTIPLICATION;


            if (curr.Value.tileType == MyTile.TileType.DICE || curr.Value.tileType == MyTile.TileType.COIN)
            {

                if (curr.Previous.Value == curr.Value.GetNeighbourAt(0))
                    direction = ChainGenerator.Direction.TOP_LEFT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(1))
                    direction = ChainGenerator.Direction.TOP_RIGHT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(2))
                    direction = ChainGenerator.Direction.MIDDLE_LEFT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(3))
                    direction = ChainGenerator.Direction.MIDDLE_RIGHT;
                else if (curr.Previous.Value == curr.Value.GetNeighbourAt(4))
                    direction = ChainGenerator.Direction.BOTTOM_LEFT;
                else
                    direction = ChainGenerator.Direction.BOTTOM_RIGHT;
            }
            curr.Value.modifier.SetOperation(direction, operation);

            curr = curr.Previous;
        }
    }

    private void RemoveOperationSigns(Chain chain)
    {
        foreach (MyTile tile in chain.tileChain)
        {
            if (tile.tileType == MyTile.TileType.DICE || tile.tileType == MyTile.TileType.COIN)
            {
                tile.modifier.EraseOperations();
            }
        }
    }

}