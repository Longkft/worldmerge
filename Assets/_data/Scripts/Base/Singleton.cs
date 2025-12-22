using UnityEngine;

// <T> ở đây bắt buộc phải là 1 Component
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    // Biến check xem game đang tắt chưa để tránh lỗi tạo object khi quit
    private static bool _isQuitting = false;

    public static T Instance
    {
        get
        {
            // Nếu game đang tắt thì trả về null luôn, không tạo mới nữa
            if (_isQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            if (_instance == null)
            {
                // 1. Tìm xem trong scene có sẵn chưa
                _instance = FindObjectOfType<T>();

                // 2. Nếu chưa có -> Tự tạo mới
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name; // Đặt tên GameObject theo tên Class
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // Xử lý trùng lặp (nếu lỡ có 2 cái trong scene)
        if (_instance == null)
        {
            _instance = this as T;

            // Nếu muốn giữ qua các màn chơi
            if (ShouldDontDestroyOnLoad())
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != this)
        {
            // Nếu đã có 1 thằng khác làm trùm rồi thì thằng này là fake -> hủy
            Destroy(gameObject);
        }
    }

    // Hàm ảo để các class con có thể override nếu KHÔNG muốn DontDestroyOnLoad
    protected virtual bool ShouldDontDestroyOnLoad()
    {
        return true;
    }

    // Khi game tắt, set cờ để chặn việc tạo instance mới
    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}