#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations; // これが必要

public class AnimatorFixer : EditorWindow
{
    [MenuItem("Tools/Fix All Transitions")]
    public static void FixTransitions()
    {
        // 選択しているAnimatorControllerを取得
        AnimatorController controller = Selection.activeObject as AnimatorController;

        if (controller == null)
        {
            Debug.LogError("AnimatorControllerを選択してから実行してください！");
            return;
        }

        // すべてのレイヤーをループ
        foreach (var layer in controller.layers)
        {
            // ステートマシン内のすべてのステートをループ
            FixStateMachine(layer.stateMachine);
        }

        Debug.Log("すべてのトランジションを修正しました！ (HasExitTime=OFF, Duration=0)");

        // 保存（これをしないと元に戻ることがある）
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
    }

    private static void FixStateMachine(AnimatorStateMachine stateMachine)
    {
        // 各ステートからの遷移を修正
        foreach (var state in stateMachine.states)
        {
            foreach (var transition in state.state.transitions)
            {
                ModifyTransition(transition);
            }
        }

        // AnyStateからの遷移も修正
        foreach (var transition in stateMachine.anyStateTransitions)
        {
            ModifyTransition(transition);
        }

        // サブステートマシンがあれば再帰的に処理
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            FixStateMachine(subStateMachine.stateMachine);
        }
    }

    private static void ModifyTransition(AnimatorStateTransition transition)
    {
        // ★ここで設定を一括変更しています
        transition.hasExitTime = false; // チェックを外す
        transition.duration = 0f;       // 遷移時間を0にする
        transition.exitTime = 0f;       // 念のため
    }
}
#endif