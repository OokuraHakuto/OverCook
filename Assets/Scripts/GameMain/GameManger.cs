using UnityEngine;
using TMPro; // TextMeshPro���g�����߂ɕK�v

public class GameManager : MonoBehaviour
{
    [Header("���Ԑݒ� (�b)")]
    public float roundTime = 180f; // 1���E���h�̎��ԁi��: 180�b = 3���j

    [Header("UI�֘A")]
    public TextMeshProUGUI timerText; // ���Ԃ�\������UI�e�L�X�g

    private float currentTime; // �c�莞��

    void Start()
    {
        currentTime = roundTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            // �����Ɏ��Ԃ��[���ɂȂ������̏����i�Q�[���I�[�o�[�Ȃǁj������
            Debug.Log("���Ԑ؂�I");
        }

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}