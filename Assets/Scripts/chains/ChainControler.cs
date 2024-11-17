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

    public Color? color;


    public Chain(LinkedList<MyTile> _tileChain, List<PooledChain> _pooledObjectChain, int ID, Color? color_ = null, int chainSum_ = 0)
    {
        tileChain = _tileChain;
        pooledObjectChain = _pooledObjectChain;
        chainID = ID;
        chainSum = chainSum_;
        color = color_;

        if (_tileChain != null)
        {
            lineRenderer = ItemPool.SharedInstance.GetPooledLineRenderer().GetComponent<LineRendererController>();
            lineRenderer.gameObject.SetActive(true);
            //Debug.Log("Line rendered pooling");
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
        return new Chain(this.tileChain, this.pooledObjectChain, this.chainID, this.color, newSum);
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
    private float singleCannonExtraDelay = 2.5f;



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

        processChain(receivedChain);
        //rollAllModifiers();

    }

    public void StopRolling()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        /*
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
        }*/

        /*
        else if (Input.GetKeyDown(KeyCode.C))
        {
            findMaxLength();
            rollAllModifiers();
            
        }
        */
    }

    private MyTile searchForIntersection(LinkedList<MyTile> receivedChain) {
        foreach (MyTile t in receivedChain) {
            if (t.usedForChain) {
                return t;
            }
        }
        
        return null;
    }

    private Chain searchForChainWithGivenTile(MyTile intersection) {
        foreach (Chain ch in myChains) {
            if (ch.tileChain.Contains(intersection)) {
                return ch;
            }
        }
        return null;
    }

    private LinkedList<MyTile> connectChainsWithIntersection(LinkedList<MyTile> receivedChain, Chain chainThatContainsIntersection, MyTile intersection) {
        LinkedList<MyTile> outputChain = new LinkedList<MyTile>();
        
        // first part - chain drawn on the screen
        foreach (MyTile tile in receivedChain) {
            if (tile != intersection)
                outputChain.AddLast(tile);
            else
                break;
        }

        // second part - chain that contains intersetion

        bool foundIntersection = false;

        foreach (MyTile tile in chainThatContainsIntersection.tileChain) {
            if (tile == intersection)
                foundIntersection = true;
            
            if (foundIntersection)
                outputChain.AddLast(tile);
        }
        Debug.Log(outputChain.Count);
        return outputChain;
    }


     private bool checkIfCannonIsFirstInTheChain(LinkedList<MyTile> receivedChain) {
        if (receivedChain.First.Value.tileType != MyTile.TileType.CANNON) {
            return false;
        }
        return true;
    }

    private bool checkIfAddedAtTheEnd(MyTile intersection, Chain chainThatContainsIntersection) {
        if (chainThatContainsIntersection.tileChain.Last.Value == intersection)
            return true;
        return false;
    }

    public void extendPooledObjectsInChain(Chain chainToExtend, LinkedList<MyTile> receivedChain) {
        Color? c = chainToExtend.color;
        LinkedList<MyTile> withoutFirst = new LinkedList<MyTile>(receivedChain.Skip(1)); //skip first element
        List<PooledChain> pooled = poolObjects(withoutFirst, c ?? new Color(1, 1, 1));

        foreach (PooledChain p in pooled) {
            chainToExtend.pooledObjectChain.Add(p);
        }

    }

    

    public void extendChain(Chain chainToExtend, LinkedList<MyTile> receivedChain) {
        foreach(MyTile tile in receivedChain) {
            if(!chainToExtend.tileChain.Contains(tile)) {
                chainToExtend.tileChain.AddLast(tile);
                
            }
        }
        chainGenerator.markUsedForChainTiles(receivedChain, true);
        RemoveOperationSigns(chainToExtend);
        AddOperationSigns(chainToExtend.tileChain);
        chainToExtend.RenderLine();
        extendPooledObjectsInChain(chainToExtend, receivedChain);
    }

    private bool checkIfChainsIntersectCompletely(LinkedList<MyTile> receivedChain, Chain chainThatContainsIntersection) {
        int i = 0;
        foreach(MyTile t in receivedChain) {
            if (chainThatContainsIntersection.tileChain.Contains(t))
                i += 1;
        }

        if (i > 1 && receivedChain.Count > 1)
            return true;
        else if (i == 1 && receivedChain.Count == 1)
            return true;
        return false;
    }

    private void processChain(LinkedList<MyTile> receivedChain) {
        MyTile intersection = searchForIntersection(receivedChain);
        LinkedList<MyTile> outputChain = new LinkedList<MyTile>(); // idk why, but without it, doesnt work


        if (intersection != null) {
            Debug.Log("found intersection");
            Chain chainThatContainsIntersection = searchForChainWithGivenTile(intersection);

            if(checkIfChainsIntersectCompletely(receivedChain, chainThatContainsIntersection))
                return;

            if (!checkIfAddedAtTheEnd(intersection, chainThatContainsIntersection)) {
                outputChain = connectChainsWithIntersection(receivedChain, chainThatContainsIntersection, intersection);
                saveNewChain(outputChain);
            } else {
                if (checkIfCannonIsFirstInTheChain(receivedChain)) {
                    outputChain = connectChainsWithIntersection(receivedChain, chainThatContainsIntersection, intersection);
                    saveNewChain(outputChain);
                } else {
                    extendChain(chainThatContainsIntersection, receivedChain);
                }
            }
            
            

        } else {
            Debug.Log("did not find intersection");
            foreach(MyTile t in receivedChain)
                outputChain.AddLast(t);

            saveNewChain(outputChain);
        }
        
        
    }

    public List<PooledChain> poolObjects(LinkedList<MyTile> receivedChain, Color color) {
        int amount = receivedChain.Count;

        // get (transparent) object to indicate where Chain is
        List<PooledChain> pooledObjects = chainPool.GetObjects(amount);


        int i = 0;
        foreach (MyTile t in receivedChain) {
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

        return pooledObjects;
    }
   

    private void saveNewChain(LinkedList<MyTile> receivedChain)
    { // get completed Chain with its tiles
        //LinkedList<MyTile> receivedChain = searchForInteresctionsAndChangeChain(_receivedChain);

        if (!checkIfCannonIsFirstInTheChain(receivedChain))
            return;
        

        
        AddOperationSigns(receivedChain);
        
        // init random color for a Chain
        System.Random rnd = new System.Random();
        int r = rnd.Next(256), g = rnd.Next(256), b = rnd.Next(256);
        Color color = new Color(r / 255f, g / 255f, b / 255f, 1f);

        List<PooledChain> pooledObjects = poolObjects(receivedChain, color); 
        
        chainGenerator.markUsedForChainTiles(receivedChain, true);
        // create new Chain, set the text to it and save
        Chain newChain = new Chain(receivedChain, pooledObjects, currID++, color);
        //setTextForChain(newChain);
        
        myChains.Add(newChain);
        
        if (newChain.tileChain.Count > maxChainLeght)
        {
            maxChainLeght = newChain.tileChain.Count;
        }

        //StartCoroutine(rolling(newChain));

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

        StopCoroutine(rolling(toDelete));
        myChains.Remove(toDelete);
        updateNotDeletedChains();

    }


    public void deleteChainByElement(MovableItem item)
    {
        List<int> chainsToDelete = new List<int>();
        foreach(Chain ch in myChains)
        {
            foreach (MyTile tile in ch.tileChain)
            {
                if (tile.tileType == MyTile.TileType.CANNON)
                {
                    if (tile.cannon == item)
                    {
                        chainsToDelete.Add(ch.chainID);
                        break;
                    }
                }
                else
                {
                    if (tile.modifier == item)
                    {
                        chainsToDelete.Add(ch.chainID);
                        break;
                    }
                }
            }
        }
        foreach(int id in chainsToDelete)
        {
            Debug.Log("Deleting chain");
            deleteChain(id);
        }
    }

    private Dictionary<Chain, Coroutine> activeCoroutines = new Dictionary<Chain, Coroutine>();

    public void rollAllModifiers()
    {

        foreach (Chain ch in myChains)
        {
            //StartCoroutine(rolling(ch));
            Coroutine coroutine = StartCoroutine(rolling(ch));
            activeCoroutines[ch] = coroutine;
        }

    }

    public void clearCourtine() {
        foreach (var coroutine in activeCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();
    }

    public void resetAnimations() {
        foreach (Chain ch in myChains) {
            foreach (MyTile curr in ch.tileChain) {
                if (curr.tileType == MyTile.TileType.COIN || curr.tileType == MyTile.TileType.DICE) {
                    curr.modifier.resetAnimation();
                    Debug.Log(curr.modifier + "RESET");
                }

            }
        }
    }

    //for now: cannons base damage works if it is only the cannon in the chain
    //otherwise damage is computed ignoring cannon base damage

    private IEnumerator rolling(Chain currChain)
    {
        while (true)
        {
            while (GameManager.instance.currentGameState != GameState.GS_BATTLE)
            {
                resetAnimations();
                yield return (rollingDelay);
            }


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
                            //yield return new WaitForSeconds(rollingDelay * (maxChainLeght + 1 - currChain.tileChain.Count));
                            yield return new WaitForSeconds(rollingDelay);
                        } while (GameManager.instance.currentGameState != GameState.GS_BATTLE);
                        //curr.Value.cannon.setShootingDamage(myChains[myChains.Count - 1].chainSum);

                        if (currChain.tileChain.Count == 1)
                        {
                            Debug.Log("Just one");
                            yield return new WaitForSeconds(singleCannonExtraDelay);
                            curr.Value.cannon.setDamageAsBaseDamage();
                            curr.Value.cannon.setSlowingEffect(newSpeed);
                        }
                        else
                        {
                            curr.Value.cannon.setExtraShootingDamage(currChain.chainSum);
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