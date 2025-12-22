using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour

{
    [Header("UI Components")]
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loadingTextPhanTram;
    public Image imageFill;

    [Header("Settings")]
    public string sceneToLoad = "Home"; // Tên scene muốn load

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Bắt đầu quá trình load ngay khi scene Loading hiện lên
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // 1. Bắt đầu load scene ngầm
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        // Ngăn scene tự động bật lên khi chưa xử lý xong (tùy chọn, ở đây ta để tự động cho đơn giản)
        // operation.allowSceneActivation = false; 

        // 2. Vòng lặp chạy liên tục cho đến khi load xong
        while (!operation.isDone)
        {
            // Unity trả về progress từ 0 -> 0.9. 
            // Ta chia cho 0.9 để giá trị chạy từ 0 -> 1 (cho thanh fill đầy)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Cập nhật UI Image Fill
            if (imageFill != null)
            {
                imageFill.fillAmount = progress;
            }

            // Cập nhật UI Text %
            if (loadingTextPhanTram != null)
            {
                // Format "F0" để không lấy số thập phân (VD: 55%)
                loadingTextPhanTram.text = (progress * 100).ToString("F0") + "%";
            }

            // Cập nhật text trạng thái (tùy chọn)
            if (loadingText != null)
            {
                loadingText.text = progress < 0.9f ? "Loading..." : "Almost done...";
            }

            // Đợi 1 frame rồi check tiếp
            yield return null;
        }
    }
}
