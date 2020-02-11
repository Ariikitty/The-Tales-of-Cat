using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // Using Serializable allows sub properties to be embeded in the unity editor
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximun;

        public Count (int min, int max)
        {
            minimum = min;
            maximun = max;
        }
    }

    //Setting the board size and prefabs the level generator will use for each object
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count (5,9);
    public Count foodCount = new Count (1,5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder; //Stores a reference to the transform of the Board object
    private List <Vector3> gridPositions = new List<Vector3>(); //A list of possible locations to place tiles

    //Clears the list gridPositions and prepares it to generate a new board
    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x,y,0f));
            }
        }
    }

    //Setting up the outer walls and floor of the game board
    void BoardSetup ()
    {
        boardHolder = new GameObject ("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3 (x,y,0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //RandomPosition returns a random position from the list of gridPositions
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximun)
    {
        int objectCount = Random.Range (minimum, maximun + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
            Instantiate (tileChoice, randomPosition, Quaternion.identity);
        }
    }

    //SetupScene initializes the level and calls the previous functions to lay out the game board
    public void SetupScene (int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximun);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximun);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows -1, 0f), Quaternion.identity);
    }
}
