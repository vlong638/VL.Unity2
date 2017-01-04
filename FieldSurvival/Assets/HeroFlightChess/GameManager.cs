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
    public PlayerManager PlayerManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
        DontDestroyOnLoad(Instance);
    }

    public void Init(string playerName)
    {
        PlayerManager = new PlayerManager(playerName);
    }
    public void Clear(string playerName)
    {
        PlayerManager = null;
    }
}
