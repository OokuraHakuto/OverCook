using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("最初に選択状態にしたいボタン")]
    public GameObject firstSelectButton;

    void Start()
    {
        // ゲーム開始時に、指定したボタンを強制的に選択状態にする
        if (firstSelectButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectButton);
        }
    }

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
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
