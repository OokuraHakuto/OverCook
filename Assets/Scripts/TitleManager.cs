using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // スタートボタン
    public void StartGame()
    {
        //Select シーンをロード
        SceneManager.LoadScene("Select");
    }

    // オプションボタン
    public void OpenOptions()
    {
        Debug.Log("未実装");
    }

    // 終了ボタン
    public void QuitGame()
    {
        
        Application.Quit();
        
    }
}
