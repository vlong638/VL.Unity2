using Assets.VL.Scripts;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartExploration : MonoBehaviour
{
    public Text ErrorText;
    public InputField Input_PlayerName;


    #region 数据检验
    string playerName;
    public bool ValueCheck()
    {
        playerName = Input_PlayerName.text;
        if (string.IsNullOrEmpty(playerName))
        {
            ErrorText.text = "用户名称不可为空";
            return false;
        }
        if (playerName.Length > 16)
        {
            ErrorText.text = "用户名称不可长于10个字母";
            return false;
        }
        return true;
    } 
    #endregion

    public void StartGame_FieldSurvival()
    {
        if (!ValueCheck())
            return;

        //GameManager.Instance.Init(playerName);
        SceneManager.LoadScene(Constraints.Scene_FieldSurvival);
    }
    public void StartGame_HeroFlightChess()
    {
        if (!ValueCheck())
        {
            GameManager.Instance.Init("SecretV");
            SceneManager.LoadScene(Constraints.Scene_HeroFlightChess);
            return;
        }

        GameManager.Instance.Init(playerName);
        SceneManager.LoadScene(Constraints.Scene_HeroFlightChess);
    }

}