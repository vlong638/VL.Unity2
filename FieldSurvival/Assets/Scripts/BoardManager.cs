using Assets.VL.Scripts;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    float Height;
    float Width;

    public Canvas Canvas;

    //#region Set
    ///// <summary>
    ///// 行动栏
    ///// </summary>
    //public Area Actions;
    ///// <summary>
    ///// 移动区域
    ///// </summary>
    //public Area Grounds;
    ///// <summary>
    ///// 执行输出
    ///// </summary>
    //public Area Console;
    ///// <summary>
    ///// 状态栏
    ///// </summary>
    //public Area Status;
    ///// <summary>
    ///// 包裹
    ///// </summary>
    //public Area Package;
    //#endregion

    //public void Display()
    //{
    //    var endAction=Actions.Display(new Vector2(0,0));
    //    var endConsole= Console.Display(new Vector2(endAction.x, 0));
    //    var endGrounds = Grounds.Display(new Vector2(endConsole.x, 0));
    //    var endPackage = Package.Display(new Vector2(0, endAction.y));
    //    Status.Display(new Vector2(endPackage.y, endAction.y));
    //}

    private void Awake()
    {
        if (Canvas != null)
        {
            var c = Canvas.GetComponent<Canvas>().GetComponent<RectTransform>();
            Height = c.rect.width;
            Width = c.rect.height;
        }
    }

    private void OnGUI()
    {
        if (Canvas == null)
            return;

        if (GUI.Button(new Rect(0, 0, Width, Height), "return to Menu"))
        {
            SceneManager.LoadScene(Constraints.Scene_Menu);
        }
    }
    public void SetUpBoard()
    {
        //var canvas = new GameObject();
        //var t = canvas.AddComponent<RectTransform>();
        //var c = canvas.AddComponent<Canvas>();
        //canvas.AddComponent<CanvasScaler>();
        //canvas.AddComponent<GraphicRaycaster>();
        //c.renderMode = RenderMode.WorldSpace;
        //t.sizeDelta = new Vector2(Width, Height);

        //var image = new GameObject();
        //var t=image.AddComponent<RectTransform>();
        //image.AddComponent<CanvasRenderer>();
        //var i=image.AddComponent<Image>();
        //t.anchorMin = new Vector2(0, 0);
        //t.anchorMax = new Vector2(Width, Height);
        //i.color = new Color(88, 255, 88, 255);
        //image.SetActive(true);
        //Instantiate(image);
    }
}