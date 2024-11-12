using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{
    public enum Grid{EMPTY, FLOOR, START, BOSS, CHALLENGE, SHOP, CAMP}

    //Variables
    public Grid[,] gridHandler;
    public List<WalkerObject> Walkers;
    public Tilemap tileMap;
    public Tile Floor;
    public Tile StartRoom;
    public Tile Boss;
    public Tile Challenge;
    public Tile Shop;
    public Tile Camp;
    public int MapWidth = 12;
    public int MapHeight = 12;
    public int MaximumWalkers = 3;
    public int TileCount = 0;
    public float FillPercentage = 0.25f;

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
        CreateSpecialRoom(Boss, Grid.BOSS, true);
        CreateSpecialRoom(Shop, Grid.SHOP, true);
        CreateSpecialRoom(Challenge, Grid.CHALLENGE, true);
        CreateSpecialRoom(Challenge, Grid.CHALLENGE, true);
        CreateSpecialRoom(Camp, Grid.CAMP, false);
    }

    /// <summary>
    /// Sets every tile in the map to be empty, then creates a walker in the center of the grid.
    /// </summary>
    void InitializeGrid()
    {
        gridHandler = new Grid[MapWidth, MapHeight];

        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)  // Set all tiles to be empty in the grid
            {
                gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<WalkerObject>();                     // Store walkers here

        Vector3Int TileCenter = new Vector3Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2, 0);

        // Initialise a walker in the center.
        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x, TileCenter.y), WalkerObject.ChooseDirection());
        gridHandler[TileCenter.x, TileCenter.y] = Grid.START;
        tileMap.SetTile(TileCenter, StartRoom);
        Walkers.Add(curWalker);

        TileCount++;
    }

    /// <summary>
    /// Uses random walkers to decide a layout for the map.
    /// </summary>
    void CreateFloors()
    {
        while ((float)TileCount / (float)gridHandler.Length < FillPercentage) // Until X tiles are filled
        {
            foreach (WalkerObject curWalker in Walkers) // Update each walker's tile
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (gridHandler[curPos.x, curPos.y] != Grid.EMPTY) continue;
                
                tileMap.SetTile(curPos, Floor);
                TileCount++;
                gridHandler[curPos.x, curPos.y] = Grid.FLOOR;
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
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToRemove && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Uses random values for each walker to decide if it should change direction.
    /// </summary>
    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (Random.value < Walkers[i].ChanceToChangeDirection)
            {
                Walkers[i].Direction = WalkerObject.ChooseDirection();
            }
        }
    }

    /// <summary>
    /// Uses random values for each walker to decide if a new walker should spawn in their position if it can.
    /// </summary>
    void ChanceToCreate()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (Random.value < Walkers[i].ChanceToCreate && Walkers.Count < MaximumWalkers)
            {
                Vector2Int TileCenter = new Vector2Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2);

                WalkerObject newWalker = new WalkerObject(TileCenter, WalkerObject.ChooseDirection());
                Walkers.Add(newWalker);
            }
        }
    }

    /// <summary>
    /// Moves each walker in their pre-decided direction, 
    /// also ensures the walkers stay within the bounds of the grid.
    /// </summary>
    void UpdatePosition()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            Walkers[i].Position += Walkers[i].Direction;
            Walkers[i].ClampWithinGrid(gridHandler.GetLength(0), gridHandler.GetLength(1));
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
    int GetNumberOfConnectingFloorRooms(Grid[,] gridHandler, int x, int y)
    {
        Vector2[] coords = new Vector2[4]       // array of coords of adjacent room slots
        {
            new Vector2(x, y) + Vector2.right,
            new Vector2(x, y) + Vector2.left,
            new Vector2(x, y) + Vector2.up,
            new Vector2(x, y) + Vector2.down
        };

        int numOfConnectingRooms = 0;           // int tracker to be returned

        foreach (Vector2 coord in coords)       // loop through each adjacent room and see if its a floor
        {
            if (gridHandler[(int)coord.x, (int)coord.y] == Grid.EMPTY) continue;
            
            numOfConnectingRooms++;
        }

        return numOfConnectingRooms;
    }

    /// <summary>
    /// Finds a room with a minimum amount of rooms adjacent to it. <br/><br/>
    /// <param>gridHandler</param> Is locally available in this class, just use 'gridHandler' as the parameter. <br/>
    /// </summary>
    Vector2 GetRoomWithMinimumConnections(Grid[,] gridHandler)
    {
        for (int numConnectingRooms = 1; numConnectingRooms < 4; numConnectingRooms++)
        {
            for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)      // Loops through the grid
            {
                for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)  
                {
                    if (gridHandler[x, y] != Grid.FLOOR) continue;      // Counting any form of room as a floor.

                    if (GetNumberOfConnectingFloorRooms(gridHandler, x, y) > numConnectingRooms) continue;
                    
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
    Vector2 GetRoomWithMaximumConnections(Grid[,] gridHandler)
    {
        for (int numConnectingRooms = 4; numConnectingRooms > 0; numConnectingRooms--)
        {
            for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)      // Loops through the grid
            {
                for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)  
                {
                    if (gridHandler[x, y] != Grid.FLOOR) continue;      // Counting any form of room as a floor.

                    if (GetNumberOfConnectingFloorRooms(gridHandler, x, y) < numConnectingRooms) continue;
                    
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
    void CreateSpecialRoom(Tile RoomTile, Grid RoomEnum, bool minRoomsRequired)
    {
        Vector2 validCoord;

        if (minRoomsRequired)
        {
            validCoord = GetRoomWithMinimumConnections(gridHandler);
        }
        else
        {
            validCoord = GetRoomWithMaximumConnections(gridHandler);
        }
        
        int x = (int)validCoord.x;
        int y = (int)validCoord.y;

        gridHandler[x, y] = RoomEnum;
        tileMap.SetTile(new Vector3Int(x, y), RoomTile);
    }
}
