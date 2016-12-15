using Assets.VL.Scripts;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartExploration : MonoBehaviour
{
    public Text ErrorText;
    public InputField Input_PlayerName;


    public void Click()
    {
        var playerName = Input_PlayerName.text;
        if (string.IsNullOrEmpty(playerName))
        {
            ErrorText.text = "用户名称不可为空";
            return;
        }
        if (playerName.Length>16)
        {
            ErrorText.text = "用户名称不可长于10个字母";
            return;
        }

        GameManager.Instance.InitPlayer(playerName);
        SceneManager.LoadScene(Constraints.Scene_Exploration);
    }
}