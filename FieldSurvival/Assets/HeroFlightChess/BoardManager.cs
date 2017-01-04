using Assets.VL.Scripts;
using Assets.VL.VL.Unity.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region 逻辑对象
public enum Group
{
    Red,
    Yellow,
    Blue,
    Green,
}
public class GroupJar
{
    List<Group> Groups = new List<Group>();

    public GroupJar()
    {
        var groupNames = Enum.GetNames(typeof(Group));
        foreach (var groupName in groupNames)
        {
            var group = (Group)Enum.Parse(typeof(Group), groupName);
            Groups.Add(group);
        }
    }

    public Group Roll()
    {
        var group = Groups[UnityEngine.Random.Range(0, Groups.Count)];
        Groups.Remove(group);
        return group;
    }
    public Group Get(Group group)
    {
        Groups.Remove(group);
        return group;
    }
}
public static class GroupEx
{
    public static Color GetColor(this Group group)
    {
        switch (group)
        {
            case Group.Red:
                return Color.red;
            case Group.Yellow:
                return Color.yellow;
            case Group.Blue:
                return Color.blue;
            case Group.Green:
                return Color.green;
            default:
                return Color.white;
        }
    }
}
public class MoveMent
{
    public Group Group;
    public int Vatality;

    public MoveMent(Group group, int vatality)
    {
        Group = group;
        Vatality = vatality;
    }
}
public enum FloorType
{
    /// <summary>
    /// 卵区,出生点
    /// </summary>
    Spawn,
    /// <summary>
    /// 起始点
    /// </summary>
    StartPoint,
    /// <summary>
    /// 常规路径
    /// </summary>
    Route,
    /// <summary>
    /// 转角
    /// </summary>
    Corner,
    /// <summary>
    /// 结束点
    /// </summary>
    EndPoint,
}
public enum PlayerLevel
{
    Easy,
    Normal,
    Hard,
    Crazy,
}
#endregion

public static class VectorEx
{
    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }
}
/// <summary>
/// 游戏面板,棋板
/// </summary>
public class BoardManager : MonoBehaviour
{
    public GameObject Cell;
    public GameObject Object;

    //public GameObject TargetB;
    //public GameObject TargetG;
    //public GameObject TargetR;
    //public GameObject TargetY;

    //public GameObject ObjectB;
    //public GameObject ObjectG;
    //public GameObject ObjectR;
    //public GameObject ObjectY;

