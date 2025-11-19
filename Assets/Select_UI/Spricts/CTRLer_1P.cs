using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CTRLer_1P : MonoBehaviour
{
    //SoundMgr soundMgr = new SoundMgr();

    /*public GameObject cursor;*/
    int diff;
    public int chara; // 現在選択中のキャラ番号（1～10）

    [Header("キャラクタープレファブのリスト")]
    public GameObject[] characterPrefabs;

    public Transform oya;
    Vector3 penguinSize = new Vector3(0.75f, 0.75f, 0.75f),
            nancharaSize = new Vector3(0.75f, 0.75f, 0.75f);


    // Start is called before the first frame update
    void Start()
    {
        chara = 1; // P1の初期カーソル位置
        diff = 0;

        // リストにプレファブが設定されているか確認
        if (characterPrefabs.Length == 0)
        {
            Debug.LogError("CTRLer_1PのcharacterPrefabsリストにプレファブが設定されていません！");
            return;
        }

        DispChara(chara);
    }

    // Update is called once per frame
    void Update()
    {
        SelectChara();
        SelectDiff();
    }

    public void SelectChara()
    {
        if (Input.GetKeyDown("w")) // P1のキー（W）はそのまま
        {
            chara--;
            if (chara <= 0) chara = characterPrefabs.Length; // 10からリストの長さに変更

            DispChara(chara);
            //soundMgr.PlaySE(1);
        }
        if (Input.GetKeyDown("s")) // P1のキー（S）はそのまま
        {
            chara++;
            if (chara >= characterPrefabs.Length + 1) chara = 1; // 11から(リストの長さ+1)に変更

            DispChara(chara);
            //soundMgr.PlaySE(1);
        }
    }

    public void SelectDiff()
    {
        // (この関数は変更なし。P1のA/Dキーを使用)
        if (Input.GetKeyDown("a") && diff > 0)
        {
            /*cursor.transform.Translate(-10.6f, 33.5f, 0);*/
            diff--;
            Debug.Log("Aおうか " + diff);
        }

        if (Input.GetKeyDown("d") && diff < 2)
        {
            /*cursor.transform.Translate(10.6f, -33.5f, 0);*/
            diff++;
            Debug.Log("Dおうか " + diff);
        }
    }

    void DispChara(int num)
    {
        // 1. 既存のモデルを消す（これはOK）
        foreach (Transform child in oya)
        {
            Destroy(child.gameObject);
        }

        int index = num - 1;
        if (index < 0 || index >= characterPrefabs.Length)
        {
            Debug.LogError("選択番号がプレファブリストの範囲外です！");
            return;
        }

        GameObject prefabToSpawn = characterPrefabs[index];

        if (prefabToSpawn != null)
        {
            // 2. 見本用のモデルを生成する
            GameObject displayModel = Instantiate(prefabToSpawn, oya);

            // 3. 生成したモデルの「移動スクリプト」を無効化する
            PlayerController controller = displayModel.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false; // これでUpdate()が呼ばれなくなる
            }

            // 4. ついでに「物理演算」もオフにしておく（安全のため）
            Rigidbody rb = displayModel.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // 物理エンジンで動かなくする
                rb.useGravity = false; // 重力をオフにする
            }

            // 5. スケールを調整する
            if (prefabToSpawn.name.Contains("nanchara"))
            {
                oya.transform.localScale = nancharaSize; // デカくする

                displayModel.transform.localPosition = new Vector3(0, 1.0f, 0);
            }
            else
            {
                oya.transform.localScale = penguinSize; // 普通のサイズ
            }
        }
    }

    // 自分が現在選んでいるプレファブを返す関数
    public GameObject GetCurrentSelectedPrefab()
    {
        int index = chara - 1;
        if (index >= 0 && index < characterPrefabs.Length)
        {
            return characterPrefabs[index];
        }
        return null; // 該当なし
    }
}