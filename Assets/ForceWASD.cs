using UnityEngine;

public class ForceWASD : MonoBehaviour
{
    void Awake()
    {
        // 在游戏启动的最早时刻，强制修改输入管理器的设置
        // 注意：这需要在任何可能重置设置的脚本之前执行
        Debug.Log("ForceWASD: 强制绑定 WASD 键位");
        
        // 如果你的项目使用 Input.GetAxis，通常不需要额外操作
        // 但如果它使用 KeyCode，可能需要在这里做映射
    }
}