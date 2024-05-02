using System.Text;

public class Game
{
    public byte Length {get; set;} = 10;
    public byte Width {get; set;} = 10;

    public byte Score { get; set; } = 0;
    public byte HighScore { get; set; } = 0;

    private StringBuilder sb = new StringBuilder();

    public Coordinate Point { get; set; }

    public Coordinate CurrentCoordinate = new Coordinate
    {
        X = 1,
        Y = 1
    };
    public Queue<Coordinate> Coordinates = new Queue<Coordinate>();

    public Move Direction { get; set; } = Move.RIGHT;

    private Coordinate GenerateNewPoint()
    {
        var randomizer = new Random();

        byte x, y;

        do
        {
            x = (byte)Math.Ceiling(randomizer.NextDouble() * Width);
            y = (byte)Math.Ceiling(randomizer.NextDouble() * Length);
        } while (Coordinates.Any(c => c.X == x && c.Y == y));

        var coord = new Coordinate
        {
            X = x,
            Y = y
        };

        return coord;
    }

    public bool ProcessMove(Move move)
    {
        if (Direction == Move.UP || Direction == Move.DOWN)
        {
            switch (move)
            {
                case Move.LEFT:
                    CurrentCoordinate.X--;
                    Direction = move;
                    break;
                case Move.RIGHT:
                    CurrentCoordinate.X++;
                    Direction = move;
                    break;
                default:
                    if (Direction == Move.UP)
                    {
                        CurrentCoordinate.Y--;
                    }
                    else
                    {
                        CurrentCoordinate.Y++;
                    }
                    break;
            }
        }
        else
        {
            switch (move)
            {
                case Move.UP:
                    CurrentCoordinate.Y--;
                    Direction = move;
                    break;
                case Move.DOWN:
                    CurrentCoordinate.Y++;
                    Direction = move;
                    break;
                default:
                    if (Direction == Move.LEFT)
                    {
                        CurrentCoordinate.X--;
                    }
                    else
                    {
                        CurrentCoordinate.X++;
                    }
                    break;
            }
        }
        bool IsStillIn = IsStillInGame();
        UpdateSnake();

        return IsStillIn;
    }

    private bool IsStillInGame()
    {
        if (CurrentCoordinate.X > Width || CurrentCoordinate.X < 1 || CurrentCoordinate.Y > Length || CurrentCoordinate.Y < 1)
        {
            return false;
        }

        if (Coordinates.Any(c => (c.X == CurrentCoordinate.X && c.Y == CurrentCoordinate.Y)
        || (c.X == CurrentCoordinate.X && c.Y == CurrentCoordinate.Y)))
        {
            return false;
        }

        return true;
    }

    private void UpdateSnake()
    {
        if (CurrentCoordinate.X == Point.X && CurrentCoordinate.Y == Point.Y)
        {
            Score++;
            Point = GenerateNewPoint();
        }
        else
        {
            Coordinates.TryDequeue(out Coordinate coordinate);
        }

        Coordinates.Enqueue(CurrentCoordinate);
    }

    public string DrawScreen()
    {
        if (Point.X == 0)
        {
            Point = GenerateNewPoint();
        }
        sb.Clear();
        sb.Append($"Score: {Score}\nHighscore: {HighScore}\n");

        for (byte i = 0; i < Length + 2; i++)
        {
            for (byte j = 0; j < Width + 2; j++)
            {
                if (i == 0)
                {
                    sb.Append("__");
                    continue;
                }
                if (j == 0 || j == Width + 1)
                {
                    sb.Append("|");
                    continue;
                }
                if (i == Length + 1)
                {
                    sb.Append("--");
                    continue;
                }

                if (CurrentCoordinate.X == j && CurrentCoordinate.Y == i)
                {
                    switch (Direction)
                    {
                        case Move.UP:
                            sb.Append("/\\");
                            break;
                        case Move.DOWN:
                            sb.Append("\\/");
                            break;
                        case Move.LEFT:
                            sb.Append("<#");
                            break;
                        case Move.RIGHT:
                            sb.Append("#>");
                            break;
                    }
                    continue;
                }

                if (Point.X == j && Point.Y == i)
                {
                    sb.Append("[]");
                    continue;
                }

                if (Coordinates.Any(c => c.X == j && c.Y == i))
                {
                    sb.Append("##");
                    continue;
                }

                sb.Append("  ");
                continue;
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public void Run()
    {
        bool quit = false;

        sb.Append(@"███████╗███╗   ██╗ █████╗ ██╗  ██╗███████╗
██╔════╝████╗  ██║██╔══██╗██║ ██╔╝██╔════╝
███████╗██╔██╗ ██║███████║█████╔╝ █████╗  
╚════██║██║╚██╗██║██╔══██║██╔═██╗ ██╔══╝  
███████║██║ ╚████║██║  ██║██║  ██╗███████╗
╚══════╝╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝");
        sb.AppendLine("\n\n\nPress any button to start");

        Thread.Sleep(500);


        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        System.Console.WriteLine(sb.ToString());

        while(true){
            if(Console.KeyAvailable){
                sb.Clear();
                break;
            }
        }

        Console.Clear();
        System.Console.WriteLine("What size do you want your field to be (min. 10)? \n(If you give 10, for example the field will be 10 by 10)");
        while(true){
            byte.TryParse(Console.ReadLine(), out byte d);

            if(d <= 0){
                System.Console.WriteLine("Give a valid number");
                continue;
            }else if( d < 10){
                Length = 10;
                Width = 10;
            }else if(d > 15){
                Length = 15;
                Width = 15;
            }
            else{
                Length = d;
                Width = d;
            }
            break;
        }

        while (!quit)
        {
            Console.Clear();
            Console.WriteLine(DrawScreen());

            ListenMove(out quit);
            if (quit)
            {
                HandleGameOver(ref quit);
            }

            Thread.Sleep(100);
        }
    }

    private void HandleGameOver(ref bool quit)
    {
        Console.CursorVisible = false;
        string[] options = { "YES", "NO" };
        int selectedIndex = 0;

        Console.WriteLine($"Game Over: Score {Score}");
        Console.WriteLine("Want to restart?");

        ConsoleKeyInfo key;
        do
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }
                Console.WriteLine(options[i]);
                Console.ResetColor();
            }
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % options.Length;
            }
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            Console.Write("\n\n");
            Console.SetCursorPosition(0, Console.CursorTop - 2);

        } while (key.Key != ConsoleKey.Enter);

        if (selectedIndex == 0)
        {
            Reset();
            quit = false;
        }
        else
        {
            Console.Clear();
            System.Console.WriteLine("Thank you for playing! See you later.");
            return;
        }
    }

    private void ListenMove(out bool quit)
    {
        Move Next = Direction;

        int duration = Score > 100 ? 100 : 300 - 2 * Score;
        DateTime startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds < duration)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        Next = Move.UP;
                        break;
                    case ConsoleKey.DownArrow:
                        Next = Move.DOWN;
                        break;
                    case ConsoleKey.LeftArrow:
                        Next = Move.LEFT;
                        break;
                    case ConsoleKey.RightArrow:
                        Next = Move.RIGHT;
                        break;
                }
            }
        }
        quit = !ProcessMove(Next);
    }

    private void Reset()
    {

        CurrentCoordinate = new()
        {
            X = 1,
            Y = 1
        };

        if (Score > HighScore)
        {
            HighScore = Score;
        }

        Score = 0;

        Direction = Move.RIGHT;

        Coordinates = [];

        Point = GenerateNewPoint();
    }
}