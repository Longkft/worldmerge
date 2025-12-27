using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayModeMenu
{
    // Tên key để lưu setting vào EditorPrefs
    private const string MenuPath = "Tools/Always Start From Scene 0";

    // Biến kiểm tra trạng thái hiện tại
    private static bool _isEnabled;

    static PlayModeMenu()
    {
        // Load trạng thái cũ đã lưu
        _isEnabled = EditorPrefs.GetBool(MenuPath, false);
        SetPlayModeStartScene(_isEnabled);

        // Đăng ký event để update dấu tick trên menu
        EditorApplication.delayCall += () => {
            Menu.SetChecked(MenuPath, _isEnabled);
        };
    }

    [MenuItem(MenuPath)]
    private static void ToggleAction()
    {
        _isEnabled = !_isEnabled;
        EditorPrefs.SetBool(MenuPath, _isEnabled); // Lưu lại setting
        Menu.SetChecked(MenuPath, _isEnabled);     // Cập nhật dấu tick
        SetPlayModeStartScene(_isEnabled);
    }

    private static void SetPlayModeStartScene(bool enable)
    {
        if (enable)
        {
            // Lấy Scene 0 trong Build Settings
            var path = EditorBuildSettings.scenes[0].path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            EditorSceneManager.playModeStartScene = sceneAsset;
            Debug.Log("Play Mode: Đã KHÓA vào Scene 0 (Loading).");
        }
        else
        {
            // Trả về null để Unity chạy scene hiện tại đang mở
            EditorSceneManager.playModeStartScene = null;
            Debug.Log("Play Mode: Chạy Scene hiện tại.");
        }
    }
}