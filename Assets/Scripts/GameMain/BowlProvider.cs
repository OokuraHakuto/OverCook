using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BowlProvider : MonoBehaviour, IInteracttable
{
    [Header("設定")]
    public GameObject bowlPrefab;

    public void Interact()
    {
        PlayerController player = FindClosestPlayer();
        if (player == null) return;

        if (player.heldItem == null)
        {
            GameObject newBowl = Instantiate(bowlPrefab, player.transform.position, Quaternion.identity);
            newBowl.name = bowlPrefab.name.Replace("(Clone)", "").Trim();

            player.PickUpItem(newBowl);

            ItemSettings settings = newBowl.GetComponent<ItemSettings>();
            if (settings != null)
            {
                newBowl.transform.localScale = settings.onPlayerScale;
                // ★変更：ItemSettingsの値を使うように変更
                newBowl.transform.localPosition = settings.holdPositionOffset;
            }
            else
            {
                newBowl.transform.localPosition = Vector3.zero;
            }

            newBowl.transform.localRotation = Quaternion.identity;
            Debug.Log("新しいボウルを取り出しました");
        }
    }

    // FindClosestPlayer は省略しますが、他と同じです
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        PlayerController closest = null;
        float minDistance = 7.0f;
        foreach (var p in players) { float dist = Vector3.Distance(transform.position, p.transform.position); if (dist < minDistance) { minDistance = dist; closest = p; } }
        return closest;
    }
}