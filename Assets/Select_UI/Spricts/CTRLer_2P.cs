using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CTRLer_2P : MonoBehaviour
{
    //SoundMgr soundMgr = new SoundMgr();

    public GameObject cursor;
    int diff;
    public int chara; // ���ݑI�𒆂̃L�����ԍ��i1�`10�j

    [Header("�L�����N�^�[�v���t�@�u�̃��X�g")]
    public GameObject[] characterPrefabs;

    public Transform oya;
    Vector3 penguinSize = new Vector3(0.75f, 0.75f, 0.75f),
            nancharaSize = new Vector3(1.5f, 1.5f, 1.5f);


    // Start is called before the first frame update
    void Start()
    {
        chara = 2; // P2�̏����J�[�\���ʒu
        diff = 0;

        // ���X�g�Ƀv���t�@�u���ݒ肳��Ă��邩�m�F
        if (characterPrefabs.Length == 0)
        {
            Debug.LogError("CTRLer_2P��characterPrefabs���X�g�Ƀv���t�@�u���ݒ肳��Ă��܂���I");
            return;
        }

        DispChara(chara);
    }

    // Update is called once per frame
    void Update()
    {
        SelectChara();
        //SelectDiff(); // ���̃R�[�h�ŃR�����g�A�E�g����Ă����̂ŁA���̂܂�
    }

    public void SelectChara()
    {
        if (Input.GetKeyDown("up"))
        {
            chara--;
            if (chara <= 0) chara = characterPrefabs.Length; // 10���烊�X�g�̒����ɕύX

            DispChara(chara);
            //soundMgr.PlaySE(1);
        }
        if (Input.GetKeyDown("down"))
        {
            chara++;
            if (chara >= characterPrefabs.Length + 1) chara = 1; // 11����(���X�g�̒���+1)�ɕύX

            DispChara(chara);
            //soundMgr.PlaySE(1);
        }
    }

    public void SelectDiff()
    {
        // (���̊֐��͕ύX�Ȃ�)
        if (Input.GetKeyDown("left") && diff > 0)
        {
            cursor.transform.Translate(-10.6f, 33.5f, 0);
            diff--;
        }

        if (Input.GetKeyDown("right") && diff < 2)
        {
            cursor.transform.Translate(10.6f, -33.5f, 0);
            diff++;
        }
    }

    void DispChara(int num)
    {
        // 1. �����̃��f���������i�����OK�j
        foreach (Transform child in oya)
        {
            Destroy(child.gameObject);
        }

        int index = num - 1;
        if (index < 0 || index >= characterPrefabs.Length)
        {
            Debug.LogError("�I��ԍ����v���t�@�u���X�g�͈̔͊O�ł��I");
            return;
        }

        GameObject prefabToSpawn = characterPrefabs[index];

        if (prefabToSpawn != null)
        {
            // 2. ���{�p�̃��f���𐶐�����
            GameObject displayModel = Instantiate(prefabToSpawn, oya);

            // 3. �����������f���́u�ړ��X�N���v�g�v�𖳌�������
            PlayerController controller = displayModel.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false; // �����Update()���Ă΂�Ȃ��Ȃ�
            }

            // 4. ���łɁu�������Z�v���I�t�ɂ��Ă����i���S�̂��߁j
            Rigidbody rb = displayModel.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // �����G���W���œ����Ȃ�����
                rb.useGravity = false; // �d�͂��I�t�ɂ���
            }

            // 5. �X�P�[���𒲐�����
            if (num == characterPrefabs.Length) // nanchara
            {
                oya.transform.localScale = nancharaSize;
            }
            else
            {
                oya.transform.localScale = penguinSize;
            }
        }
    }

    // ���������ݑI��ł���v���t�@�u��Ԃ��֐�
    public GameObject GetCurrentSelectedPrefab()
    {
        int index = chara - 1;
        if (index >= 0 && index < characterPrefabs.Length)
        {
            return characterPrefabs[index];
        }
        return null; // �Y���Ȃ�
    }
}