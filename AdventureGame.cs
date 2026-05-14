namespace AdventureGame;

public class AdventureGame
{
    public readonly string GO_NORTH = "W";
    public readonly string GO_SOUTH = "S";
    public readonly string GO_EAST = "D";
    public readonly string GO_WEST = "A";
    public readonly string GET_LAMP = "L";
    public readonly string GET_KEY = "K";
    public readonly string OPEN_CHEST = "O";
    public readonly string QUIT = "Q";

    private Adventurer adventurer;
    private Room[,] dungeon;

    private int adventurerRow;
    private int adventurerCol;

    private int grueRow;
    private int grueCol;

    private int exitRow;
    private int exitCol;

    private bool isChestOpen;
    private bool hasPlayerQuit;
    private bool hasPlayerDied;
    private bool hasPlayerExitedDungeon;

    private string lastDirection;

    private const char Wall = '#';

    public AdventureGame()
    {

    }

    public void Start()
    {
        Init();

        ShowItemHints();

        ShowGameStartScreen();

        string input;

        do
        {
            ShowScene();

            do
            {
                ShowInputOptions();

                input = GetInput();
            }
            while (!IsValidInput(input));

            ProcessInput(input);

            UpdateGameState();
        }
        while (!IsGameOver());

        ShowGameOverScreen();
    }

    private void Init()
    {
        adventurer = new Adventurer();

        Load("Dungeon1.txt");

        isChestOpen = false;
        hasPlayerQuit = false;
        hasPlayerDied = false;
        hasPlayerExitedDungeon = false;

        lastDirection = string.Empty;
    }

    private void ShowGameStartScreen()
    {
        Console.WriteLine("Welcome to Adventure Game!");
    }

    private void ShowScene()
    {
        Room room = dungeon[adventurerRow, adventurerCol];

        if (adventurer.HasLamp() || room.IsLit())
        {
            Console.WriteLine(room.GetDescription());
        }
        else
        {
            Console.WriteLine("This room is pitch black!");
        }
    }

    private void ShowInputOptions()
    {
        string options = ""
        + $"GO NORTH [{GO_NORTH}] | GO EAST [{GO_EAST}] | GET LAMP [{GET_LAMP}] | OPEN CHEST [{OPEN_CHEST}]\n"
        + $"GO SOUTH [{GO_SOUTH}] | GO WEST [{GO_WEST}] | GET KEY  [{GET_KEY}] | QUIT       [{QUIT}]\n"
        + $"> ";

        Console.Write(options);
    }

    private string GetInput()
    {
        return Console.ReadLine()!.ToUpper();
    }

    private bool IsValidInput(string input)
    {
        string[] validInputs =
        {
            GO_NORTH,
            GO_SOUTH,
            GO_EAST,
            GO_WEST,
            GET_LAMP,
            GET_KEY,
            OPEN_CHEST,
            QUIT
        };

        if (!validInputs.Contains(input))
        {
            Console.WriteLine("ERROR: Invalid input. Please try again.");
            return false;
        }

        return true;
    }

    private void ProcessInput(string input)
    {
        Room room = dungeon[adventurerRow, adventurerCol];

        if (!adventurer.HasLamp() && !room.IsLit() && input != lastDirection)
        {
            Console.WriteLine("You got eaten alive by the Grue!");
            hasPlayerDied = true;
        }
        else if (input == GO_NORTH)
        {
            GoNorth(room);
        }
        else if (input == GO_SOUTH)
        {
            GoSouth(room);
        }
        else if (input == GO_EAST)
        {
            GoEast(room);
        }
        else if (input == GO_WEST)
        {
            GoWest(room);
        }
        else if (input == GET_LAMP)
        {
            GetLamp(room);
        }
        else if (input == GET_KEY)
        {
            GetKey(room);
        }
        else if (input == OPEN_CHEST)
        {
            OpenChest(room);
        }
        else
        {
            Quit();
        }
    }

    private void UpdateGameState()
    {
        if (isChestOpen)
        {
            List<(int row, int col)> path = FindPathToPlayer();

            if (path.Count > 1)
            {
                grueRow = path[1].row;
                grueCol = path[1].col;
            }

            hasPlayerDied =
                (grueRow == adventurerRow && grueCol == adventurerCol);

            hasPlayerExitedDungeon =
                (exitRow == adventurerRow && exitCol == adventurerCol);
        }
    }

    private bool IsGameOver()
    {
        return hasPlayerExitedDungeon || hasPlayerQuit || hasPlayerDied;
    }

