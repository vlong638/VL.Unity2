using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.VL.Unity.Core.Utilities;
using System.Linq;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    #region  Materials
    public GameObject GOFloor;
    public Sprite Floor;
    public Sprite Flag;
    public Sprite Hesitate;
    public Sprite Mine;
    public Sprite Bomb;
    public Sprite Risk0;
    public Sprite Risk1;
    public Sprite Risk2;
    public Sprite Risk3;
    public Sprite Risk4;
    public Sprite Risk5;
    public Sprite Risk6;
    public Sprite Risk7;
    public Sprite Risk8;
    public Sprite GetSprite(CellType type, int value = 1)
    {
        switch (type)
        {
            case CellType.Floor:
                return Floor;
            case CellType.Flag:
                return Flag;
            case CellType.Hesitate:
                return Hesitate;
            case CellType.Mine:
                return Mine;
            case CellType.Bomb:
                return Bomb;
            case CellType.Risk0:
                return Risk0;
            case CellType.Risk1:
                return Risk1;
            case CellType.Risk2:
                return Risk2;
            case CellType.Risk3:
                return Risk3;
            case CellType.Risk4:
                return Risk4;
            case CellType.Risk5:
                return Risk5;
            case CellType.Risk6:
                return Risk6;
            case CellType.Risk7:
                return Risk7;
            case CellType.Risk8:
                return Risk8;
            case CellType.None:
            default:
                return null;
        }
    }
    public CellType GetRiskyCellType(bool isMine, int value = 1)
    {
        if (isMine)
        {
            return CellType.Mine;
        }
        else
        {
            switch (value)
            {
                case 0:
                    return CellType.Risk0;
                case 1:
                    return CellType.Risk1;
                case 2:
                    return CellType.Risk2;
                case 3:
                    return CellType.Risk3;
                case 4:
                    return CellType.Risk4;
                case 5:
                    return CellType.Risk5;
                case 6:
                    return CellType.Risk6;
                case 7:
                    return CellType.Risk7;
                case 8:
                    return CellType.Risk8;
            }
        }
        return CellType.None;
    }
    #endregion

    #region Cells
    public enum CellType
    {
        None,
        Floor,
        Flag,
        Hesitate,
        Mine,
        Bomb,
        Risk0,
        Risk1,
        Risk2,
        Risk3,
        Risk4,
        Risk5,
        Risk6,
        Risk7,
        Risk8,
    }
    public class Cell
    {
        public int X;
        public int Y;
        public bool IsMine;
        public int RiskLevel;
        //public Vector2 Locator;
        public CellType UserType;
        public CellType BackgroundType;
        GameBoard GameBoard;
        public GameObject Object;
        public SpriteRenderer Renderer;

        public Cell(int x, int y, GameBoard gameBoard, GameObject prototype, Vector2 locator, CellType gOUser, CellType gOEnvironment)
        {
            X = x;
            Y = y;
            GameBoard = gameBoard;
            UserType = gOUser;
            BackgroundType = gOEnvironment;
            Object = Instantiate(prototype, locator, Quaternion.identity) as GameObject;
            Renderer = Object.GetComponent<SpriteRenderer>();
        }

        public enum OpenResult
        {
            None,
            OpenSafe,
            OpenRisk,
            OpenMark,
            Bomb,
        }
        public OpenResult Open(GameBoard gameBoard)
        {
            OpenResult result = OpenResult.None;
            if (UserType == CellType.Hesitate || UserType == CellType.Flag)
            {
                SetUserType(CellType.Floor);
                result = OpenResult.OpenMark;
            }
            if (UserType == CellType.Floor)
            {
                if (BackgroundType != CellType.Mine)
                {
                    if (RiskLevel > 0)
                        result = OpenResult.OpenRisk;
                    else
                        result = OpenResult.OpenSafe;
                    SetUserType(BackgroundType);
                }
                else
                {
                    SetBackgroundType(CellType.Bomb);
                    result = OpenResult.Bomb;
                }
            }
            UpdateUserInterface();
            return result;
        }
        public enum OpenSafeResult
        {
            NoOpen,
            OpenSafe,
            OpenRisk,
        }
        public OpenSafeResult OpenSafe()
        {
            if (UserType == CellType.Floor || UserType == CellType.Hesitate || UserType == CellType.Flag)
            {
                if (BackgroundType == CellType.Risk0)
                {
                    SetUserType(CellType.Risk0);
                    UpdateUserInterface();
                    return OpenSafeResult.OpenSafe;
                }
                else
                {
                    SetUserType(BackgroundType);
                    UpdateUserInterface();
                    return OpenSafeResult.OpenRisk;
                }
            }
            return OpenSafeResult.NoOpen;
        }
        public void Mark()
        {
            if (UserType == CellType.Floor)
            {
                UserType = CellType.Flag;
            }
            else if (UserType == CellType.Flag)
            {
                UserType = CellType.Hesitate;
            }
            else if (UserType == CellType.Hesitate)
            {
                UserType = CellType.Floor;
            }
            UpdateUserInterface();
        }


        public void SetUserType(CellType environment)
        {
            UserType = environment;
        }
        public void SetBackgroundType(CellType environment)
        {
            BackgroundType = environment;
        }
        public void UpdateUserInterface()
        {
            Renderer.sprite = GameBoard.GetSprite(UserType);
        }
        public void UpdateBackground()
        {
            Renderer.sprite = GameBoard.GetSprite(BackgroundType);
        }
    }
    public class CellCollection
    {
        public int Width;
        public int Height;
        public int FloorLeft;
        public Cell[,] CellMatrix;
        public List<Cell> Cells;
        GameBoard GameBoard;

        //一像素0.03
        public CellCollection(GameBoard gameBoard, GameObject prototype, int width, int height, int mine)
        {
            GameBoard = gameBoard;
            float vectorCoefficient = 0.48f;
            Vector2 startPoint = new Vector2(-width / 2, -height / 2) * vectorCoefficient;
            Width = width;
            Height = height;
            FloorLeft = Width * Height - mine;
            CellMatrix = new Cell[Width, Height];
            Cells = new List<Cell>();
            //地板
            for (int w = 0; w < Width; w++)
            {
                for (int h = 0; h < Height; h++)
                {
                    var cell = new Cell(w, h, gameBoard, prototype, startPoint + new Vector2(w, h) * vectorCoefficient, CellType.Floor, CellType.Floor);
                    CellMatrix[w, h] = (cell);
                    Cells.Add(cell);
                }
            }
            //埋雷
            var jar = JarHelper.GetJarOfIntPair(0, Width - 1, 0, Height - 1);
            for (int i = 0; i < mine; i++)
            {
                var vector = jar.Roll();
                var cell = CellMatrix[vector.X, vector.Y];
                cell.SetBackgroundType(CellType.Mine);
                cell.IsMine = true;
                UpdateCellMatrixField(vector.X - 1, vector.Y - 1);
                UpdateCellMatrixField(vector.X - 1, vector.Y);
                UpdateCellMatrixField(vector.X - 1, vector.Y + 1);
                UpdateCellMatrixField(vector.X, vector.Y - 1);
                UpdateCellMatrixField(vector.X, vector.Y);
                UpdateCellMatrixField(vector.X, vector.Y + 1);
                UpdateCellMatrixField(vector.X + 1, vector.Y - 1);
                UpdateCellMatrixField(vector.X + 1, vector.Y);
                UpdateCellMatrixField(vector.X + 1, vector.Y + 1);
            }
            //预警
            for (int w = 0; w < Width; w++)
            {
                for (int h = 0; h < Height; h++)
                {
                    var cell = CellMatrix[w, h];
                    cell.SetBackgroundType(gameBoard.GetRiskyCellType(cell.IsMine, cell.RiskLevel));
                    Cells.Add(cell);
                }
            }
        }

        public void Open(GameObject gameObject)
        {
            var cell = Cells.First(c => c.Object == gameObject);
            var result = cell.Open(GameBoard);
            switch (result)
            {
                case Cell.OpenResult.OpenSafe:
                    FloorLeft -= 1;
                    OpenSafeAll(cell.X, cell.Y);
                    break;
                case Cell.OpenResult.OpenRisk:
                    FloorLeft -= 1;
                    if (FloorLeft == 0)
                        GameBoard.Win();
                    break;
                case Cell.OpenResult.OpenMark:
                    break;
                case Cell.OpenResult.Bomb:
                    GameBoard.Lose();
                    break;
                default:
                    throw new UnityException("未处理类型");
            }
        }
        public void Mark(GameObject gameObject)
        {
            var cell = Cells.First(c => c.Object == gameObject);
            cell.Mark();
        }
        private void OpenSafeAll(int x, int y)
        {
            if (OpenSafe(x - 1, y))
            {
                OpenSafeAll(x - 1, y);
            }
            if (OpenSafe(x + 1, y))
            {
                OpenSafeAll(x + 1, y);
            }
            if (OpenSafe(x, y - 1))
            {
                OpenSafeAll(x, y - 1);
            }
            if (OpenSafe(x, y + 1))
            {
                OpenSafeAll(x, y + 1);
            }
        }
        private bool OpenSafe(int w, int h)
        {
            if (w < 0 || w >= Width)
                return false;
            if (h < 0 || h >= Height)
                return false;
            var result = CellMatrix[w, h].OpenSafe();
            switch (result)
            {
                case Cell.OpenSafeResult.NoOpen:
                    return false;
                case Cell.OpenSafeResult.OpenSafe:
                    FloorLeft -= 1;
                    return true;
                case Cell.OpenSafeResult.OpenRisk:
                    FloorLeft -= 1;
                    return false;
                default:
                    throw new UnityException("未实现的类型");
            }
        }
        private void UpdateCellMatrixField(int w, int h)
        {
            if (w < 0 || w >= Width)
                return;
            if (h < 0 || h >= Height)
                return;
            CellMatrix[w, h].RiskLevel += 1;
        }
        public void ShowUserInterFace()
        {
            foreach (var cell in Cells)
            {
                cell.UpdateUserInterface();
            }
        }
        public void ShowEnvironment()
        {
            foreach (var cell in Cells)
            {
                cell.UpdateBackground();
            }
        }
        public void DestroyObjects()
        {
            foreach (var cell in Cells)
            {
                Destroy(cell.Object);
            }
        }
    }
    public void UpdateCellDisplay()
    {
        if (IsUserInterface)
        {
            CellCollector.ShowEnvironment();
            IsUserInterface = false;
        }
        else
        {
            CellCollector.ShowUserInterFace();
            IsUserInterface = true;
        }
    }

    #endregion

    #region Buttons
    public Button Button_Start;
    public Button Button_Restart;
    public Button Button_SwitchDisplay;
    enum TurnType
    {
        WaitForStart,
        WaitForFinish,
        Finished,
    }
    TurnType CurrentTurn;
    private void UpdateButtons()
    {
        switch (CurrentTurn)
        {
            case TurnType.WaitForStart:
                Button_Start.gameObject.SetActive(true);
                Button_Restart.gameObject.SetActive(false);
                Button_SwitchDisplay.gameObject.SetActive(false);
                break;
            case TurnType.WaitForFinish:
            case TurnType.Finished:
                Button_Start.gameObject.SetActive(false);
                Button_Restart.gameObject.SetActive(true);
                Button_SwitchDisplay.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Awake
    private void Awake()
    {
        CurrentTurn = TurnType.WaitForStart;
        UpdateButtons();
    }
    public int Width = 9;
    public int Height = 9;
    public int MineCount = 10;
    CellCollection CellCollector;
    bool IsUserInterface;
    public void SetupNewGame()
    {
        if (CellCollector != null)
            CellCollector.DestroyObjects();

        int width = Width;
        int height = Height;
        int mine = MineCount;
        FloorLeft = width * height - mine;
        if (FloorLeft <= 0)
            throw new UnityException("无效的初始化数据");
        IsUserInterface = true;

        CellCollector = new CellCollection(this, GOFloor, width, height, mine);
        CellCollector.ShowUserInterFace();
        CurrentTurn = TurnType.WaitForFinish;
        UpdateButtons();
    }
    #endregion

    #region Output
    public Text Text_OutPut;
    List<string> Output = new List<string>();
    int limitLength = 3;
    void AddText(string text)
    {
        Output.Add(text);
        if (Output.Count() > limitLength)
        {
            Output.RemoveAt(0);
        }
        Text_OutPut.text = string.Join(System.Environment.NewLine, Output.ToArray());
    }
    void AddText(string title, Vector3 vector)
    {
        AddText(string.Format("{3}:{0},{1},{2}", vector.x.ToString("F2"), vector.y.ToString("F2"), vector.z.ToString("F2"), title));
    }
    void AddText(string text, params string[] args)
    {
        Output.Add(string.Format(text, args));
        if (Output.Count() > limitLength)
        {
            Output.RemoveAt(0);
        }
        Text_OutPut.text = string.Join(System.Environment.NewLine, Output.ToArray());
    }
    #endregion

    #region Update
    private void Update()
    {
        //首先判断是否点击了鼠标左键
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (CurrentTurn == TurnType.WaitForFinish && (leftClick || rightClick))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction);
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 1);
            if (hit.transform != null)
            {
                AddText("检测到碰撞,碰撞体位置", hit.transform.position);
                if (leftClick)
                {
                    CellCollector.Open(hit.collider.gameObject);
                }
                if (rightClick)
                {
                    CellCollector.Mark(hit.collider.gameObject);
                }
            }
            else
            {
                AddText("未检测到碰撞,起始位置", ray.direction);
                AddText("未检测到碰撞,当前鼠标位置", ray.origin);
            }
        }
    }
    public int FloorLeft;
    public void Win()
    {
        CurrentTurn = TurnType.Finished;
        AddText("你胜利了");
    }
    public void Lose()
    {
        CurrentTurn = TurnType.Finished;
        CellCollector.ShowEnvironment();
        AddText("你失败了");
    }
    #endregion
}
