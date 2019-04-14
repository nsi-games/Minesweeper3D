using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Minesweeper3D
{
    public class Tile : MonoBehaviour
    {
        public int x, y, z; // Coordinate in 2D array of Grid
        public bool isMine = false; // Is this tile a mine?
        public bool isRevealed = false; // Is this tile revealed?
        public bool isFlagged = false; // Is this tile flagged?
        public GameObject minePrefab, textPrefab; // Reference to mine & text prefabs
        public Gradient textGradient; // Gradient used for coloring text
        public Color flagColor;
        public Renderer rend;
        [Range(0, 1)]
        public float mineChance = 0.15f; // Mine Percentage (0% - 100%)
        // References to components
        private Animator anim;
        private Collider col;
        // References to spawned mine / text objects
        private GameObject mine, text;
        private Color originalColor;
        // Awake runs before Start (good for getting components)
        void Awake()
        {
            // Get reference to components
            anim = GetComponent<Animator>();
            col = GetComponent<Collider>();
        }
        // Spawns a given prefab as a child
        GameObject SpawnChild(GameObject prefab)
        {
            GameObject child = Instantiate(prefab, transform);
            // Centres child
            child.transform.localPosition = Vector3.zero; 
            // Deactivates child
            child.SetActive(false);
            return child;
        }
        // Use this for initialization
        void Start()
        {
            originalColor = rend.material.color;
            // Set mine chance
            isMine = Random.value < mineChance;
            // Check if it's a mine
            if(isMine)
            {
                // Spawn mine gameobject as childd
                mine = SpawnChild(minePrefab);
            }
            else
            {
                // Spawn text gameobject as child
                text = SpawnChild(textPrefab);
            }
        }

        // Reveals a tile with optional adjacent mines
        public void Reveal(int adjacentMines = 0)
        {
            // Flags the tile as being revealed
            isRevealed = true;
            // Run Reveal animation
            anim.SetTrigger("Reveal");
            // Disable collision
            col.enabled = false;
            // Check if tile is mine
            if (isMine)
            {
                // Activate mine
                mine.SetActive(true);
            }
            else
            {
                // Is there any adjacent mines?
                if (adjacentMines > 0)
                {
                    // Enable the text
                    text.SetActive(true);
                    TextMeshPro tmp = text.GetComponent<TextMeshPro>();
                    // Setting text color
                    float time = adjacentMines / 9f; // Lerp the adjacent mines
                    tmp.color = textGradient.Evaluate(time);
                    // Setting text value
                    tmp.text = adjacentMines.ToString();
                }
            }
        }

        public void Flag()
        {
            // Toggle flagged
            isFlagged = !isFlagged;
            // Change the material
            rend.material.color = isFlagged ? flagColor : originalColor;
        }
    }

}