    private void ShowGameOverScreen()
    {
        ShowScene();

        Console.WriteLine("Game Over!");

        if (hasPlayerDied)
        {
            Console.WriteLine("You got eaten alive by the Grue!");
        }
        else if (hasPlayerExitedDungeon)
        {
            Console.WriteLine("You miraculously escaped the dungeon! Congratulations!");
        }
        else if (hasPlayerQuit)
        {
            Console.WriteLine("You quit the game, see ya!");
        }
    }

    private void GoNorth(Room room)
    {
        if (room.HasNorth())
        {
            adventurerRow -= 1;
            lastDirection = GO_SOUTH;
        }
        else
        {
            Console.WriteLine("You cannot go north!\a");
        }
    }

    private void GoSouth(Room room)
    {
        if (room.HasSouth())
        {
            adventurerRow += 1;
            lastDirection = GO_NORTH;
        }
        else
        {
            Console.WriteLine("You cannot go south!\a");
        }
    }

    private void GoEast(Room room)
    {
        if (room.HasEast())
        {
            adventurerCol += 1;
            lastDirection = GO_WEST;
        }
        else
        {
            Console.WriteLine("You cannot go east!\a");
        }
    }

    private void GoWest(Room room)
    {
        if (room.HasWest())
        {
            adventurerCol -= 1;
            lastDirection = GO_EAST;
        }
        else
        {
            Console.WriteLine("You cannot go west!\a");
        }
    }

    private void GetLamp(Room room)
    {
        if (room.HasLamp())
        {
            Console.WriteLine("You got the lamp!");
            adventurer.SetLamp(true);
            room.SetLamp(false);
        }
        else
        {
            Console.WriteLine("There is no lamp in this room.");
        }
    }

    private void GetKey(Room room)
    {
        if (room.HasKey())
        {
            Console.WriteLine("You got the key!");
            adventurer.SetKey(true);
            room.SetKey(false);
        }
        else
        {
            Console.WriteLine("There is no key in this room.");
        }
    }

    private void OpenChest(Room room)
    {
        if (room.HasChest())
        {
            if (adventurer.HasKey())
            {
                Console.WriteLine("You got the treasure!");
                isChestOpen = true;
            }
            else
            {
                Console.WriteLine("You do not have the key!");
            }
        }
        else
        {
            Console.WriteLine("There is no chest in this room.");
        }
    }

    private void Quit()
    {
        Console.WriteLine("You quit the game, see ya!");
        hasPlayerQuit = true;
    }

    private List<(int row, int col)> GetAdjacents(int row, int col)
    {
        var adjacentRooms = new List<(int row, int col)>();

        Room room = dungeon[row, col];

        if (room.HasNorth()) { adjacentRooms.Add((row - 1, col)); }
        if (room.HasSouth()) { adjacentRooms.Add((row + 1, col)); }
        if (room.HasWest()) { adjacentRooms.Add((row, col - 1)); }
        if (room.HasEast()) { adjacentRooms.Add((row, col + 1)); }

        return adjacentRooms;
    }

    private List<(int row, int col)> FindPathToPlayer()
    {
        var start = (row: grueRow, col: grueCol);
        var goal = (row: adventurerRow, col: adventurerCol);

        var openSet = new PriorityQueue<(int row, int col), int>();
        var cameFrom = new Dictionary<(int row, int col), (int row, int col)>();
        var gScore = new Dictionary<(int row, int col), int>();

        gScore[start] = 0;

        openSet.Enqueue(start, Heuristic(start, goal));

        while (openSet.Count > 0)
        {
            var currentRoom = openSet.Dequeue();

            if (currentRoom == goal)
            {
                return ReconstructPath(cameFrom, currentRoom);
            }

            foreach (var adjacentRoom in GetAdjacents(currentRoom.row, currentRoom.col))
            {
                int newScore = gScore[currentRoom] + 1;

                if (!gScore.TryGetValue(adjacentRoom, out int existingScore)
                    || newScore < existingScore)
                {
                    cameFrom[adjacentRoom] = currentRoom;
                    gScore[adjacentRoom] = newScore;

                    int fScore = newScore + Heuristic(adjacentRoom, goal);

                    openSet.Enqueue(adjacentRoom, fScore);
                }
            }
        }

        return new List<(int row, int col)> { start };
    }

    private static int Heuristic(
        (int row, int col) a,
        (int row, int col) b)
    {
        return Math.Abs(a.row - b.row)
             + Math.Abs(a.col - b.col);
    }

