using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{
    private enum Grid{Empty, Floor, Start, Boss, Challenge, Shop, Camp}

    //Variables
    private Grid[,] gridHandler;
    private List<WalkerObject> walkers;
    public Tilemap tileMap;
    public Tile floor;
    public Tile startRoom;
    public Tile boss;
    public Tile challenge;
    public Tile shop;
    public Tile camp;
    public int mapWidth = 12;
    public int mapHeight = 12;
    public int maximumWalkers = 3;
    public int tileCount;
    public float fillPercentage = 0.25f;

    /// <summary>
    /// Called when the level starts
    /// </summary>
    void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// Generates a map by initialising an empty grid, creating the floors and then add special rooms.
    /// </summary>
    void GenerateMap()
    {
        InitializeGrid();
        CreateFloors();
        CreateSpecialRoom(boss, Grid.Boss, true);
        CreateSpecialRoom(shop, Grid.Shop, true);
        CreateSpecialRoom(challenge, Grid.Challenge, true);
        CreateSpecialRoom(challenge, Grid.Challenge, true);
        CreateSpecialRoom(camp, Grid.Camp, false);
    }

    /// <summary>
    /// Sets every tile in the map to be empty, then creates a walker in the center of the grid.
    /// </summary>
    void InitializeGrid()
    {
        gridHandler = new Grid[mapWidth, mapHeight];

        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)  // Set all tiles to be empty in the grid
            {
                gridHandler[x, y] = Grid.Empty;
            }
        }

        walkers = new List<WalkerObject>();                     // Store walkers here

        Vector3Int tileCenter = new Vector3Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2, 0);

        // Initialise a walker in the center.
        WalkerObject curWalker = new WalkerObject(new Vector2(tileCenter.x, tileCenter.y), WalkerObject.ChooseDirection());
        gridHandler[tileCenter.x, tileCenter.y] = Grid.Start;
        tileMap.SetTile(tileCenter, startRoom);
        walkers.Add(curWalker);

        tileCount++;
    }

    /// <summary>
    /// Uses random walkers to decide a layout for the map.
    /// </summary>
    void CreateFloors()
    {
        while (tileCount / (float)gridHandler.Length < fillPercentage) // Until X tiles are filled
        {
            foreach (WalkerObject curWalker in walkers) // Update each walker's tile
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (gridHandler[curPos.x, curPos.y] != Grid.Empty) continue;
                
                tileMap.SetTile(curPos, floor);
                tileCount++;
                gridHandler[curPos.x, curPos.y] = Grid.Floor;
            }

            //Walker Methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
        }
    }

    /// <summary>
    /// Uses random values to decide if a single walker should be destroyed at each step.
    /// </summary>
    void ChanceToRemove()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            if (Random.value < walkers[i].ChanceToRemove && walkers.Count > 1)
            {
                walkers.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Uses random values for each walker to decide if it should change direction.
    /// </summary>
    void ChanceToRedirect()
    {
        foreach (WalkerObject walker in walkers)
        {
            if (Random.value < walker.ChanceToChangeDirection)
            {
                walker.Direction = WalkerObject.ChooseDirection();
            }
        }
    }

    /// <summary>
    /// Uses random values for each walker to decide if a new walker should spawn in their position if it can.
    /// </summary>
    void ChanceToCreate()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            if (Random.value < walkers[i].ChanceToCreate && walkers.Count < maximumWalkers)
            {
                Vector2Int tileCenter = new Vector2Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2);

                WalkerObject newWalker = new WalkerObject(tileCenter, WalkerObject.ChooseDirection());
                walkers.Add(newWalker);
            }
        }
    }

    /// <summary>
    /// Moves each walker in their pre-decided direction, 
    /// also ensures the walkers stay within the bounds of the grid.
    /// </summary>
    void UpdatePosition()
    {
        foreach (WalkerObject walker in walkers)
        {
            walker.Position += walker.Direction;
            walker.ClampWithinGrid(gridHandler.GetLength(0), gridHandler.GetLength(1));
        }
    }

    /// <summary>
    /// Finds the number of rooms surrounding the current one which is marked as a FLOOR. <br/><br/>
    /// <param>gridHandler</param> Is locally available in this class, just use 'gridHandler' as the parameter. <br/>
    /// <param>x</param> 
    /// The x coordinate of the currently inspected room. <br/>
    /// <param>y</param> 
    /// The y coordinate of the currently inspected room. <br/>
    /// </summary>
    int GetNumberOfConnectingFloorRooms(int x, int y)
    {
        Vector2[] coords = new Vector2[]       // array of coords of adjacent room slots
        {
            new Vector2(x, y) + Vector2.right,
            new Vector2(x, y) + Vector2.left,
            new Vector2(x, y) + Vector2.up,
            new Vector2(x, y) + Vector2.down
        };

        int numOfConnectingRooms = 0;           // int tracker to be returned

        foreach (Vector2 coord in coords)       // loop through each adjacent room and see if it's a floor
        {
            if (gridHandler[(int)coord.x, (int)coord.y] == Grid.Empty) continue;
            
            numOfConnectingRooms++;
        }

        return numOfConnectingRooms;
    }

    /// <summary>
    /// Finds a room with a minimum amount of rooms adjacent to it. <br/><br/>
    /// <param>gridHandler</param> Is locally available in this class, just use 'gridHandler' as the parameter. <br/>
    /// </summary>
    Vector2 GetRoomWithMinimumConnections()
    {
        for (int numConnectingRooms = 1; numConnectingRooms < 4; numConnectingRooms++)
        {
            for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)      // Loops through the grid
            {
                for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)  
                {
                    if (gridHandler[x, y] != Grid.Floor) continue;      // Counting any form of room as a floor.

                    if (GetNumberOfConnectingFloorRooms(x, y) > numConnectingRooms) continue;
                    
                    return new Vector2(x, y);
                }
            } 
        }

        Debug.Log("Error : no appropriate room found");

        return new Vector2(0, 0);
    }

    /// <summary>
    /// Finds a room with a maximum amount of rooms adjacent to it. <br/><br/>
    /// <param>gridHandler</param> Is locally available in this class, just use 'gridHandler' as the parameter. <br/>
    /// </summary>
    Vector2 GetRoomWithMaximumConnections()
    {
        for (int numConnectingRooms = 4; numConnectingRooms > 0; numConnectingRooms--)
        {
            for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)      // Loops through the grid
            {
                for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)  
                {
                    if (gridHandler[x, y] != Grid.Floor) continue;      // Counting any form of room as a floor.

                    if (GetNumberOfConnectingFloorRooms(x, y) < numConnectingRooms) continue;
                    
                    return new Vector2(x, y);
                }
            } 
        }

        Debug.Log("Error : no appropriate room found");

        return new Vector2(0, 0);
    }

    /// <summary>
    /// Creates a special room, e.g a shop or boss on an already generated floor. <br/><br/>
    /// <param>RoomTile</param> The tile the room should show up as on the map. <br/>
    /// <param>RoomEnum</param> The corresponding room name in the Grid enum. <br/>
    /// </summary>
    void CreateSpecialRoom(Tile roomTile, Grid roomEnum, bool minRoomsRequired)
    {
        Vector2 validCoord = minRoomsRequired ? GetRoomWithMinimumConnections() : GetRoomWithMaximumConnections();
        
        int x = (int)validCoord.x;
        int y = (int)validCoord.y;

        gridHandler[x, y] = roomEnum;
        tileMap.SetTile(new Vector3Int(x, y), roomTile);
    }
}
