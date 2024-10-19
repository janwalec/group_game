using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RumController : MonoBehaviour
{
    double HP = 100;
    [SerializeField] private LayerMask enemyMask;
    public Canvas canvas;
    private float range = 2.0f;
    void Start()
    {
        //occupy tiles so that a cannon or a modifier cannot be placed there
        GameManager.instance.getTilemap().occupyTile(transform.position);
        changeText(((int)HP).ToString());
    }


    void Update()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            TakeDamage(hits.Length);
        }
    }
    void TakeDamage(int enemiesNum)
    {
        HP -= enemiesNum * 0.01;
        changeText(((int)HP).ToString());
    }

    public void changeText(string newText)
    {
        TextMeshProUGUI textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = newText;
    }
}
