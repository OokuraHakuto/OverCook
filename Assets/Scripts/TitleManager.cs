using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // �X�^�[�g�{�^��
    public void StartGame()
    {
        //Select �V�[�������[�h
        SceneManager.LoadScene("Select");
    }

    // �I�v�V�����{�^��
    public void OpenOptions()
    {
        Debug.Log("������");
    }

    // �I���{�^��
    public void QuitGame()
    {
        
        Application.Quit();
        
    }
}
