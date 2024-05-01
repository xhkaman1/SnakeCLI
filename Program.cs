Game game = new Game();

Console.WriteLine("Game Started");

while(true){
    foreach (var line in game.DrawScreen())
    {
        System.Console.WriteLine(line);   
    }   
    System.Console.WriteLine("Select Next Move: u,d,l,r");
    var NextMove = Console.ReadLine()!.Trim().ToLower();
    Move Next = Move.UP;
    switch(NextMove){
        case "u":
            Next = Move.UP;
            break;
        case "d":
            Next = Move.DOWN;
            break;
        case "l":
            Next = Move.LEFT;
            break;
        case "r":
            Next = Move.RIGHT;
            break;
    }
    if(!game.ProcessMove(Next)){
        System.Console.WriteLine($"Game Over: Score {game.Score}");
        System.Console.WriteLine("Press any button to quit");
        Console.ReadLine();
        return;
    }
}