// Developed by AceInfinity (c) Tech.Reboot.Pro 2013
// Reversi Game
//
// OBJECTIVE:
// The objective of this game is to own more pieces than your opponent by the time the game is finished. 
// The game is over when the board is fully filled in, or neither player can make a move. 

// RULES:
// Each reversi piece has a red side and a blue side. Player1 will be red, and Player2 will be blue here.
// You must place the piece so that an opponent's piece is sandwiched between 2 of your pieces including the
// piece you just played for that turn. All of the opponent's pieces between that newly placed piece and the
// pre-existing piece of your own, become your own pieces and change to your Player's color. 
//
// You can only place a piece on an empty tile. If you cannot make a legal move, you have to pass your turn,
// and allow your opponent to make a move instead. You can capture vertical, horizontal, and diagonal paths of pieces.
// You can also, capture more than one path of your opponents pieces at once with a good move. 

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Reversi
{
    class ReversiBoard : PictureBox
    {
        #region // Constructors
        public ReversiBoard() : this(new ReversiLayout(8, 8)) { }

        public ReversiBoard(ReversiLayout layout)
        {
            _layout = layout;

            // Default Initialization
            _borderColor = Color.FromArgb(255, 50, 50, 50);
            _tileColor = Color.FromArgb(255, 25, 25, 25);

            _player1Color = Color.FromArgb(255, 255, 25, 25); // Red
            _player2Color = Color.FromArgb(255, 25, 150, 255); // Blue

            _markers = new ReversiPlayer[_layout.Columns, _layout.Rows];

            this.Size = new Size(_layout.Columns * _tileSize, _layout.Rows * _tileSize + _paddingBottom);
            _faintCollection.ListChanged += new ListChangedEventHandler((sender, e) => this.Invalidate());
            NewGame();
        }
        #endregion

        #region // Fields & Properties
        private ReversiLayout _layout; // Stores board layout information
        private Coordinates _lastCoordinates; // Keeps track of last coordinate for efficient checking
        private const int _tileSize = 25; // Constant tile size for game board
        private const int _paddingBottom = 15; // Constant for extra information at bottom (score)
        private ReversiPlayer[,] _markers; // Player markers for game board
        private BindingList<Marker> _faintCollection = new BindingList<Marker>(); // For hover image on valid locations

        private ReversiPlayer _currentPlayer = ReversiPlayer.Player1; // Current player to take his/her turn
        private Coordinates _targetCoordinates; // Represents the target coordinate for the flips
        private int _player1MarkerCount; // Represents the target coordinate for the flips
        private int _player2MarkerCount; // Represents the target coordinate for the flips

        /// <summary>
        /// Game tiles border color.
        /// </summary>
        private Color _borderColor;
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// Game tiles back color.
        /// </summary>
        private Color _tileColor;
        public Color TileColor
        {
            get { return _tileColor; }
            set { _tileColor = value; }
        }

        /// <summary>
        /// Player1's marker color.
        /// </summary>
        private Color _player1Color;
        public Color Player1Color
        {
            get { return _player1Color; }
            set { _player1Color = value; }
        }

        /// <summary>
        /// Player2's marker color.
        /// </summary>
        private Color _player2Color;
        public Color Player2Color
        {
            get { return _player2Color; }
            set { _player2Color = value; }
        }

        public bool GameFinished
        {
            get
            {
                bool boardComplete = true;
                for (int i = 0; i < _layout.Columns; i++)
                {
                    for (int j = 0; j < _layout.Rows; j++)
                    {
                        if (_markers[i, j] == ReversiPlayer.None)
                        {
                            goto BoardNotComplete;
                        }
                    }
                }
                if (boardComplete)
                {
                    return true;
                }
            BoardNotComplete:
                // Board was found incomplete so let's check if any moves can still be made
                //...

                return false;
            }
        }
        #endregion

        #region // Overrides
        /// <summary>
        /// Paints the board visual with all the player markers and current player indicator.
        /// </summary>
        /// <param name="pe">PaintEventArgs.</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint the grid
            SolidBrush sb = new SolidBrush(_borderColor);
            Pen p = new Pen(sb);

            HatchBrush hb1 = new HatchBrush(HatchStyle.SmallCheckerBoard, _tileColor);
            HatchBrush hb2 = new HatchBrush(HatchStyle.SmallCheckerBoard, Color.FromArgb(245, _tileColor));

            int x = 0;
            bool evenRows = _layout.Rows % 2 == 0;

            for (int i = 0; i < _layout.Columns; i++)
            {
                for (int j = 0; j < _layout.Rows; j++, x++)
                {
                    Rectangle rect = new Rectangle(i * _tileSize, j * _tileSize, _tileSize, _tileSize);
                    g.FillRectangle(x % 2 == 0 ? hb1 : hb2, rect);
                    g.DrawRectangle(p, rect);
                }

                if (evenRows)
                {
                    x++;
                }
            }

            hb1.Dispose();
            hb2.Dispose();

            // Paint Markers
            sb.Color = _player1Color;
            p.Color = sb.Color;

            for (int i = 0; i <= _markers.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _markers.GetUpperBound(1); j++)
                {
                    if (_markers[i, j] != ReversiPlayer.None)
                    {
                        Color color = _markers[i, j] == ReversiPlayer.Player1 ? _player1Color : _player2Color;
                        p.Color = color;
                        Rectangle rect = new Rectangle(i * _tileSize + 1, j * _tileSize + 1, _tileSize - 2, _tileSize - 2);
                        g.DrawEllipse(p, rect);

                        sb.Color = Color.FromArgb(100, color);
                        g.FillEllipse(sb, rect);
                    }
                }
            }

            foreach (Marker m in _faintCollection)
            {
                Color color = Color.FromArgb(50, m.Player == ReversiPlayer.Player1 ? _player1Color : _player2Color);
                p.Color = color;
                Rectangle rect = new Rectangle(m.TileCoordinates.X * _tileSize + 1, m.TileCoordinates.Y * _tileSize + 1, _tileSize - 2, _tileSize - 2);
                g.DrawEllipse(p, rect);

                sb.Color = Color.FromArgb(25, color);
                g.FillEllipse(sb, rect);
            }

            p.Dispose();

            // Paint current player indicator
            using (Font font = new Font("Arial", 7.5f, FontStyle.Regular))
            {
                sb.Color = Color.FromArgb(200, 255, 255, 255);
                g.DrawString(string.Format("{0}'s Turn", _currentPlayer), font, sb, new PointF(2, 2));
                sb.Color = Color.Black;
                g.DrawString(string.Format("Player1: {0} | Player2: {1}", _player1MarkerCount, _player2MarkerCount), font, sb, new PointF(2, this.Height - 14));
            }

            sb.Dispose();
        }

        /// <summary>
        /// Makes sure that we cannot resize the board once it has been initialized.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Size = new Size(_layout.Columns * _tileSize, _layout.Rows * _tileSize + _paddingBottom);
        }

        /// <summary>
        /// For clicking on the game board and adding a marker to a location on the grid.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            // Check release boundaries
            Coordinates coordinates = new Coordinates(e.X / _tileSize, e.Y / _tileSize);
            if (IsValidLocation(coordinates))
            {
                _markers[coordinates.X, coordinates.Y] = _currentPlayer;
                FlipOpponentMarkers(coordinates);
                _faintCollection.Clear();
                UpdateMarkerCount();

                if (GameFinished)
                {
                    GameOver();
                }
            }
        }

        /// <summary>
        /// Calculates move validity and shows faint marker indicator for valid tiles
        /// to place markers on.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.None)
            {
                Coordinates coordinates = new Coordinates(e.X / _tileSize, e.Y / _tileSize);
                if (!_lastCoordinates.Equals(coordinates))
                {
                    if (IsValidLocation(coordinates))
                    {
                        _faintCollection.Add(new Marker(_currentPlayer, coordinates));
                        if (_faintCollection.Count > 1)
                        {
                            _faintCollection.RemoveAt(0);
                        }
                    }
                    _lastCoordinates = coordinates;
                }
            }
        }

        /// <summary>
        /// Gets rid of the faint marker indicator if mouse leaves the control.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _faintCollection.Clear();
        }
        #endregion

        #region // Methods
        /// <summary>
        /// Initialize and start a new game.
        /// </summary>
        private void NewGame()
        {
            for (int i = 0; i <= _markers.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _markers.GetUpperBound(1); j++)
                {
                    _markers[i, j] = ReversiPlayer.None;
                }
            }

            int xPos = _markers.GetUpperBound(0) / 2;
            int yPos = _markers.GetUpperBound(1) / 2;

            _markers[xPos++, yPos] = ReversiPlayer.Player1;
            _markers[xPos, yPos++] = ReversiPlayer.Player2;
            _markers[xPos--, yPos] = ReversiPlayer.Player1;
            _markers[xPos, yPos] = ReversiPlayer.Player2;
            this.Invalidate();
        }

        /// <summary>
        /// Game was finished: Display winner and reset the game.
        /// </summary>
        private void GameOver()
        {
            string msg = string.Empty;
            if (_player1MarkerCount > _player2MarkerCount)
            {
                msg = string.Format("Player1 won with {0} markers!", _player1MarkerCount);
            }
            else
            {
                msg = string.Format("Player2 won with {0} markers!", _player2MarkerCount);
            }
            MessageBox.Show(msg, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            NewGame();
        }

        /// <summary>
        /// Updates the marker count for both players.
        /// </summary>
        private void UpdateMarkerCount()
        {
            _player1MarkerCount = _player2MarkerCount = 0;
            for (int i = 0; i <= _markers.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _markers.GetUpperBound(1); j++)
                {
                    if (_markers[i, j] == ReversiPlayer.Player1)
                    {
                        _player1MarkerCount++;
                    }
                    else if (_markers[i, j] == ReversiPlayer.Player2)
                    {
                        _player2MarkerCount++;
                    }
                }
            }
            this.Invalidate();
        }

        /// <summary>
        /// Determining whether a move from the current coordinate in a specific direction is possible.
        /// </summary>
        /// <param name="coordinates">Current coordinates to check.</param>
        /// <param name="direction">Current direction from coordinates to check.</param>
        /// <returns>Whether the marker position is a possible move.</returns>
        private bool PossibleMove(Coordinates coordinates, ReversiPlayer player, Direction direction)
        {
            bool valid = false;
            int x = coordinates.X;
            int y = coordinates.Y;

            switch (direction)
            {
                case Direction.Up:
                    y--;
                    while (y > 0 && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        y--;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
                case Direction.Down:
                    y++;
                    while (y < _markers.GetUpperBound(1) && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        y++;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
                case Direction.Left:
                    x--;
                    while (x > 0 && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        x--;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
                case Direction.Right:
                    x++;
                    while (x < _markers.GetUpperBound(0) && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        x++;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
                case Direction.DiagonalRightUp:
                    x++;
                    y--;
                    while ((x < _markers.GetUpperBound(0) && y > 0) && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        x++;
                        y--;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
                case Direction.DiagonalRightDown:
                    x++;
                    y++;
                    while ((x < _markers.GetUpperBound(0) && y < _markers.GetUpperBound(1)) && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        x++;
                        y++;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
                case Direction.DiagonalLeftUp:
                    x--;
                    y--;
                    while ((x > 0 && y > 0) && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        x--;
                        y--;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
                case Direction.DiagonalLeftDown:
                    x--;
                    y++;
                    while ((x > 0 && y < _markers.GetUpperBound(1) && y > 0) && !valid)
                    {
                        if (_markers[x, y] == ReversiPlayer.None) { goto FoundEmpty; }
                        x--;
                        y++;
                        if (_markers[x, y] == _currentPlayer) { valid = true; }
                    }
                    break;
            }

            if (valid)
            {
                _targetCoordinates = new Coordinates(x, y);
            }

        FoundEmpty:
            return valid;
        }

        /// <summary>
        /// Checks if current tile is a valid move for the current player.
        /// </summary>
        /// <param name="coordinates">Coordinates that indicate the current tile.</param>
        /// <returns>Valid move or not.</returns>
        private bool IsValidLocation(Coordinates coordinates)
        {
            if ((coordinates.X >= 0 && coordinates.X <= _markers.GetUpperBound(0)) &&
                (coordinates.Y >= 0 && coordinates.Y <= _markers.GetUpperBound(1)))
            {
                // Check if marker already exists
                if (_markers[coordinates.X, coordinates.Y] != ReversiPlayer.None)
                {
                    return false;
                }

                bool validLocation = false;
                ReversiPlayer opponent = _currentPlayer ^ (ReversiPlayer)3;

                // Check 3 spots to the right of tile
                if (coordinates.X < _markers.GetUpperBound(0))
                {
                    if (coordinates.Y > 0 && _markers[coordinates.X + 1, coordinates.Y - 1] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalRightUp))
                        {
                            validLocation = true;
                        }
                    }
                    if (coordinates.Y < _markers.GetUpperBound(1) && _markers[coordinates.X + 1, coordinates.Y + 1] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalRightDown))
                        {
                            validLocation = true;
                        }
                    }
                    if (_markers[coordinates.X + 1, coordinates.Y] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.Right))
                        {
                            validLocation = true;
                        }
                    }

                    if (validLocation)
                    {
                        goto FoundValidMove;
                    }
                }

                // Check 3 spots to the left of tile
                if (coordinates.X > 0)
                {
                    if (coordinates.Y > 0 && _markers[coordinates.X - 1, coordinates.Y - 1] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalLeftUp))
                        {
                            validLocation = true;
                        }
                    }
                    if (coordinates.Y < _markers.GetUpperBound(1) && _markers[coordinates.X - 1, coordinates.Y + 1] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalLeftDown))
                        {
                            validLocation = true;
                        }
                    }
                    if (_markers[coordinates.X - 1, coordinates.Y] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.Left))
                        {
                            validLocation = true;
                        }
                    }

                    if (validLocation)
                    {
                        goto FoundValidMove;
                    }
                }

                // Check 1 spot below the tile
                if (coordinates.Y < _markers.GetUpperBound(1))
                {
                    if (_markers[coordinates.X, coordinates.Y + 1] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.Down))
                        {
                            validLocation = true;
                        }
                    }

                    if (validLocation)
                    {
                        goto FoundValidMove;
                    }
                }

                // Check 1 spot above the tile
                if (coordinates.Y > 0)
                {
                    if (_markers[coordinates.X, coordinates.Y - 1] == opponent)
                    {
                        if (PossibleMove(coordinates, _currentPlayer, Direction.Up))
                        {
                            validLocation = true;
                        }
                    }
                }

            FoundValidMove:
                return validLocation;
            }
            return false;
        }

        /// <summary>
        /// Flip opponent markers from current move at specified coordinates.
        /// </summary>
        /// <param name="coordinates">Coordinates of the newly placed marker.</param>
        private void FlipOpponentMarkers(Coordinates coordinates)
        {
            if (PossibleMove(coordinates, _currentPlayer, Direction.Up))
            {
                for (int i = coordinates.Y; i > _targetCoordinates.Y; i--)
                {
                    _markers[coordinates.X, i] = _currentPlayer;
                }
            }

            if (PossibleMove(coordinates, _currentPlayer, Direction.Down))
            {
                for (int i = coordinates.Y; i < _targetCoordinates.Y; i++)
                {
                    _markers[coordinates.X, i] = _currentPlayer;
                }
            }

            if (PossibleMove(coordinates, _currentPlayer, Direction.Left))
            {
                for (int i = coordinates.X; i > _targetCoordinates.X; i--)
                {
                    _markers[i, coordinates.Y] = _currentPlayer;
                }
            }

            if (PossibleMove(coordinates, _currentPlayer, Direction.Right))
            {
                for (int i = coordinates.X; i < _targetCoordinates.X; i++)
                {
                    _markers[i, coordinates.Y] = _currentPlayer;
                }
            }

            if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalRightUp))
            {
                int j = coordinates.Y;
                for (int i = coordinates.X; i < _targetCoordinates.X; i++, j--)
                {
                    _markers[i, j] = _currentPlayer;
                }
            }

            if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalRightDown))
            {
                int j = coordinates.Y;
                for (int i = coordinates.X; i < _targetCoordinates.X; i++, j++)
                {
                    _markers[i, j] = _currentPlayer;
                }
            }

            if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalLeftUp))
            {
                int j = coordinates.Y;
                for (int i = coordinates.X; i > _targetCoordinates.X; i--, j--)
                {
                    _markers[i, j] = _currentPlayer;
                }
            }

            if (PossibleMove(coordinates, _currentPlayer, Direction.DiagonalLeftDown))
            {
                int j = coordinates.Y;
                for (int i = coordinates.X; i > _targetCoordinates.X; i--, j++)
                {
                    _markers[i, j] = _currentPlayer;
                }
            }

            _currentPlayer ^= (ReversiPlayer)3;
            this.Invalidate();
        }
        #endregion
    }

    /// <summary>
    /// Determines the layout for the game grid/board.
    /// </summary>
    struct ReversiLayout
    {
        public ReversiLayout(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
        }

        public readonly int Rows;
        public readonly int Columns;
    }

    /// <summary>
    /// Determines the current player and occupied or vacant tile piece.
    /// </summary>
    enum ReversiPlayer : short
    {
        None = 0,
        Player1 = 1,
        Player2 = 2
    }

    /// <summary>
    /// Determines the direction to look in for a valid move.
    /// </summary>
    enum Direction : short
    {
        Up,
        Down,
        Left,
        Right,
        DiagonalRightUp,
        DiagonalRightDown,
        DiagonalLeftUp,
        DiagonalLeftDown
    }

    /// <summary>
    /// Stores relative tile coordinates on game board.
    /// </summary>
    struct Coordinates
    {
        public Coordinates(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public readonly int X;
        public readonly int Y;
    }

    /// <summary>
    /// Data for game pieces that are displayed on the board.
    /// </summary>
    class Marker
    {
        // Fields & Properties
        public ReversiPlayer Player;
        public Coordinates TileCoordinates;

        // Constructor
        public Marker(ReversiPlayer player, Coordinates location)
        {
            this.Player = player;
            this.TileCoordinates = location;
        }
    }
}