using UnityEngine;


public class TextBasedAdventure : MonoBehaviour
{
    [System.Serializable]
    public struct Room
    {
        public string Name;
        public TileType Type;
        public string Description;
        public bool HasRoomBeenVisited;
    }

    [System.Serializable]
    public struct RoomRow
    {
        public Room[] rooms;
    }

    public enum TileType
    {
        Invalid,
        Empty,
        Item,
        Enemy,
        Exit,
        Teleport,
        Blockade,
    }

    private Room[,] dungeon = {
                                {   new Room { Name = "Dark Cave",    Type = TileType.Empty, Description = "It's almost too dark to see...", HasRoomBeenVisited = false },
                                    new Room { Name = "Mossy Tunnel", Type = TileType.Empty, Description = "The moss caked ground is easy to slip on", HasRoomBeenVisited = false },
                                    new Room { Name = "Crystal Room", Type = TileType.Empty, Description = "Light refracts and bounces in this room like nothing you've seen before", HasRoomBeenVisited = false },
                                    new Room { Name = "Teleporter",   Type = TileType.Teleport, Description = "A strange plate lies in the middle of the room", HasRoomBeenVisited = false } },

                                {   new Room { Name = "Bone Chamber", Type = TileType.Enemy, Description = "You can almost taste the calcium particles floating in the air", HasRoomBeenVisited = false },
                                    new Room { Name = "Flooded Hall", Type = TileType.Empty, Description = "The water in this hall goes up to your knees...", HasRoomBeenVisited = false },
                                    new Room { Name = "Forge",    Type = TileType.Item, Description = "A furnace, still kindled, hums faintly", HasRoomBeenVisited = false },
                                    new Room { Name = "Iron Gate",    Type = TileType.Exit, Description = "An iron gate, presumably the exit bars your path", HasRoomBeenVisited = false } },

                                {   new Room { Name = "Teleporter", Type = TileType.Teleport, Description = "A strange plate lies in the middle of the room", HasRoomBeenVisited = false },
                                    new Room { Name = "Goblin Den", Type = TileType.Enemy, Description = "Scampering can be heard faintly", HasRoomBeenVisited = false },
                                    new Room { Name = "Armory",    Type = TileType.Item, Description = "Suits of armor line the walls, some carrying grim weaponry", HasRoomBeenVisited = false },
                                    new Room { Name = "Throne Room", Type = TileType.Empty, Description = "A lonely throne sits at the end of the hall", HasRoomBeenVisited = false } },

                                {   new Room { Name = "Blockade", Type = TileType.Blockade, Description = "A wall blocks your path, you cannot proceed this way", HasRoomBeenVisited = false },
                                    new Room { Name = "Pitfall Bridge", Type = TileType.Empty, Description = "It'd be all too easy to slip and fall into the pit below", HasRoomBeenVisited = false },
                                    new Room { Name = "Courtyard",    Type = TileType.Empty, Description = "The sun feels nice on the skin", HasRoomBeenVisited = false },
                                    new Room { Name = "Living Quarters", Type = TileType.Item, Description = "Furniture indicates this to be a lived-in space", HasRoomBeenVisited = false} }
                                };

    private int playerRow = 0;
    private int playerCol = 0;
    private int playerHealth = 10;
    private int enemyDamage = 1;
    private int itemHealAmount = 2;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPlayerPosition(playerRow, playerCol);
        OutputTileInformation();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        bool wasKeyPressed = HandleInput(out int newRow, out int newCol);
        if (!wasKeyPressed)
        {
            return;
        }
        SetPlayerPosition(newRow, newCol);
       // HasRoomBeenVisited = true;
        OutputTileInformation();
    }

    private void OutputTileInformation()
    {
        Debug.Log("You are in: " + dungeon[playerRow, playerCol].Name);

        if (!dungeon[playerRow, playerCol].HasRoomBeenVisited)
        {
            Debug.Log(dungeon[playerRow, playerCol].Description);
            dungeon[playerRow, playerCol].HasRoomBeenVisited = true;
        }

        switch (dungeon[playerRow, playerCol].Type)
        {
            case TileType.Empty:
                Debug.Log("There is nothing here.");
                break;
            case TileType.Enemy:
                Debug.Log("Oooo a spooky ghost");
                EncounterEnemy();
                break;
            case TileType.Item:
                Debug.Log("You see a shiny object");
                ItemPickup();
                break;
            case TileType.Exit:
                Debug.Log("You see a way out");
                break;
            case TileType.Blockade:
                Debug.Log("You cannot proceed");
                break;
            case TileType.Teleport:
                Debug.Log("You can teleport here");
                break;
            default:
                Debug.LogError("Invalid TileType");
                break;
        }
    }

    private void EncounterEnemy()
    {
        PlayerTakeDamage(enemyDamage);
    }
    
    private void ItemPickup()
    {
        PlayerHeal(itemHealAmount);
    }

    private void PlayerHeal(int heal)
    {
        playerHealth += heal;
        Debug.Log("You get healed. Your health is now " + playerHealth);
    }

    private void PlayerTakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log("You get hit. Your health is now " + playerHealth);
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("You are dead");
        }
    }

    /// <summary>
    /// Sets the player position to a new row and column position
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    private void SetPlayerPosition(int newRow, int newCol)
    {
        if (CheckIfNewPositionInTileBounds(newRow, newCol))
        {
            if (dungeon[newRow, newCol].Type != TileType.Blockade)
            {
                playerRow = newRow;
                playerCol = newCol;
            }
            else
            {
                Debug.Log("A blockade lies here");
            }
        }
        else
        {
            Debug.Log("Can't go that way");
        }
    }

    /// <summary>
    /// Determine if the new row and column position are within the bounds of the tiles
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    /// <returns>True if it is within the bounds, false if not</returns>
    private bool CheckIfNewPositionInTileBounds(int newRow, int newCol)
    {
        return (newRow >= 0 && newRow < dungeon.GetLength(0)) && (newCol >= 0 && newCol < dungeon.GetLength(1));
    }

    /// <summary>
    /// Handles the player's input and sets potential new position in the tileNames array
    /// </summary>
    /// <param name="newRow">new row position</param>
    /// <param name="newCol">new column position</param>
    /// <returns>True if an input was pressed, false if not</returns>
    private bool HandleInput(out int newRow, out int newCol)
    {
        bool hasPressedKey = true;
        newRow = playerRow;
        newCol = playerCol;
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("You pressed " + KeyCode.D);
            newCol++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("You pressed " + KeyCode.A);
            newCol--;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("You pressed " + KeyCode.W);
            newRow--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("You pressed " + KeyCode.S);
            newRow++;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(dungeon[playerRow, playerCol].Description);
        }
        else if (dungeon[playerRow, playerCol].Type == TileType.Teleport)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                dungeon[playerRow, playerCol].Type = TileType.Teleport;
            }
        }
        else
        {
            hasPressedKey = false;
        }
        return hasPressedKey;
    }

}