    //public GameObject CellB;
    //public GameObject CellG;
    //public GameObject CellR;
    //public GameObject CellY;

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= InitGame;
        SceneManager.sceneLoaded += InitGame;
    }
    private void OnDisable()
    {
    }

    #region 内建对象
    public static class PlayerGenerator
    {
        static string[] Names = new string[] { "疯狂的狮子头","屁毛兽","金月工",
        "斑马", "狗", "狐", "熊", "象", "豹子", "麝牛", "狮子",
        "小熊猫", "疣猪", "羚羊", "驯鹿", "考拉", "犀牛", "猞猁", "穿山甲",
        "长颈鹿", "熊猫", "食蚁兽", "猩猩", "海牛", "水獭",  "灵猫",
        "海豚",   "海象", "鸭嘴兽", "刺猬",
    };
        public static string GetName()
        {
            return Names.GetRandomOne();
        }

        public static Player GetPlayer(Group group, PlayerLevel level = PlayerLevel.Normal)
        {
            switch (level)
            {
                case PlayerLevel.Easy:
                    break;
                case PlayerLevel.Normal:
                    break;
                case PlayerLevel.Hard:
                    break;
                case PlayerLevel.Crazy:
                    break;
            }
            return new Player(GetName(), group);
        }
    }
    public class GroupSet
    {
        public Player Player;
        public GameObject Cell;
        public GameObject Object;

        public GroupSet(Player player, GameObject cell, GameObject @object, bool isPlayer = false)
        {
            Player = player;
            var color = player.Group.GetColor();
            var render = cell.GetComponent<SpriteRenderer>();
            render.color = color;
            Cell = Instantiate(cell);
            render = @object.GetComponent<SpriteRenderer>();
            render.color = color;
            Object = Instantiate(@object);
            if (isPlayer)
            {
                //var collider = Object.AddComponent<BoxCollider2D>();
                //collider.size = new Vector2(0.3f, 0.3f);
                Object.tag = Constraints.Tag_PlayerUnit;
            }
        }
    }
    public class Player
    {
        public string Name;
        public int Score;
        public Group Group;
        public MoveMent Movement;
        public List<Unit> Units = new List<Unit>();

        public Player(string name, Group group)
        {
            Name = name;
            Group = group;
            Movement = new MoveMent(Group, 0);
        }

        public void Roll()
        {
            var roll = RandomHelper.Next(1, 7);
            Movement = new MoveMent(Group, roll);
        }
        public void AddVatality(int vatality)
        {
            Movement.Vatality += vatality;
        }
    }
    public class Floor
    {
        public Floor Pre { set; get; }
        public Vector2 Locator { set; get; }
        public FloorType FloorType { set; get; }
        public GameObject Object { set; get; }
        public Player Player { set; get; }

        public Floor(Vector2 locator, FloorType floorType, GameObject prototype, Player player)
        {
            Locator = locator;
            FloorType = floorType;
            Player = player;
            Object = Instantiate(prototype, locator, Quaternion.identity) as GameObject;
        }
        public Floor(Vector2 locator, FloorType floorType, GameObject prototype, Player owner, Func<Floor, MoveMent, bool> nextCondition, Floor nextValue) : this(locator, floorType, prototype, owner)
        {
            AddNext(nextCondition, nextValue);
        }

        private List<Func<Floor, MoveMent, Floor>> Nexts = new List<Func<Floor, MoveMent, Floor>>();
        public List<Floor> References = new List<Floor>();
        public int AddReference(Floor Reference)
        {
            References.Add(Reference);
            return References.Count - 1;
        }
        public void AddNext(Func<Floor, MoveMent, bool> nextCondition, Floor nextValue)
        {
            var index = AddReference(nextValue);
            if (nextCondition == null)
                nextCondition = (f, m) => true;
            Nexts.Insert(0, (f, m) =>
            {
                if (nextCondition(this, m))
                {
                    return References[index];
                }
                return null;
            });
            //Display(nextCondition(new MoveMent(CurrentPlayer.Group, 1)));
        }
        public Floor GetNext(MoveMent move)
        {
            var nextFuc = Nexts.FirstOrDefault(n => n(this, move) != null);
            if (nextFuc == null)
                return null;
            return nextFuc(this, move);
        }
        public void Display(Floor n)
        {
            if (n != null)
            {
                var start = Locator.ToVector3();
                var end = n.Locator.ToVector3();
                var mid = (start + end) / 2;
                Debug.DrawLine(start, mid, Color.red, 2000, false);
                Debug.DrawLine(mid, end, Color.yellow, 2000, false);
            }
        }
        public Unit Unit { set; get; }
    }
    public float inverseMoveTime = 2f;
    public class Unit
    {
        public Player Player;
        public GameObject Obj;
        public Floor Start;
        public Floor Current;
        public Rigidbody2D Rigidbody2D;
        public BoxCollider2D BoxCollider2D;

        public Unit(Player player, Floor start, GameObject prototype)
        {
            Player = player;
            Player.Units.Add(this);
            Start = start;
            Current = start;
            Obj = Instantiate(prototype, start.Locator, Quaternion.identity) as GameObject;
            Rigidbody2D = Obj.GetComponent<Rigidbody2D>();
            BoxCollider2D = Obj.GetComponent<BoxCollider2D>();
        }

        //public void Move(BoardManager board)
        public bool CanMove(BoardManager board)
        {
            if (Current.FloorType != FloorType.Spawn)
            {
                if (Player.Movement.Vatality > 0)
                {
                    return true;
                }
                else
                {
                    board.AddText("没有行动点了");
                    return false;
                }
            }
            else
            {
                if (Player.Movement.Vatality == 6)
                {
                    return true;
                }
                else
                {
                    board.AddText("行动点不足以发车哦");
                    return false;
                }
            }
            //return (Current.FloorType != FloorType.Spawn && Player.Movement.Vatality > 0) || (Current.FloorType == FloorType.Spawn && Player.Movement.Vatality == 6);
        }
        public IEnumerator Move(BoardManager board)
        {
            //关闭Collider
            foreach (var player in board.Players)
            {
                foreach (Unit unit in player.Units)
                {
                    unit.BoxCollider2D.enabled = false;
                }
            }
            bool isForward = true;
            if (Current.FloorType == FloorType.Spawn && Player.Movement.Vatality == 6)
            {
                Floor now = Current;
                var next = Current.GetNext(null);
                Vector3 nextPosition = new Vector3(next.Locator.x, next.Locator.y, 0);
                board.AddText("发车");
                board.AddText("当前位置", now.Locator);
                board.AddText("目标位置", next.Locator);
                //移动对象,平滑移动
                float sqrRemainingDistance = (Obj.transform.position - nextPosition).sqrMagnitude;
                while (sqrRemainingDistance > float.Epsilon)
                {
                    Vector3 newPostion = Vector3.MoveTowards(Rigidbody2D.position, nextPosition, board.inverseMoveTime * Time.deltaTime);
                    Rigidbody2D.MovePosition(newPostion);
                    sqrRemainingDistance = (Obj.transform.position - nextPosition).sqrMagnitude;
                    yield return null;
                }
                Player.Movement.Vatality -= 6;
                Current = next;
            }
            else
            {
                Floor now = Current;
                while (Player.Movement.Vatality > 0)
                {
                    board.AddText("剩余移动点数:" + Player.Movement.Vatality);
                    //确定移动位置
                    Floor next;
                    if (isForward)
                    {
                        next = now.GetNext(Player.Movement);
                        if (next.FloorType == FloorType.EndPoint)
                            isForward = false;
                    }
                    else
                        next = now.Pre;
                    //TEST Output
                    Vector3 nextPosition = new Vector3(next.Locator.x, next.Locator.y, 0);
                    board.AddText("开始移动");
                    board.AddText("当前位置", now.Locator);
                    board.AddText("目标位置", next.Locator);
                    //移动对象,平滑移动
                    float sqrRemainingDistance = (Obj.transform.position - nextPosition).sqrMagnitude;
                    while (sqrRemainingDistance > float.Epsilon)
                    {
                        Vector3 newPostion = Vector3.MoveTowards(Rigidbody2D.position, nextPosition, board.inverseMoveTime * Time.deltaTime);
                        Rigidbody2D.MovePosition(newPostion);
                        sqrRemainingDistance = (Obj.transform.position - nextPosition).sqrMagnitude;
                        yield return null;
                    }
                    Player.Movement.Vatality -= 1;
                    now = next;
                }
                Current = now;
                if (Current.FloorType==FloorType.EndPoint)
                {
                    Player.Score++;
                    Obj.SetActive(false);
                    board.UpdateScore();
                }
            }
            //重启Collider
            foreach (var player in board.Players)
            {
                foreach (Unit unit in player.Units)
                {
                    unit.BoxCollider2D.enabled = true;
                }
            }
            //是否结束
            if (board.IsFinished())
                board.NextPlayer();
        }
    }
    #endregion


    static Player CurrentPlayer;
    Player[] Players;
    private void InitGame(Scene arg0, LoadSceneMode arg1)
    {
        #region 素材准备
        var jar = new GroupJar();
        Players = new Player[4];
        GroupSet[] sets = new GroupSet[4];
        Players[0] = new Player(GameManager.Instance.PlayerManager.PlayerName, jar.Roll());
        CurrentPlayer = Players[0];
        sets[0] = new GroupSet(Players[0], Cell, Object, true);
        for (int i = 1; i < 4; i++)
        {
            Players[i] = PlayerGenerator.GetPlayer(jar.Roll());
            sets[i] = new GroupSet(Players[i], Cell, Object);
        }
        #endregion

        #region 构建棋盘
        Vector2 locatorStart = new Vector2(0, 0);
        Vector2 step = new Vector2(0.5f, 0);
        int size = sets.Count();
        int radius = 8;
        int rotation = -90;
        Vector2 locatorNow = locatorStart;
        List<Floor> edgeStarts = new List<Floor>();
        List<Floor> edgeEnds = new List<Floor>();
        Floor pre = null;
        Floor current = null;
        for (int i = 0; i < 4; i++)
        {
            //素材集合
            GroupSet set = sets[i];
            //起始点
            locatorNow = locatorStart;
            Vector2 stepNow = Quaternion.Euler(0, 0, rotation * i) * step;
            locatorNow += stepNow;
            current = new Floor(locatorNow, FloorType.EndPoint, set.Cell, set.Player);
            pre = current;
            //通关路径
            for (int j = 0; j < radius - 3; j++)
            {
                locatorNow += stepNow;
                current = new Floor(locatorNow, FloorType.Route, set.Cell, set.Player, null, pre);
                pre.Pre = current;
                pre = current;
            }
            //转弯路径
            locatorNow += stepNow;
            var group = set.Player.Group;
            current = new Floor(locatorNow, FloorType.Corner, set.Cell, set.Player, (f, m) => m.Group == f.Player.Group, pre);
            edgeStarts.Add(current);
            pre = current;
            //常规路径
            var index = 0;
            for (int x = 0; x < 13; x++)
            {
                //开头和第八格需要转弯
                switch (x)
                {
                    case 0:
                    case 7:
                        stepNow = Quaternion.Euler(0, 0, rotation) * stepNow;
                        break;
                }
                locatorNow += stepNow;
                //第七格保留,在第八格后绘制
                if (x == 6)
                    continue;
                var tempSet = sets[(i + index + 1) % size];
                current = new Floor(locatorNow, FloorType.Route, tempSet.Cell, tempSet.Player);//i因为定位到对应的组,Index+1是绘制当前格的偏移量
                if (x == 0)
                    pre.AddNext((f, m) => m.Group != f.Player.Group, current);
                else
                    pre.AddNext(null, current);
                if (x == 12)
                    edgeEnds.Add(current);
                current.Pre = pre;
                pre = current;
                if (x == 7)
                {
                    //出生点
                    var tempNow = locatorNow;
                    Vector2 tempStep = Quaternion.Euler(0, 0, rotation * i) * step;
                    tempNow += tempStep;
                    var startPoint = new Floor(tempNow, FloorType.StartPoint, set.Cell, set.Player, null, pre);
                    //卵点
                    tempNow += tempStep * 2 + (Vector2)(Quaternion.Euler(0, 0, rotation) * tempStep);
                    for (int y = 0; y < 4; y++)
                    {
                        tempStep = Quaternion.Euler(0, 0, rotation) * tempStep;
                        tempNow += tempStep;
                        var spawn = new Floor(tempNow, FloorType.Spawn, set.Cell, set.Player, null, startPoint);
                        new Unit(set.Player, spawn, set.Object);
                    }
                }
                index += 1;
            }
        }
        //连接首尾
        for (int i = 0; i < edgeStarts.Count; i++)
        {
            var preEnd = edgeEnds[i == 0 ? edgeStarts.Count - 1 : i - 1];
            edgeStarts[i].Pre = preEnd;
        }
        for (int i = 0; i < edgeEnds.Count; i++)
        {
            var nextStart = edgeStarts[i == edgeStarts.Count - 1 ? 0 : i + 1];
            edgeEnds[i].AddNext(null, nextStart);
        }
        #endregion

        #region 释放参数
        foreach (var set in sets)
        {
            set.Cell.SetActive(false);
            set.Object.SetActive(false);
        }
        #endregion

        ////绘制行走线路
        //var move = new MoveMent(Players[0].Group, 100);
        //current = Players[0].Units[0].Current;
        //while (move.Vatality>0)
        //{
        //    var next = current.GetNext(move);
        //    current.Display(next);
        //    current = next;
        //    move.Vatality--;
        //}

        //开始游戏
        CurrentPlayerIndex = -1;
        NextPlayer();
    }

    public Button Button_Roll;
    public Button Button_Next;
    Image Button_Roll_Image;
    Image Button_Next_Image;
    private void Awake()
    {
        Button_Roll_Image = Button_Roll.GetComponent<Image>();
        Button_Next_Image = Button_Next.GetComponent<Image>();
    }

    public Text Text_OutPut;
    List<string> Output = new List<string>();
    int limitLength = 10;
    void AddText(string text)
    {
        Output.Add(text);
        if (Output.Count() > limitLength)
        {
            Output.RemoveAt(0);
        }
        Text_OutPut.text = string.Join(System.Environment.NewLine, Output.ToArray());
    }
    void AddText(string text,params string[]args)
    {
        Output.Add(string.Format(text, args));
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
    void AddText(string title, Vector2 vector)
    {
        AddText(string.Format("{2}:{0},{1}", vector.x.ToString("F2"), vector.y.ToString("F2"), title));
    }

    int CurrentPlayerIndex = 0;
    //bool rollTurn = true;
    //bool moveTurn = false;

    enum TurnType
    {
        WaitForRoll,
        WaitForMove,
        WaitForFinish,
        Finished,
    }
    TurnType CurrentTurn;

    public void NextPlayer()
    {
        
        AddText("玩家{0}结束回合", CurrentPlayerIndex.ToString());
        CurrentPlayerIndex += 1;
        if (CurrentPlayerIndex == 4)
            CurrentPlayerIndex = 0;
        if (CurrentPlayerIndex == 0)
        {
            CurrentPlayerIndex = 0;
            CurrentTurn = TurnType.WaitForRoll;
            SwithButtons(CurrentTurn);
            AddText("等待玩家投掷");
        }
    }
    public void Roll()
    {
        var player = Players[CurrentPlayerIndex];
        player.Roll();
        AddText("你投出了" + player.Movement.Vatality);
        AddText("选择一个单位行动吧!");
        CurrentTurn = TurnType.WaitForMove;
        SwithButtons(CurrentTurn);//总是在更新内在内容后更新外在表现
    }
    private void SwithButtons(TurnType type)
    {
        switch (type)
        {
            case TurnType.WaitForRoll:
                Button_Roll.enabled = true;
                Button_Roll_Image.color = Color.white;
                Button_Next.enabled = true;
                Button_Next_Image.color = Color.white;
                break;
            case TurnType.WaitForMove:
                Button_Roll.enabled = false;
                Button_Roll_Image.color = Color.gray;
                Button_Next.enabled = true;
                Button_Next_Image.color = Color.white;
                break;
            case TurnType.WaitForFinish:
                Button_Roll.enabled = false;
                Button_Roll_Image.color = Color.gray;
                Button_Next.enabled = true;
                Button_Next_Image.color = Color.white;
                break;
            case TurnType.Finished:
                Button_Roll.enabled = false;
                Button_Roll_Image.color = Color.gray;
                Button_Next.enabled = false;
                Button_Next_Image.color = Color.gray;
                break;
            default:
                break;
        }
    }
    // 射线调试
    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(ray.origin, ray.direction);
    }
    private void Update()
    {
        if (CurrentPlayerIndex == 0)
        {
            //首先判断是否点击了鼠标左键
            if (CurrentTurn==TurnType.WaitForMove && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hit = Physics2D.Raycast(ray.origin, ray.direction);
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 1);
                if (hit.transform != null)
                {
                    AddText("检测到碰撞,碰撞体位置", hit.transform.position);
                    int index = 0;
                    foreach (var player in Players)
                    {
                        foreach (Unit unit in player.Units)
                        {
                            if (hit.collider.gameObject == unit.Obj)
                            {
                                if (player == CurrentPlayer)
                                {
                                    if (unit.CanMove(this))
                                    {
                                        CurrentTurn = TurnType.WaitForFinish;
                                        StartCoroutine(unit.Move(this));
                                    }
                                }
                                else
                                {
                                    AddText(string.Format("这是玩家{0}的单位", index));
                                    AddText("请选择一个我方单位");
                                    AddText("您的移动点数:" + CurrentPlayer.Movement.Vatality);
                                }
                            }
                        }
                        index++;
                    }
                }
                else
                {
                    AddText("未检测到碰撞,起始位置", ray.direction);
                    AddText("未检测到碰撞,当前鼠标位置", ray.origin);
                }
            }
        }
        else
        {
            //AI行动
            //TODO 暂无行动
            AIMoving();
        }
    }
    public void AIMoving()
    {
        AddText(string.Format("玩家{0}进行投掷", CurrentPlayerIndex));
        AddText(string.Format("玩家{0}进行移动", CurrentPlayerIndex));
        if (!IsFinished())
        {
            NextPlayer();
            AddText(string.Format("玩家{0}结束回合", CurrentPlayerIndex));
        }
    }
    public bool IsFinished()
    {
        foreach (var player in Players)
        {
            if (player.Score > 0)
            {
                //胜利效果
                return true;
            }
        }
        return false;
    }
    public void UpdateScore()
    {
        //TODO Display Score
    }

}

