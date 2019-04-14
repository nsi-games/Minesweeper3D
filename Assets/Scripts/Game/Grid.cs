using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper3D
{
    public class Grid : MonoBehaviour
    {
        public GameObject tilePrefab; // Prefab to spawn
        public int width = 10, height = 10, depth = 10; // Grid dimensions
        public int mineCount = 5;
        public float spacing = 1.1f; // Spacing between each tile
        private Tile[,,] tiles; // 3D Array to store all the tiles

        

        Tile SpawnTile(Vector3 position)
        {
            // Clone the tile prefab
            GameObject clone = Instantiate(tilePrefab, transform);
            // Edit it's properties
            clone.transform.position = position;
            // Return the Tile component of clone
            return clone.GetComponent<Tile>();
        }
        void GenerateTiles()
        {
            // Instantiate the new 3D array of size width x height x depth
            tiles = new Tile[width, height, depth];

            // Store half the size of the grid
            Vector3 halfSize = new Vector3(width * .5f, height * .5f, depth * .5f);

            // Offset
            Vector3 offset = new Vector3(.5f, .5f, .5f);
            
            // Loop through the entire list of tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        // Generate position for current tile
                        Vector3 position = new Vector3(x - halfSize.x,
                                                       y - halfSize.y,
                                                       z - halfSize.z);
                        // Offset position to center
                        position += offset;
                        // Apply spacing
                        position *= spacing;
                        // Spawn a new tile
                        Tile newTile = SpawnTile(position);
                        // Store coordinates
                        newTile.x = x;
                        newTile.y = y;
                        newTile.z = z;
                        // Store tile in array at those coordinates
                        tiles[x, y, z] = newTile;
                    }
                }
            }
        }
        // Use this for initialization
        void Start()
        {
            GenerateTiles();
        }
        bool IsOutOfBounds(int x, int y, int z)
        {
            return x < 0 || x >= width ||
                   y < 0 || y >= height ||
                   z < 0 || z >= depth;
        }
        int GetAdjacentMineCount(Tile tile)
        {
            // Set count to 0
            int count = 0;
            // Loop through all the adjacent tiles
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        // Calculate which adjacent tile to look at
                        int desiredX = tile.x + x;
                        int desiredY = tile.y + y;
                        int desiredZ = tile.z + z;
                        // Check if the desired x & y is outside bounds
                        if (IsOutOfBounds(desiredX, desiredY, desiredZ))
                        {
                            // Continue to next element in the loop
                            continue;
                        }
                        // Select current tile
                        Tile currentTile = tiles[desiredX, desiredY, desiredZ];
                        // Check if that tile is a mine
                        if (currentTile.isMine)
                        {
                            // Increase count by 1
                            count++;
                        }
                    }
                }
            }
            // Remember to return the count!
            return count;
        }
        void FFuncover(int x, int y, int z, bool[,,] visited)
        {
            // Is x and y out of bounds of the grid?
            if (IsOutOfBounds(x, y, z))
            {
                // Exit
                return;
            }

            // Have the coordinates already been visited?
            if (visited[x, y, z])
            {
                // Exit
                return;
            }
            // Reveal that tile in that X and Y coordinate
            Tile tile = tiles[x, y, z];
            // Get number of mines around that tile
            int adjacentMines = GetAdjacentMineCount(tile);
            // Reveal the tile
            tile.Reveal(adjacentMines);

            // If there are no adjacent mines around that tile
            if (adjacentMines == 0)
            {
                // This tile has been visited
                visited[x, y, z] = true;
                // Visit all other tiles around this tile
                FFuncover(x - 1, y, z, visited);
                FFuncover(x + 1, y, z, visited);

                FFuncover(x, y - 1, z, visited);
                FFuncover(x, y + 1, z, visited);

                FFuncover(x, y, z - 1, visited);
                FFuncover(x, y, z + 1, visited);
            }
        }
        // Scans the grid to check if there are no more empty tiles
        bool NoMoreEmptyTiles()
        {
            // Set empty tile count to 0
            int emptyTileCount = 0;
            // Loop through 2D array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        Tile tile = tiles[x, y, z];
                        // If tile is revealed or is a mine
                        if (tile.isRevealed || tile.isMine)
                        {
                            // Skip to next loop iteration
                            continue;
                        }
                        // An empty tile has not been revealed
                        emptyTileCount++;
                    }
                }
            }
            // Return true if all empty tiles have been revealed
            return emptyTileCount == 0;
        }
        // Uncovers all mines in the grid
        void UncoverAllMines()
        {
            // Loop through entire grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        Tile tile = tiles[x, y, z];
                        // Check if tile is a mine
                        if (tile.isMine)
                        {
                            // Reveal that tile
                            tile.Reveal();
                        }
                    }
                }
            }
        }
        // Performs set of actions on selected tile
        void SelectTile(Tile selected)
        {
            int adjacentMines = GetAdjacentMineCount(selected);
            selected.Reveal(adjacentMines);

            // Is the selected tile a mine?
            if (selected.isMine)
            {
                // Uncover all mines
                UncoverAllMines();
                // Game Over - Lose
                print("Game Over - You loose.");
            }
            // Else, are there no more mines around this tile?
            else if (adjacentMines == 0)
            {
                int x = selected.x;
                int y = selected.y;
                int z = selected.z;
                // Use Flood Fill to uncover all adjacent mines
                FFuncover(x, y, z, new bool[width, height, depth]);
            }
            // Are there no more empty tiles in the game at this point?
            if (NoMoreEmptyTiles())
            {
                //  Uncover all mines
                UncoverAllMines();
                // Game Over - Win
                print("Game Over - You Win!");
            }
        }

        Tile GetHitTile(Vector2 mousePosition)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit))
            {
                return hit.collider.GetComponent<Tile>();
            }
            return null;
        }
        // Raycasts to find a hit tile
        void MouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Tile hitTile = GetHitTile(Input.mousePosition);
                if (hitTile)
                {
                    SelectTile(hitTile);
                }
            }
            if(Input.GetMouseButtonDown(2))
            {
                Tile hitTile = GetHitTile(Input.mousePosition);
                if (hitTile)
                {
                    hitTile.Flag();
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            MouseOver();
            UpdateGrid();
        }

        void UpdateGrid()
        {
            // Store half the size of the grid
            Vector3 halfSize = new Vector3(width * .5f, height * .5f, depth * .5f);

            // Offset
            Vector3 offset = new Vector3(.5f, .5f, .5f);

            // Loop through the entire list of tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        // Generate position for current tile
                        Vector3 position = new Vector3(x - halfSize.x,
                                                       y - halfSize.y,
                                                       z - halfSize.z);
                        // Offset position to center
                        position += offset;
                        // Apply spacing
                        position *= spacing;
                        // Spawn a new tile
                        Tile tile = tiles[x, y, z];


                        tile.transform.position = position;
                    }
                }
            }
        }
    }

}