using Assets.VL.VL.Unity.Core.Utilities;
using System.Collections.Generic;

/// <summary>
/// 玩家状态
/// </summary>
public class Player : Unit
{
    public List<Item> Items;
    public CombatStatus CombatStatus;
    public ManageStatus ManageStatus;
    public EducateStaus EducateStaus;

    public Player(string name, string description = "") : base(name, description)
    {
        Items = new List<Item>();
        CombatStatus = new CombatStatus()
        {
            HP = RandomHelper.Next(100, 120),
            Attack = RandomHelper.Next(5, 10),
            Defense = RandomHelper.Next(1, 5),
        };
        ManageStatus = new ManageStatus()
        {
            Hunting = 1,
            HuntingExperience = 0,
            Farming = 1,
            FarmingExperience = 0,
        };
        EducateStaus = new EducateStaus()
        {
            Level = 1,
            Experience = 0,
        };
    }
}

#region 状态
/// <summary>
/// 战斗系统,个人状态
/// </summary>
public class CombatStatus
{
    public int HP;
    public int Attack;
    public int Defense;
}
/// <summary>
/// 经营系统,个人状态
/// </summary>
public class ManageStatus
{
    public int Hunting;
    public int HuntingExperience;
    public int Farming;
    public int FarmingExperience;
}
/// <summary>
/// 成长系统
/// </summary>
public class EducateStaus
{
    public int Level;
    public int Experience;
}
#endregion

#region 对象
/// <summary>
/// 道具
/// </summary>
public class Item : Unit
{
    public Item(string name, string description) : base(name, description)
    {
    }
}
/// <summary>
/// 可战斗单位
/// </summary>
public class FightableUnit : Unit
{
    public CombatStatus CombatStatus;

    public FightableUnit(int hp, int attack, int defense, string name, string description) : base(name, description)
    {
        CombatStatus = new CombatStatus()
        {
            HP = hp,
            Attack = attack,
            Defense = defense,
        };
    }
}
/// <summary>
/// 单位
/// </summary>
public class Unit
{
    public string Name;
    public string Description;

    public Unit(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
#endregion