class KeyValue
{
    public KeyValue(int key, params string[] values)
    {
        Key = key;
        Values = values;
    }

    public int Key { set; get; }
    public string[] Values { set; get; }
}
class KeyValueCollection : List<KeyValue>
{
    string Seperator = "\r";
    public string GetValue(int key)
    {
        return string.Join(Seperator, this.FirstOrDefault(c => c.Key == key).Values);
    }
}

//private void InitGame2(Scene arg0, LoadSceneMode arg1)
//{
//    //GameObject[] targets = new GameObject[4] { TargetB, TargetG, TargetR, TargetY };
//    //GameObject[] objects = new GameObject[4] { ObjectB, ObjectG, ObjectR, ObjectY };
//    //GameObject[] cells = new GameObject[4] { CellB, CellG, CellR, CellY };

//    #region 参数准备
//    Color blue = Color.blue;
//    Color green = Color.green;
//    Color red = Color.red;
//    Color yellow = Color.yellow;
//    Color white = Color.white;
//    var bRender = Object.GetComponent<SpriteRenderer>();
//    bRender.color = blue;
//    var objectB = Instantiate(Object);
//    bRender.color = green;
//    var objectG = Instantiate(Object);
//    bRender.color = red;
//    var objectR = Instantiate(Object);
//    bRender.color = yellow;
//    var objectY = Instantiate(Object);
//    GameObject[] objects = new GameObject[4] { objectB, objectG, objectR, objectY };
//    var cRender = Cell.GetComponent<SpriteRenderer>();
//    cRender.color = blue;
//    var cellB = Instantiate(Cell);
//    cRender.color = green;
//    var cellG = Instantiate(Cell);
//    cRender.color = red;
//    var cellR = Instantiate(Cell);
//    cRender.color = yellow;
//    var cellY = Instantiate(Cell);
//    GameObject[] cells = new GameObject[4] { cellB, cellG, cellR, cellY };
//    cRender.color = white;
//    Vector2 start = new Vector2(0, 0);
//    Vector2 step = new Vector2(0.5f, 0);
//    int size = 4;
//    int radius = 8;
//    #endregion

