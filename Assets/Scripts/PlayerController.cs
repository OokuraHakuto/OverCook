using UnityEngine;
// �����Łi�r���h���j�ł̂�Input System��ǂݍ���
#if !UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    // --- �����̃��[�h�ŋ��ʂ��Ďg���ϐ� ---
    [Header("�ړ����x")]
    public float moveSpeed = 5f; // �L�����N�^�[�̈ړ����x
    [Header("��]�̑���")]
    public float rotateSpeed = 10f; // �L�����N�^�[�̉�]�̒Ǐ]���x

    private Rigidbody rb; // �������Z���Ǘ�����Rigidbody�R���|�[�l���g
    private Vector2 moveInput; // �ړ����́iX, Y�j��ێ�����ϐ�


    //#if false�ɂ���Ɗ����ł̕��̃f�o�b�O���ł���s
#if UNITY_EDITOR // ������ Unity�G�f�B�^�Ŏ��s���Ă��鎞�����A���̕������L���ɂȂ� ������

    [Header("�y�f�o�b�O�p�z�v���C���[�ԍ�")]
    public int playerID = 1; // �y�G�f�B�^��p�z�v���C���[�ԍ��i1��2�j���C���X�y�N�^�Ŏw��

    // �y�G�f�B�^��p�z���t���[���A�L�[�{�[�h���͂𒼐ڃ`�F�b�N����
    void Update()
    {
        // �܂����͂����Z�b�g
        moveInput = Vector2.zero;

        // playerID�ɉ����āAWASD�����L�[�̓��͂��󂯎��
        if (playerID == 1)
        {
            if (Input.GetKey(KeyCode.W)) { moveInput.y = 1; }
            if (Input.GetKey(KeyCode.S)) { moveInput.y = -1; }
            if (Input.GetKey(KeyCode.A)) { moveInput.x = -1; }
            if (Input.GetKey(KeyCode.D)) { moveInput.x = 1; }
        }
        else if (playerID == 2)
        {
            if (Input.GetKey(KeyCode.UpArrow)) { moveInput.y = 1; }
            if (Input.GetKey(KeyCode.DownArrow)) { moveInput.y = -1; }
            if (Input.GetKey(KeyCode.LeftArrow)) { moveInput.x = -1; }
            if (Input.GetKey(KeyCode.RightArrow)) { moveInput.x = 1; }
        }
    }

#else // ������ �Q�[�����r���h�������i�����Łj�����A���̕������L���ɂȂ� ������

    // �y�����ŁzPlayer Input�R���|�[�l���g����C�x���g�Ƃ��ČĂяo�����
    public void OnMove(InputAction.CallbackContext context)
    {
        // �p�b�h��L�[�{�[�h����̓��͂�Vector2�Ƃ��Ď󂯎��
        moveInput = context.ReadValue<Vector2>();
    }

#endif // ������ �����Ŗ��߂͏I��� ������


    // --- �����̃��[�h�ŋ��ʂ��Ďg������ ---

    // �Q�[���J�n���Ɉ�x�����Ă΂�A�������g�̃R���|�[�l���g���擾����
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // �������Z�̃^�C�~���O�ň��Ԋu�ŌĂ΂��
    void FixedUpdate()
    {
        // 2D�̓��͂�3D��Ԃ̈ړ��x�N�g���ɕϊ�����
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        // Rigidbody�̑��x���X�V���ăL�����N�^�[�𕨗��I�ɓ������i���K�����Ď΂߈ړ��΍�j
        rb.velocity = movement.normalized * moveSpeed;

        // �L�����N�^�[�̌�������͕����ɍ��킹��i���͂����鎞�����j
        if (movement != Vector3.zero)
        {
            // ���������������v�Z
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            // ���݂̌�������ڕW�̌����ցA�w�肵�������Ŋ��炩�ɉ�]������
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}