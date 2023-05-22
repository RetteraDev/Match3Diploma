using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class GameGrid : MonoBehaviour
    {
        [System.Serializable]
        public struct PiecePrefab
        {
            public PieceType type;
            public GameObject prefab;
        };

        [System.Serializable]
        public struct PiecePosition
        {
            public PieceType type;
            public int x;
            public int y;

            public PiecePosition(PieceType _type, int _x, int _y)
            {
                type = _type;
                x = _x;
                y = _y;
            }
        };

        [System.Serializable]
        public class Map
        {
            public string type;
            public int width;
            public int height;
            public string[] _map;
            public string[] _mapObst;
            public string moves;
            public string time;
            public string score1Star;
            public string score2Star;
            public string score3Star;
        };

        public List<List<int>> _colorMap = new List<List<int>>();
        public List<List<int>> _obstMap = new List<List<int>>();
        public TextAsset jsonFile;
        public Map _mapInfo = new Map();

        public int xDim;
        public int yDim;
        public float fillTime;

        public Level level;
        public Hud hud;

        public PiecePrefab[] piecePrefabs;
        public GameObject backgroundPrefab;

        public List<PiecePosition> initialPieces;

        private Dictionary<PieceType, GameObject> _piecePrefabDict;

        public GamePiece[,] _pieces;

        private bool _inverse;

        private GamePiece _pressedPiece;
        private GamePiece _enteredPiece;

        private bool _gameOver;

        public bool IsFilling { get; private set; }

        private void ParseJSON() => _mapInfo = JsonUtility.FromJson<Map>(jsonFile.text);

        private void Awake()
        {
            ParseJSON();
            FillMapList(_colorMap, _mapInfo._map);
            FillMapList(_obstMap, _mapInfo._mapObst);
            ChangeComponents();

            xDim = _mapInfo.width;
            yDim = _mapInfo.height;
            
            level.score1Star = Convert.ToInt32(_mapInfo.score1Star);
            level.score2Star = Convert.ToInt32(_mapInfo.score2Star);
            level.score3Star = Convert.ToInt32(_mapInfo.score3Star);
            level.numMoves = Convert.ToInt32(_mapInfo.moves);
            level.timeInSeconds = Convert.ToInt32(_mapInfo.time);

            for (int i = 0; i < _mapInfo.height; i++)
            {
                for (int j = 0; j < _mapInfo.width; j++)
                {
                    if(_obstMap[i][j] == 1) initialPieces.Add(new PiecePosition(PieceType.Frozen, j, i));
                    if(_obstMap[i][j] == 2) initialPieces.Add(new PiecePosition(PieceType.Brick, j, i));
                }
            }
            
            _piecePrefabDict = new Dictionary<PieceType, GameObject>();
            for (int i = 0; i < piecePrefabs.Length; i++)
            {
                if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
                {
                    _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
                }
            }

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                    background.transform.parent = transform;
                }
            }

            _pieces = new GamePiece[xDim, yDim];

            if(initialPieces.Count > 0)
            {
                for (int i = 0; i < initialPieces.Count; i++)
                {
                    if (initialPieces[i].x >= 0 && initialPieces[i].y < xDim
                                                && initialPieces[i].y >=0 && initialPieces[i].y <yDim)
                    {
                        SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
                    }
                }
            }

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (_pieces[x, y] == null)
                        SpawnNewPiece(x, y, PieceType.Empty);          
                }
            }

            StartCoroutine(Fill());
        }

        private void FillMapList(List<List<int>> _list, string[] _map)
        {
            for (int i = 0; i < _mapInfo.height; i++)
            {   
                string[] _row = _map[i].Split(' ');
                List<int> __row = new List<int>();
                for(int j = 0; j < _mapInfo.width; j++)
                    __row.Add(Convert.ToInt32(_row[j]));

                _list.Add(__row);
            }
        }

        private void ChangeComponents()
        {
            hud = this.GetComponent<Hud>();
            switch(_mapInfo.type)
            {
                case "LevelMoves":
                    this.GetComponent<LevelMoves>().enabled = true;
                    this.GetComponent<LevelObstacles>().enabled = false;
                    this.GetComponent<LevelTimer>().enabled = false;
                    level = this.GetComponent<LevelMoves>();
                    break;
                case "LevelObstacles":
                    this.GetComponent<LevelMoves>().enabled = false;
                    this.GetComponent<LevelObstacles>().enabled = true;
                    this.GetComponent<LevelTimer>().enabled = false;
                    level = this.GetComponent<LevelObstacles>();
                    break;
                case "LevelTimer":
                    this.GetComponent<LevelMoves>().enabled = false;
                    this.GetComponent<LevelObstacles>().enabled = false;
                    this.GetComponent<LevelTimer>().enabled = true;
                    level = this.GetComponent<LevelTimer>();
                    break;
            }
            hud.level = level;
        }

        private IEnumerator Fill()
        {        
            bool needsRefill = true;
            IsFilling = true;

            while (needsRefill)
            {
                yield return new WaitForSeconds(fillTime);
                while (FillStep())
                {
                    _inverse = !_inverse;
                    yield return new WaitForSeconds(fillTime);
                }

                needsRefill = ClearAllValidMatches();
            }

            IsFilling = false;
        }

        private bool FillStep()
        {
            bool movedPiece = false;
            // y = 0 is at the top, we ignore the last row, since it can't be moved down.
            for (int y = yDim - 2; y >= 0; y--)
            {
                for (int loopX = 0; loopX < xDim; loopX++)
                {
                    int x = loopX;
                    if (_inverse) { x = xDim - 1 - loopX; }
                    GamePiece piece = _pieces[x, y];

                    if (!piece.IsMovable()) continue;
                
                    GamePiece pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.Empty)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.Empty);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag == 0) continue;
                        
                            int diagX = x + diag;

                            if (_inverse)
                                diagX = x - diag;

                            if (diagX < 0 || diagX >= xDim) continue;
                        
                            GamePiece diagonalPiece = _pieces[diagX, y + 1];

                            if (diagonalPiece.Type != PieceType.Empty) continue;
                        
                            bool hasPieceAbove = true;

                            for (int aboveY = y; aboveY >= 0; aboveY--)
                            {
                                GamePiece pieceAbove = _pieces[diagX, aboveY];

                                if (pieceAbove.IsMovable())
                                    break;
                                else if (pieceAbove.Type != PieceType.Empty)
                                {
                                    hasPieceAbove = false;
                                    break;
                                }
                            }

                            if (hasPieceAbove) continue;
                        
                            Destroy(diagonalPiece.gameObject);
                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                            _pieces[diagX, y + 1] = piece;
                            SpawnNewPiece(x, y, PieceType.Empty);
                            movedPiece = true;
                            break;
                        }
                    }
                }
            }

            for (int x = 0; x < xDim; x++)
            {
                GamePiece pieceBelow = _pieces[x, 0];

                if (pieceBelow.Type != PieceType.Empty) continue;
            
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = Instantiate(_piecePrefabDict[PieceType.Normal], GetWorldPosition(x, -1), Quaternion.identity, this.transform);

                _pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                _pieces[x, 0].Init(x, -1, this, PieceType.Normal);
                _pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                if(_colorMap[x].Count > 0) 
                {
                    _pieces[x, 0].ColorComponent.SetColor((ColorType)_colorMap[x][0]);
                    _colorMap[x].RemoveAt(0);
                }
                else _pieces[x, 0].ColorComponent.SetColor((ColorType)UnityEngine.Random.Range(0, _pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }

            return movedPiece;
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(
                transform.position.x - xDim / 2.0f + x,
                transform.position.y + yDim / 2.0f - y);
        }

        private GamePiece SpawnNewPiece(int x, int y, PieceType type)
        {
            GameObject newPiece = Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity, this.transform);
            _pieces[x, y] = newPiece.GetComponent<GamePiece>();
            _pieces[x, y].Init(x, y, this, type);

            return _pieces[x, y];
        }

        private static bool IsAdjacent(GamePiece piece1, GamePiece piece2) =>
            (piece1.X == piece2.X && Mathf.Abs(piece1.Y - piece2.Y) == 1) ||
            (piece1.Y == piece2.Y && Mathf.Abs(piece1.X - piece2.X) == 1);

        private void SwapPieces(GamePiece piece1, GamePiece piece2)
        {
            if (_gameOver) { return; }

            if (!piece1.IsMovable() || !piece2.IsMovable()) return;
        
            _pieces[piece1.X, piece1.Y] = piece2;
            _pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || 
                GetMatch(piece2, piece1.X, piece1.Y) != null ||
                piece1.Type == PieceType.Rainbow ||
                piece2.Type == PieceType.Rainbow)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

                if (piece1.Type == PieceType.Rainbow && piece1.IsClearable() && piece2.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor)
                        clearColor.Color = piece2.ColorComponent.Color;

                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.Rainbow && piece2.IsClearable() && piece1.IsColored())
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                    if (clearColor)
                        clearColor.Color = piece1.ColorComponent.Color;

                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches();

                // special pieces get cleared, event if they are not matched
                if (piece1.Type == PieceType.RowClear || piece1.Type == PieceType.ColumnClear)
                    ClearPiece(piece1.X, piece1.Y);

                if (piece2.Type == PieceType.RowClear || piece2.Type == PieceType.ColumnClear)
                    ClearPiece(piece2.X, piece2.Y);

                _pressedPiece = null;
                _enteredPiece = null;

                StartCoroutine(Fill());

                level.OnMove();
            }
            else
            {
                _pieces[piece1.X, piece1.Y] = piece1;
                _pieces[piece2.X, piece2.Y] = piece2;
            }
        }

        public void PressPiece(GamePiece piece) => _pressedPiece = piece;

        public void EnterPiece(GamePiece piece) => _enteredPiece = piece;

        public void ReleasePiece()
        {
            if (IsAdjacent (_pressedPiece, _enteredPiece))
                SwapPieces(_pressedPiece, _enteredPiece);
        }

        private bool ClearAllValidMatches()
        {
            bool needsRefill = false;

            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    if (!_pieces[x, y].IsClearable()) continue;
                
                    List<GamePiece> match = GetMatch(_pieces[x, y], x, y);

                    if (match == null) continue;
                
                    PieceType specialPieceType = PieceType.Count;
                    GamePiece randomPiece = match[UnityEngine.Random.Range(0, match.Count)];
                    int specialPieceX = randomPiece.X;
                    int specialPieceY = randomPiece.Y;

                    // Spawning special pieces
                    if (match.Count == 4)
                    {
                        if (_pressedPiece == null || _enteredPiece == null)
                            specialPieceType = (PieceType)UnityEngine.Random.Range((int) PieceType.RowClear, (int) PieceType.ColumnClear);
                        else if (_pressedPiece.Y == _enteredPiece.Y)
                            specialPieceType = PieceType.RowClear;
                        else
                            specialPieceType = PieceType.ColumnClear;
                    } // Spawning a rainbow piece
                    else if (match.Count >= 5)
                        specialPieceType = PieceType.Rainbow;

                    foreach (var gamePiece in match)
                    {
                        if (!ClearPiece(gamePiece.X, gamePiece.Y)) continue;
                    
                        needsRefill = true;

                        if (gamePiece != _pressedPiece && gamePiece != _enteredPiece) continue;
                    
                        specialPieceX = gamePiece.X;
                        specialPieceY = gamePiece.Y;
                    }

                    // Setting their colors
                    if (specialPieceType == PieceType.Count) continue;
                
                    Destroy(_pieces[specialPieceX, specialPieceY]);
                    GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                    if ((specialPieceType == PieceType.RowClear || specialPieceType == PieceType.ColumnClear) 
                        && newPiece.IsColored() && match[0].IsColored())
                        newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                    else if (specialPieceType == PieceType.Rainbow && newPiece.IsColored())
                        newPiece.ColorComponent.SetColor(ColorType.Any);
                }
            }

            return needsRefill;
        }

        private List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
        {
            if (!piece.IsColored()) return null;
            var color = piece.ColorComponent.Color;
            var horizontalPieces = new List<GamePiece>();
            var verticalPieces = new List<GamePiece>();
            var matchingPieces = new List<GamePiece>();

            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if (dir == 0)
                        x = newX - xOffset;
                    else
                        x = newX + xOffset;

                    if (x < 0 || x >= xDim) { break; }

                    if (_pieces[x, newY].IsColored() && _pieces[x, newY].ColorComponent.Color == color)
                        horizontalPieces.Add(_pieces[x, newY]);
                    else
                        break;
                }
            }

            if (horizontalPieces.Count >= 3)
                matchingPieces.AddRange(horizontalPieces);

            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++ )
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)                        
                        {
                            int y;
                            
                            if (dir == 0)
                                y = newY - yOffset;
                            else
                                y = newY + yOffset;

                            if (y < 0 || y >= yDim)
                                break;

                            if (_pieces[horizontalPieces[i].X, y].IsColored() && _pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
                            else
                                break;
                        }
                    }

                    if (verticalPieces.Count < 2)
                        verticalPieces.Clear();
                    else
                    {
                        matchingPieces.AddRange(verticalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
                return matchingPieces;

            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < xDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                        y = newY - yOffset;
                    else
                        y = newY + yOffset;                        

                    if (y < 0 || y >= yDim) { break; }

                    if (_pieces[newX, y].IsColored() && _pieces[newX, y].ColorComponent.Color == color)
                        verticalPieces.Add(_pieces[newX, y]);
                    else
                        break;
                }
            }

            if (verticalPieces.Count >= 3)
                matchingPieces.AddRange(verticalPieces);

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                                x = newX - xOffset;
                            else
                                x = newX + xOffset;

                            if (x < 0 || x >= xDim)
                                break;

                            if (_pieces[x, verticalPieces[i].Y].IsColored() && _pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                                horizontalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            else
                                break;
                        }
                    }

                    if (horizontalPieces.Count < 2)
                        horizontalPieces.Clear();
                    else
                    {
                        matchingPieces.AddRange(horizontalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
                return matchingPieces;

            return null;
        }

        private bool ClearPiece(int x, int y)
        {
            if (!_pieces[x, y].IsClearable() || _pieces[x, y].ClearableComponent.IsBeingCleared) return false;
        
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.Empty);

            ClearObstacles(x, y);

            return true;

        }

        public bool ChangePiece(int x, int y, PieceType type)
        {
            if (!_pieces[x, y].IsClearable() || _pieces[x, y].ClearableComponent.IsBeingCleared) return false;
            
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, type);
            _pieces[x, y].ColorComponent.SetColor((ColorType)UnityEngine.Random.Range(0, _pieces[x, 0].ColorComponent.NumColors));

            ClearObstacles(x, y);

            return true;

        }

        private void ClearObstacles(int x, int y)
        {
            for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
            {
                if (adjacentX == x || adjacentX < 0 || adjacentX >= xDim) continue;

                if (_pieces[adjacentX, y].Type != PieceType.Frozen || !_pieces[adjacentX, y].IsClearable()) continue;
            
                _pieces[adjacentX, y].ClearableComponent.Clear();
                SpawnNewPiece(adjacentX, y, PieceType.Empty);
            }

            for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
            {
                if (adjacentY == y || adjacentY < 0 || adjacentY >= yDim) continue;

                if (_pieces[x, adjacentY].Type != PieceType.Frozen || !_pieces[x, adjacentY].IsClearable()) continue;
            
                _pieces[x, adjacentY].ClearableComponent.Clear();
                SpawnNewPiece(x, adjacentY, PieceType.Empty);
            }
        }

        public void ClearRow(int row)
        {
            for (int x = 0; x < xDim; x++)
                ClearPiece(x, row);
        }

        public void ClearColumn(int column)
        {
            for (int y = 0; y < yDim; y++)
                ClearPiece(column, y);
        }

        public void ClearColor(ColorType color)
        {
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if ((_pieces[x, y].IsColored() && _pieces[x, y].ColorComponent.Color == color)
                        || (color == ColorType.Any))
                        ClearPiece(x, y);
                }
            }
        }

        public void GameOver() => _gameOver = true;

        public List<GamePiece> GetPiecesOfType(PieceType type)
        {
            var piecesOfType = new List<GamePiece>();

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (_pieces[x, y].Type == type)
                        piecesOfType.Add(_pieces[x, y]);
                }
            }

            return piecesOfType;
        }

    }
}