//    #region 构建棋盘
//    //起始点
//    Vector2 now = start;
//    int rotation = -90;
//    for (int i = 0; i < size; i++)
//    {
//        //从起始点开始
//        now = start;
//        //通关路径
//        Vector2 stepNow = Quaternion.Euler(0, 0, rotation * i) * step;
//        for (int j = 0; j < radius - 2; j++)
//        {
//            now += stepNow;
//            Instantiate(cells[i], now, Quaternion.identity);
//        }
//        //转弯路径
//        now += stepNow;
//        Instantiate(cells[i], now, Quaternion.identity);
//        //常规路径
//        var index = 0;
//        for (int x = 0; x < 13; x++)
//        {
//            switch (x)
//            {
//                case 0:
//                    stepNow = Quaternion.Euler(0, 0, rotation) * stepNow;
//                    break;
//                case 7:
//                    stepNow = Quaternion.Euler(0, 0, rotation) * stepNow;
//                    break;
//            }
//            now += stepNow;
//            if (x == 6)
//            {
//                Instantiate(cells[i % size], now, Quaternion.identity);
//                #region 出生点
//                var bornLocator = now + stepNow + (Vector2)(Quaternion.Euler(0, 0, -rotation) * stepNow);
//                for (int y = 0; y < 4; y++)
//                {
//                    Vector2 bornStep = Quaternion.Euler(0, 0, -rotation * y) * stepNow;
//                    bornLocator += bornStep;
//                    Instantiate(cells[i], bornLocator, Quaternion.identity);
//                    Instantiate(objects[i], bornLocator, Quaternion.identity);
//                }
//                #endregion
//                continue;
//            }
//            Instantiate(cells[(index + i + 1) % size], now, Quaternion.identity);
//            index += 1;
//        }
//    }
//    #endregion

//    #region 释放参数
//    foreach (var item in objects)
//    {
//        item.SetActive(false);
//    }
//    foreach (var item in cells)
//    {
//        item.SetActive(false);
//    }
//    #endregion
//}