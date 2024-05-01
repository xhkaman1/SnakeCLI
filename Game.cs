using System.ComponentModel;

public class Game{
    public int Score {get; set;} = 0;

    public Coordinate Point {get; set;}  

    public Coordinate CurrentCoordinate = new Coordinate{
        X = 1, Y = 1 
    };
    public Queue<Coordinate> Coordinates = new Queue<Coordinate>();

    public Move Direction {get; set;} = Move.RIGHT;

    private Coordinate GenerateNewPoint(){
        var randomizer = new Random();
        
        int x,y;
        
        do
        {
            x = (int)Math.Ceiling(randomizer.NextDouble() * 10);
            y = (int)Math.Ceiling(randomizer.NextDouble() * 10);
        } while (Coordinates.Any(c => c.X == x && c.Y == y));
        {

        }

        var coord = new Coordinate{
            X = x,
            Y = y
        };

        return coord;
    }

    public bool ProcessMove(Move move){
        if(Direction == Move.UP || Direction == Move.DOWN){
            switch(move){
                case Move.LEFT:
                    CurrentCoordinate.X--;
                    Direction = move;
                    break;
                case Move.RIGHT:
                    CurrentCoordinate.X++;
                    Direction = move;
                    break;   
                default:
                    if(Direction == Move.UP){
                        CurrentCoordinate.Y--;
                    }else{
                        CurrentCoordinate.Y++;
                    }
                    break;
            }
        }else{
            switch(move){
                case Move.UP:
                    CurrentCoordinate.Y--;
                    Direction = move;
                    break;
                case Move.DOWN:
                    CurrentCoordinate.Y++;
                    Direction = move;
                    break;   
                default:
                    if(Direction == Move.LEFT){
                        CurrentCoordinate.X--;
                    }else{
                        CurrentCoordinate.X++;
                    }
                    break;
            }
        }
        bool IsStillInGame = this.IsStillInGame();
        UpdateSnake();

        return IsStillInGame;
    }

    private bool IsStillInGame(){
        if(CurrentCoordinate.X > 10 || CurrentCoordinate.X < 1 || CurrentCoordinate.Y > 10 || CurrentCoordinate.Y < 1){
            return false;
        }
        
        if(Coordinates.Where(x => x.X == CurrentCoordinate.X && x.Y == CurrentCoordinate.Y).Count() > 1){
            return false; 
        }

        if(Coordinates.Any(c => c.X == CurrentCoordinate.X && c.Y == CurrentCoordinate.Y))
        {
            return false;
        }

        return true;
    }

    private void UpdateSnake(){
        if(CurrentCoordinate.X == Point.X && CurrentCoordinate.Y == Point.Y){
            Score++;
            Point = GenerateNewPoint();
        }else{
            Coordinates.TryDequeue(out Coordinate coordinate);
        }

        Coordinates.Enqueue(CurrentCoordinate);
    }

    public string[] DrawScreen(){        
        if(Point.X == 0){
            Point = GenerateNewPoint();
        }
        string[] screen = new string[12];

        for(int i = 0; i < 12; i++){
            var line = "";
            for (int j = 0; j < 12; j++)
            {
                var nextChar = "";
                if(i == 0 || j == 0 || i == 11 || j == 11){
                    nextChar = "-";
                    line = $"{line}{nextChar}";
                    continue;
                }

                if(CurrentCoordinate.X == j && CurrentCoordinate.Y == i){
                    nextChar = "0";
                    line = $"{line}{nextChar}";
                    continue;
                }

                if(Point.X == j && Point.Y == i){
                    nextChar = "+";
                    line = $"{line}{nextChar}";
                    continue;
                }

                foreach (var coordinate in Coordinates)
                {
                    if (coordinate.X == j && coordinate.Y == i)
                    {
                        nextChar = "o";
                        line = $"{line}{nextChar}";
                        continue;
                    }
                }

                nextChar = " ";
                line = $"{line}{nextChar}";
                continue;
            }
            screen[i] = line;
        }

        return screen;
    }
}