    private static List<(int row, int col)> ReconstructPath(
        Dictionary<(int row, int col), (int row, int col)> cameFrom,
        (int row, int col) current)
    {
        var totalPath = new List<(int row, int col)>()
        {
            current
        };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        totalPath.Reverse();

        return totalPath;
    }

    public void Load(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        int rows = int.Parse(lines[0]);
        int cols = int.Parse(lines[1]);

        exitRow = int.Parse(lines[2]);
        exitCol = int.Parse(lines[3]);

        adventurerRow = int.Parse(lines[4]);
        adventurerCol = int.Parse(lines[5]);

        int lampRow = int.Parse(lines[6]);
        int lampCol = int.Parse(lines[7]);

        int keyRow = int.Parse(lines[8]);
        int keyCol = int.Parse(lines[9]);

        int chestRow = int.Parse(lines[10]);
        int chestCol = int.Parse(lines[11]);

        grueRow = int.Parse(lines[12]);
        grueCol = int.Parse(lines[13]);

        int layoutStart = 14;
        int descriptionsStart = layoutStart + rows;

        if (lines.Length < descriptionsStart)
        {
            throw new FormatException(
                "File does not contain enough layout rows.");
        }

        dungeon = new Room[rows, cols];

        List<(int row, int col)> traversableTiles = new();

        for (int row = 0; row < rows; row++)
        {
            string layoutLine = lines[layoutStart + row];

            if (layoutLine.Length != cols)
            {
                throw new FormatException(
                    $"Layout row {row} must contain exactly {cols} characters.");
            }

            for (int col = 0; col < cols; col++)
            {
                if (layoutLine[col] != Wall)
                {
                    dungeon[row, col] = new Room();
                    traversableTiles.Add((row, col));
                }
            }
        }

        int descriptionCount = lines.Length - descriptionsStart;

        if (descriptionCount != traversableTiles.Count)
        {
            throw new FormatException(
                $"Description count ({descriptionCount}) must match traversable tile count ({traversableTiles.Count})."
            );
        }

        for (int i = 0; i < traversableTiles.Count; i++)
        {
            string[] parts =
                lines[descriptionsStart + i].Split('|', 2);

            if (parts.Length != 2)
            {
                throw new FormatException(
                    $"Invalid room description line: {lines[descriptionsStart + i]}");
            }

            bool isLit = parts[0] switch
            {
                "1" => true,
                "0" => false,
                _ => throw new FormatException(
                    "Room lit value must be 1 or 0.")
            };

            string description = parts[1];

            var (row, col) = traversableTiles[i];

            Room room = dungeon[row, col];

            room.SetLit(isLit);
            room.SetDescription(description);

            room.SetLamp(row == lampRow && col == lampCol);
            room.SetKey(row == keyRow && col == keyCol);
            room.SetChest(row == chestRow && col == chestCol);

            room.SetNorth(IsTraversable(dungeon, row - 1, col));
            room.SetSouth(IsTraversable(dungeon, row + 1, col));
            room.SetEast(IsTraversable(dungeon, row, col + 1));
            room.SetWest(IsTraversable(dungeon, row, col - 1));
        }

        ValidateTraversableTile(
            dungeon,
            exitRow,
            exitCol,
            "exit");

        ValidateTraversableTile(
            dungeon,
            adventurerRow,
            adventurerCol,
            "adventurer");
    }

    private static bool IsTraversable(
        Room[,] dungeon,
        int row,
        int col)
    {
        return row >= 0
            && row < dungeon.GetLength(0)
            && col >= 0
            && col < dungeon.GetLength(1)
            && dungeon[row, col] != null;
    }

    private static void ValidateTraversableTile(
        Room[,] dungeon,
        int row,
        int col,
        string name)
    {
        if (!IsTraversable(dungeon, row, col))
        {
            throw new FormatException(
                $"The {name} position must be on a traversable tile.");
        }
    }
    private void ShowItemHints()
    {
        for (int row = 0; row < dungeon.GetLength(0); row++)
        {
            for (int col = 0; col < dungeon.GetLength(1); col++)
            {
                Room room = dungeon[row, col];
                if (room == null) continue;

                if (room.HasLamp())
                    Console.WriteLine($"Lamp is at ({row}, {col})");

                if (room.HasKey())
                    Console.WriteLine($"Key is at ({row}, {col})");

                if (room.HasChest())
                    Console.WriteLine($"Chest is at ({row}, {col})");
            }
        }
    }
}
