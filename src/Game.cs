using System.Text;

public class Game
{
    public byte Score { get; set; } = 0;
    public byte HighScore { get; set; } = 0;

    private StringBuilder sb = new StringBuilder(180);

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
            x = (byte)Math.Ceiling(randomizer.NextDouble() * 10);
            y = (byte)Math.Ceiling(randomizer.NextDouble() * 10);
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
        bool IsStillIn = this.IsStillInGame();
        UpdateSnake();

        return IsStillIn;
    }

    private bool IsStillInGame()
    {
        if (CurrentCoordinate.X > 10 || CurrentCoordinate.X < 1 || CurrentCoordinate.Y > 10 || CurrentCoordinate.Y < 1)
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
        sb = new StringBuilder(200);
        sb.Append($"Score: {Score}\nHighscore: {HighScore}\n");
        char nextChar = '^';

        for (byte i = 0; i < 12; i++)
        {
            for (byte j = 0; j < 12; j++)
            {
                if (i == 0)
                {
                    nextChar = '_';
                    sb.Append(nextChar);
                    continue;
                }
                if (j == 0 || j == 11)
                {
                    nextChar = '|';
                    sb.Append(nextChar);
                    continue;
                }
                if (i == 11)
                {
                    nextChar = '-';
                    sb.Append(nextChar);
                    continue;
                }

                if (CurrentCoordinate.X == j && CurrentCoordinate.Y == i)
                {
                    switch (Direction)
                    {
                        case Move.UP:
                            nextChar = '^';
                            break;
                        case Move.DOWN:
                            nextChar = 'v';
                            break;
                        case Move.LEFT:
                            nextChar = '<';
                            break;
                        case Move.RIGHT:
                            nextChar = '>';
                            break;
                    }
                    sb.Append(nextChar);
                    continue;
                }

                if (Point.X == j && Point.Y == i)
                {
                    nextChar = '@';
                    sb.Append(nextChar);
                    continue;
                }

                if (Coordinates.Any(c => c.X == j && c.Y == i))
                {
                    nextChar = '#';
                    sb.Append(nextChar);
                    continue;
                }

                nextChar = ' ';
                sb.Append(nextChar);
                continue;
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public void Run()
    {
        bool quit = false;

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        while (!quit)
        {
            Console.Clear();
            System.Console.WriteLine(DrawScreen());
            Move Next = Direction;

            int duration = Score > 50 ? 100 : 250 - 3 * Score;
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
            if (quit)
            {
                System.Console.WriteLine($"Game Over: Score {Score}");
                System.Console.WriteLine("Press R to restart or anything else to quit");
                while(quit){
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    switch(keyInfo.Key){
                        case ConsoleKey.R:
                            Clear();
                            quit = false;
                            break;
                        default:
                            return;
                    }

                }
            }
        }
    }

    private void Clear()
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