//using Assets.VL.VL.Unity.Core.Utilities;
//using System;
//using UnityEngine;
//using UnityEngine.UI;

//public abstract class Card
//{
//    public float HeightWeight;
//    public float WidthWeight;
//    public float Height;
//    public float Width;

//    /// <summary>
//    /// 展现,returns End Vector
//    /// </summary>
//    public abstract Vector2 Display(Vector2 start);
//}
///// <summary>
///// 区域牌,用于地图地洞
///// </summary>
//public class AreaCard : Card
//{
//    public Image Floor;
//    public Unit Unit;

//    public override Vector2 Display(Vector2 start)
//    {
//        throw new NotImplementedException();
//    }
//}
///// <summary>
///// 行动牌,Action
///// </summary>
//public class ActionCard : Card
//{
//    public override Vector2 Display(Vector2 start)
//    {
//        throw new NotImplementedException();
//    }
//}
///// <summary>
///// 单元牌,Unit
///// 多个描述区域
///// 人物,物品,任务皆是一个单元
///// </summary>
//public class UnitCard : Card
//{
//    public Unit Unit;

//    public override Vector2 Display(Vector2 start)
//    {
//        throw new NotImplementedException();
//    }
//}
