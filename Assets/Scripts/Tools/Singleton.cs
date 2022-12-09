// 單例模式class，繼承用
public abstract class Singleton<T> where T : new() {
    private static readonly object syslock = new object();  // 安全鎖標記
    private static T _instance;                             // 實例
    public static T instance {
        get {
            if (_instance == null) {
                // 用安全鎖鎖住，避免new動作完成之前又再被呼叫
                lock (syslock) {
                    if (_instance == null) {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}