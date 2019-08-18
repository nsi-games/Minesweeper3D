using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y, z; // Coordinate in 2D array of Grid
    public bool isMine = false; // Is this tile a mine?
    public bool isRevealed = false; // Is this tile revealed?
    public bool isFlagged = false; // Is this tile flagged?
    [Range(0f, 1f)] public float mineChance = 0.05f;
    public Sprite[] emptySprites;
    public Sprite[] mineSprites;
    public Sprite flagSprite;
    public SpriteRenderer rend;

    private Sprite originalSprite;

    // Awake runs before Start (good for getting components)
    private void Awake()
    {
        // Get reference to components
        rend = GetComponent<SpriteRenderer>();
        // Record the original sprite for later
        originalSprite = rend.sprite;
    }
    // Use this for initialization
    private void Start()
    {
        // Set mine chance
        isMine = Random.value < mineChance;
    }
    // Spawns a given prefab as a child
    private GameObject SpawnChild(GameObject prefab)
    {
        GameObject child = Instantiate(prefab, transform);
        // Centres child
        child.transform.localPosition = Vector3.zero;
        // Deactivates child
        child.SetActive(false);
        return child;
    }

    // Reveals a tile with optional adjacent mines
    public void Reveal(int adjacentMines, int mineState = 0)
    {
        // Flags the tile as being revealed
        isRevealed = true;
        // Check if tile is mine
        if (isMine)
        {
            // Activate mine
            rend.sprite = mineSprites[mineState];
        }
        else
        {
            rend.sprite = emptySprites[adjacentMines];
        }
    }
    public void Flag()
    {
        // Toggle flagged
        isFlagged = !isFlagged;
        // Change the material
        rend.sprite = isFlagged ? flagSprite : originalSprite;
    }
}
