using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 管理整个游戏的资料
/// 由登录环节初始化
/// 维护数据直至用户登出
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public Player Player;

    private void Awake()
    {
        //Instance
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
        DontDestroyOnLoad(Instance);

        //Events
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    public void InitPlayer(string playerName)
    {
        Player = new Player(playerName);
    }











    #region 场景加载事件
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
    } 
    #endregion


    private void Start()
    {
    }


    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

}
