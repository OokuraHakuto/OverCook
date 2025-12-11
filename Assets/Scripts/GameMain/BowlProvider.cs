using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlProvider : MonoBehaviour, IInteracttable
{
    public GameObject bowlPrefab; // 生成するボウルのプレハブ

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        // プレイヤーが手ぶらなら
        if (player.heldItem == null)
        {
            // 1. ボウルを生成
            // ★ポイント：生成する場所を最初から「プレイヤーの手の位置」にしておくと、一瞬変な場所に映るのを防げます
            // （player.transform.position でもいいですが、手元にパッと出るようにします）
            GameObject newBowl = Instantiate(bowlPrefab, player.transform.position, Quaternion.identity);

            newBowl.name = bowlPrefab.name.Replace("(Clone)", "").Trim();

            // 2. プレイヤーに持たせる
            // ★修正点：HoldItem（生成して持つ）ではなく、PickUpItem（今あるこれを持つ）に変える！
            player.PickUpItem(newBowl);

            // 3. 位置合わせ（PickUpItem内でやってなければ念のため）
            newBowl.transform.localPosition = Vector3.zero;
            newBowl.transform.localRotation = Quaternion.identity;

            // 4. サイズ調整
            ItemSettings settings = newBowl.GetComponent<ItemSettings>();
            if (settings != null)
            {
                newBowl.transform.localScale = settings.onPlayerScale;
            }

            Debug.Log("新しいボウルを取り出しました");
        }
    }

    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 7.0f;
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDistance) { minDistance = dist; closest = p; }
        }
        return closest;
    }
}