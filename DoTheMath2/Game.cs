using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Controls;


namespace DoTheMath2
{
    public class Game
    {
        #region Game board pieces, and two dimensions of one dimensional array containing them;
        private BoardPeaceModel[] BoardPieces;
        private int BoardRowLen;
        private int BoardColumnLen;
        # endregion

        #region Other Member definitions
        private LinkedList<int> SnakeBody; // Board piece numbers , which is in snake body
        private int DefaultStartPosition; // position where snake head is placed firstly in board

        public event EventHandler<EventArgs> SnakeCrashed;
        public event EventHandler<EventArgs> FoodEatten;

        public enum State { NotStarted, Running, Paused, End }
        public State GameState { get; private set; }

        private Timer _Timer;
        private Random _RandomNumber; // used to generate number for placing the piece of food  randomly in board
        private Direction _currentDirection;
        #endregion

        public Game(ItemsControl board, int rows = 22, int columns = 22, int defaultStart = 0, Direction defaulMoveDirection = Direction.Right)
        {
            DefaultStartPosition = defaultStart;
            BoardRowLen = rows;
            BoardColumnLen = columns;
            _currentDirection = defaulMoveDirection;
            _RandomNumber = new Random(System.DateTime.Now.Millisecond);
            SnakeBody = new LinkedList<int>();
            BoardPieces = new BoardPeaceModel[rows * columns];


            for (int i = 0; i < BoardPieces.Length; i++)
            {
                BoardPieces[i] = new BoardPeaceModel(Piece.Free, i);
            }

            SnakeBody.AddFirst(new LinkedListNode<int>(defaultStart)); // add Head
            BoardPieces[defaultStart].ChangeType(Piece.Head);

            board.ItemsSource = BoardPieces;
        }

        public void ChangeDirection(Direction value)
        {
            Direction opposite = Direction.Left;
            switch (value)
            {
                case Direction.Up:
                    opposite = Direction.Down;
                    break;
                case Direction.Down:
                    opposite = Direction.Up;
                    break;
                case Direction.Left:
                    opposite = Direction.Right;
                    break;
                case Direction.Right:
                    opposite = Direction.Left;
                    break;
            }

            // must not move snake backwards except when it is only snake head
            if (opposite != _currentDirection || SnakeBody.Count == 1)
            {
                _currentDirection = value;
            }
        }

        public void ChangeSpeed(int value)
        {
            if (value > 30 && value < 1000)
            {
                _Timer.Interval = value;
            }
        }

        private void DropFood()
        {
            // construct array with free pieces  in board
            List<int> available = new List<int>();
            for (int i = 0; i < BoardPieces.Length; i++)
            {
                if (BoardPieces[i].Type == Piece.Free)
                {
                    available.Add(i);
                }
            }
            // drop randomly selected piece
            BoardPieces[_RandomNumber.Next(0, available.Count)].ChangeType(Piece.Food);
        }

        public void Resume()
        {
            if (GameState == State.Paused)
            {
                GameState = State.Running;
            }
        }

        public void Pause()
        {
            if (GameState == State.Running)
            {
                GameState = State.Paused;
            }
        }

        public void StartPlay(Direction dir)
        {
            if (GameState == State.NotStarted)
            {
                _currentDirection = dir;
                GameState = State.Running;
                if (_Timer == null) // When Game is restarted , Timer Must Not Be Reinitialized ! Because Old one olso will tick
                {
                    _Timer = new Timer(200);
                    _Timer.Elapsed += (object Sender, ElapsedEventArgs e) =>
                    {
                        if (GameState == State.Running)
                        {
                            moveSnake();
                        }
                    };
                    _Timer.Start();
                    DropFood();
                }
            }
        }

