const int SEPARATOR_WIDTH = 2;

const int PLAYDROUND_SIZE = 3;

const int GAME_FIELD_START_LEFT = 9;
const int GAME_FIELD_START_TOP = 3;

//-------------------------------------------------------------------------------------------------------

PlaygroundFieldsValue[,] playground = PlaygroundFill(new PlaygroundFieldsValue[PLAYDROUND_SIZE, PLAYDROUND_SIZE]);

bool isPlayerStep = false;

Winner winner = Winner.None;

PrintPlayground(playground);


while (winner == Winner.None)
{
    isPlayerStep = !isPlayerStep;

    if (isPlayerStep)
    {
        InputUsersFieldValue(playground);
    }
    else
    {
        InputComputerFieldValue(playground);
    }

    Console.Clear();
    PrintPlayground(playground);


    winner = CheckPlayground(playground);
}

PrintWhoIsWinner(winner);

Console.ReadKey();

//-------------------------------------------------------------------------------------------------------

static PlaygroundFieldsValue[,] PlaygroundFill(PlaygroundFieldsValue[,] playground)
{
    for (int i = 0; i < playground.GetLength(0); i++)
    {
        for (int j = 0; j < playground.GetLength(1); j++)
        {
            playground[i,j] = PlaygroundFieldsValue.Empty;
        }
    }
    return playground;
}


