using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class SnakeGame
{
    static int screenWidth = 50;
    static int screenHeight = 20;
    static int score;
    static int foodX, foodY;
    static int snakeX, snakeY;
    static List<Tuple<int, int>> snakeBody;
    static string direction = "RIGHT";
    static bool gameOver;

    static void Main()
    {
        Console.CursorVisible = false;
        Console.SetWindowSize(screenWidth, screenHeight);
        Console.SetBufferSize(screenWidth, screenHeight);

        InitializeGame();

        while (!gameOver)
        {
            Draw();
            Input();
            Logic();
            Thread.Sleep(100);
        }

        Console.SetCursorPosition(0, screenHeight - 1);
        Console.WriteLine($"Game Over! Your score is {score}");
        Console.ReadKey();
    }

    static void InitializeGame()
    {
        snakeX = screenWidth / 4;
        snakeY = screenHeight / 2;
        snakeBody = new List<Tuple<int, int>> { Tuple.Create(snakeX, snakeY) };
        score = 0;
        gameOver = false;
        GenerateFood();
    }

    static void Draw()
    {
        Console.Clear();

        // Draw Snake
        foreach (var bodyPart in snakeBody)
        {
            Console.SetCursorPosition(bodyPart.Item1, bodyPart.Item2);
            Console.Write("■");
        }

        // Draw Food
        Console.SetCursorPosition(foodX, foodY);
        Console.Write("●");

        // Draw Border
        for (int i = 0; i < screenWidth; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("■");
            Console.SetCursorPosition(i, screenHeight - 1);
            Console.Write("■");
        }

        for (int i = 0; i < screenHeight; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("■");
            Console.SetCursorPosition(screenWidth - 1, i);
            Console.Write("■");
        }

        // Draw Score
        Console.SetCursorPosition(2, screenHeight - 2);
        Console.Write($"Score: {score}");
    }

    static void Input()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow && direction != "DOWN") direction = "UP";
            else if (key == ConsoleKey.DownArrow && direction != "UP") direction = "DOWN";
            else if (key == ConsoleKey.LeftArrow && direction != "RIGHT") direction = "LEFT";
            else if (key == ConsoleKey.RightArrow && direction != "LEFT") direction = "RIGHT";
        }
    }

    static void Logic()
    {
        int prevX = snakeBody.First().Item1;
        int prevY = snakeBody.First().Item2;
        int prev2X, prev2Y;
        Tuple<int, int> tail;

        if (direction == "UP") snakeY--;
        if (direction == "DOWN") snakeY++;
        if (direction == "LEFT") snakeX--;
        if (direction == "RIGHT") snakeX++;

        // Game Over if snake hits the wall
        if (snakeX <= 0 || snakeX >= screenWidth - 1 || snakeY <= 0 || snakeY >= screenHeight - 1)
            gameOver = true;

        // Check if snake collides with itself
        foreach (var bodyPart in snakeBody.Skip(1))
        {
            if (bodyPart.Item1 == snakeX && bodyPart.Item2 == snakeY)
                gameOver = true;
        }

        // Add new head to snake body
        snakeBody.Insert(0, Tuple.Create(snakeX, snakeY));

        // Check if snake eats food
        if (snakeX == foodX && snakeY == foodY)
        {
            score += 10;
            GenerateFood();
        }
        else
        {
            // Remove tail
            tail = snakeBody.Last();
            snakeBody.RemoveAt(snakeBody.Count - 1);
        }
    }

    static void GenerateFood()
    {
        Random random = new Random();
        foodX = random.Next(1, screenWidth - 1);
        foodY = random.Next(1, screenHeight - 1);
    }
}
 