        public void Reset()
        {
            GameState = State.NotStarted;

            // Free all peaces
            for (int i = 0; i < BoardPieces.Length; i++)
            {
                BoardPieces[i].ChangeType(Piece.Free);
            }
            // Reset Snake Head
            SnakeBody.Clear();
            SnakeBody.AddFirst(new LinkedListNode<int>(DefaultStartPosition)); // add first peace
            BoardPieces[DefaultStartPosition].ChangeType(Piece.Head);
            DropFood();

        }

        private void OnFoodEatten()
        {
            DropFood();
            if (FoodEatten != null)
            {
                this.FoodEatten(this, EventArgs.Empty);
            }
        }

        private void OnSnakeCrashed()
        {
            this.GameState = State.End;
            if (SnakeCrashed != null)
            {
                this.SnakeCrashed(this, EventArgs.Empty);
            }
        }

        public void Move()
        {
            if (GameState == State.Running)
            {
                moveSnake();
            }
        }

        private void moveSnake()
        {
            int snakeHead = SnakeBody.First.Value;
            int row = snakeHead / BoardRowLen, col = snakeHead % BoardRowLen;

            // make Snake Head to snake body part
            BoardPieces[snakeHead].ChangeType(Piece.Body);

            if (_currentDirection == Direction.Left)
            {
                int next = snakeHead - 1;
                if (col - 1 >= 0)
                {
                    if (BoardPieces[next].Type == Piece.Free)
                    {
                        //make snake last part free 
                        BoardPieces[SnakeBody.Last.Value].ChangeType(Piece.Free);
                        SnakeBody.RemoveLast();
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                    }
                    else if (BoardPieces[next].Type == Piece.Food)
                    {
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                        OnFoodEatten();
                    }
                    else // wall 
                    {
                        OnSnakeCrashed();
                    }
                }
                else
                {
                    OnSnakeCrashed();
                }
            }
            else if (_currentDirection == Direction.Right)
            {
                int next = snakeHead + 1;
                if (col + 1 < BoardColumnLen)
                {
                    if (BoardPieces[next].Type == Piece.Free)
                    {
                        //make snake last part free 
                        BoardPieces[SnakeBody.Last.Value].ChangeType(Piece.Free);
                        SnakeBody.RemoveLast();
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                    }
                    else if (BoardPieces[next].Type == Piece.Food)
                    {
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                        OnFoodEatten();
                    }
                    else // wall 
                    {
                        OnSnakeCrashed();
                    }
                }
                else
                {
                    OnSnakeCrashed();
                }
            }
            else if (_currentDirection == Direction.Up)
            {
                int next = snakeHead - BoardColumnLen;
                if (row - 1 >= 0)
                {
                    if (BoardPieces[next].Type == Piece.Free)
                    {
                        //make snake last part free 
                        BoardPieces[SnakeBody.Last.Value].ChangeType(Piece.Free);
                        SnakeBody.RemoveLast();
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                    }
                    else if (BoardPieces[next].Type == Piece.Food)
                    {
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                        OnFoodEatten();
                    }
                    else // wall 
                    {
                        OnSnakeCrashed();
                    }
                }
                else
                {
                    OnSnakeCrashed();
                }
            }
            else if (_currentDirection == Direction.Down)
            {
                int next = snakeHead + BoardColumnLen;
                if (row + 1 < BoardRowLen)
                {
                    if (BoardPieces[next].Type == Piece.Free)
                    {
                        //make snake last part free 
                        BoardPieces[SnakeBody.Last.Value].ChangeType(Piece.Free);
                        SnakeBody.RemoveLast();
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                    }
                    else if (BoardPieces[next].Type == Piece.Food)
                    {
                        //add head
                        BoardPieces[next].ChangeType(Piece.Head);
                        SnakeBody.AddFirst(next);
                        OnFoodEatten();
                    }
                    else // wall 
                    {
                        OnSnakeCrashed();
                    }
                }
                else
                {
                    OnSnakeCrashed();
                }
            }
        }

        ~Game()
        {
            if (_Timer != null)
                _Timer.Dispose();
        }

    }
}