static void PrintPlayground(PlaygroundFieldsValue[,] playground)
{
    for (int i = 0; i < playground.GetLength(0); i++)
    {
        Console.SetCursorPosition(1, 1);
        Console.WriteLine("К Р Е С Т И К И - Н О Л И К И");

        Console.SetCursorPosition(GAME_FIELD_START_LEFT, (GAME_FIELD_START_TOP + (i * 2)));

        for (int j = 0; j < playground.GetLength(1); j++)
        {
            PlaygroundFieldsValue cellInput = playground[i,j];
            string cell;

            switch (cellInput)
            {
                case PlaygroundFieldsValue.Player:
                    cell = $"{"X",2} ";
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case PlaygroundFieldsValue.Computer:
                    cell = $"{"O",2} ";
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                default:
                    cell = $"{_getPlaceholder(i,j, playground),2} ";
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.Write(cell);

            if (j != playground.GetLength(1) - 1)
            {
                string separator = new string(' ', SEPARATOR_WIDTH);
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.Write(separator);
            }
            Console.ResetColor();
        }
        Console.WriteLine();
        if (i != playground.GetLength(0) - 1)
        {
            Console.SetCursorPosition(GAME_FIELD_START_LEFT, (GAME_FIELD_START_TOP + (i * 2 + 1)));

            int playgroundWidth = (3 /*(cell.Lenght)*/ * playground.GetLength(1)) + (SEPARATOR_WIDTH * (playground.GetLength(1) - 1));
            string separator = new string(' ', playgroundWidth);
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine(separator);
        }
        Console.ResetColor();
    }
}


static void InputUsersFieldValue(PlaygroundFieldsValue[,] playground)
{
    int top = GAME_FIELD_START_TOP + (playground.GetLength(0) * 2 - 1) + 1;

    Console.SetCursorPosition(1, top);
    Console.Write("Введите номер поля: ");


    bool isVacanted = true;

    int cursorLeftPosition = Console.CursorLeft;
    int cursorTopPosition = Console.CursorTop;

    while (isVacanted)
    {
        Console.SetCursorPosition(cursorLeftPosition, cursorTopPosition);
        Console.WriteLine(new String(' ', 25));
        Console.SetCursorPosition(cursorLeftPosition, cursorTopPosition);
        string inputText = Console.ReadLine();
        int inputNumber = _toInt32(inputText);
        if (inputNumber <= 0 || inputNumber > playground.Length)
        {
            continue;
        }
        int[] coordinate = _getCoordinates(inputNumber, playground);
        if (playground[coordinate[0], coordinate[1]] != PlaygroundFieldsValue.Empty)
        {
            continue;
        }

        playground[coordinate[0], coordinate[1]] = PlaygroundFieldsValue.Player;
        isVacanted = false;
    }
}

static void InputComputerFieldValue(PlaygroundFieldsValue[,] playground)
{
    Random rnd = new Random();

    while (true)
    {
        int rndRow = rnd.Next(0, PLAYDROUND_SIZE);
        int rndCol = rnd.Next(0, PLAYDROUND_SIZE);

        if (playground[rndRow, rndCol] == PlaygroundFieldsValue.Empty)
        {
            playground[rndRow, rndCol] = PlaygroundFieldsValue.Computer;
            return;
        }
    }
}


static Winner CheckPlayground(PlaygroundFieldsValue[,] playground)
{
    if (_checkByPoint(playground, PlaygroundFieldsValue.Player))
    {
        return Winner.Player;
    }
    
    if (_checkByPoint(playground, PlaygroundFieldsValue.Computer))
    {
        return Winner.Computer;
    }

    foreach (PlaygroundFieldsValue cell in playground)
    {
        if (cell == PlaygroundFieldsValue.Empty)
            return Winner.None;
    }

    return Winner.Draw;
}

static void PrintWhoIsWinner(Winner winner)
{
    int top = GAME_FIELD_START_TOP + (PLAYDROUND_SIZE * 2 - 1) + 2;
    Console.SetCursorPosition(1, top);

    switch (winner)
    {
        case Winner.Player:
            Console.WriteLine("Победил игрок");
            break;
        case Winner.Computer:
            Console.WriteLine("Победил компьютер");
            break;
        case Winner.Draw:
            Console.WriteLine("Победила дружба");
            break;
    }
}


static int _getPlaceholder(int i, int j, PlaygroundFieldsValue[,] playground)
{
    int placeholder = ((i + 1) * playground.GetLength(0)) - (playground.GetLength(1) - (j + 1));
    return placeholder;
}

static int[] _getCoordinates(int placeholder, PlaygroundFieldsValue[,] playground)
{
    int[] coordinate = new int[2];

    coordinate[0] = (placeholder - 1) / playground.GetLength(0);
    coordinate[1] = (placeholder - 1) % playground.GetLength(1);

    return coordinate;
}

static int _toInt32(string inputText)
{
    inputText = string.Join("",
        inputText.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

    if (string.IsNullOrEmpty(inputText))
    {
        return -1;
    }

    int startIndex = inputText[0] == '-' || inputText[0] == '+' ? 1 : 0;

    for (int i = startIndex; i < inputText.Length; i++)
    {
        if (!char.IsDigit(inputText[i]))
        {
            return -1;
        }
    }

    return Convert.ToInt32(inputText);

}

static bool _checkByPoint(PlaygroundFieldsValue[,] playground, PlaygroundFieldsValue point)
{
    bool[] lines = new bool[playground.GetLength(0) + playground.GetLength(1) + 2];
    int linesCount = 0;
    bool check;

    // ---
    for (int i = 0; i < playground.GetLength(0); i++)
    {
        check = true;
        for (int j = 0; j < playground.GetLength(1); j++)
        {
            check &= playground[i, j] == point;
        }
        lines[linesCount++] = check;
    }
    // |||
    for (int i = 0; i < playground.GetLength(1); i++)
    {
        check = true;
        for (int j = 0; j < playground.GetLength(0); j++)
        {
            check &= playground[j, i] == point;
        }
        lines[linesCount++] = check;
    }
    // X
    for (int i = linesCount; i < lines.Length; i++)
    {
        check = true;
        if (linesCount == i)
        {
            for (int j = 0; j < PLAYDROUND_SIZE; j++)
            {
                check &= playground[j, j] == point;
            }
        }
        else
        {
            for (int j = 0; j < PLAYDROUND_SIZE; j++)
            {
                check &= playground[PLAYDROUND_SIZE - 1 - j, j] == point;
            }
        }
        lines[i] = check;
    }
    
    foreach (bool line in lines)
    {
        if (line)
        {
            return true;
        }
    }

    return false;
}

enum Winner
{
    None,
    Player,
    Computer,
    Draw
}

enum PlaygroundFieldsValue
{
    Empty,
    Player,
    Computer
}