using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;  // 玩家移动速度
    public float defaultMoveSpeed = 5f;  // 玩家移动速度
    public float rotationSpeed = 100f;  // 玩家旋转速度
    public float rotationSpeedY = 5f;  // 玩家旋转速度
    public Camera playerCamera;  // 玩家摄像机
    private Rigidbody rb;  // 玩家刚体
    public Transform armTransform;  // 手臂的 Transform
    public float mouseSensitivity = 2f;  // 鼠标灵敏度
    private float rotationX = 0f;  // 当前水平旋转
    private float rotationY = 0f;  // 当前垂直旋转

    // 设置旋转的最大和最小限制
    public float minRotationX = -30f;
    public float maxRotationX = 30f;
    public float minRotationY = -40f;
    public float maxRotationY = 40f;

    public float minCameraAngle = 0f;  // 最小视角（0度）
    public float maxCameraAngle = 90f; // 最大视角（90度）
    private Vector3 lastMoveDirection = Vector3.zero;  // 记录玩家上次的移动方向\
    public bool vtgU = true;
    public bool vtgD = true;

    void SpeedControl()
    {
       // Vector3 flattVel = new Vector3(rb.velocity)
    }

    void Start()
    {
        // 获取玩家的刚体组件
        rb = GetComponent<Rigidbody>();
   

        // 隐藏鼠标光标并锁定其到屏幕中心
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update 是每帧调用一次
    void Update()
    {
        WASD();

        // 获取鼠标输入来旋转摄像机和手臂
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;  // 水平鼠标移动
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;  // 垂直鼠标移动

        // 更新玩家的旋转，限制垂直旋转的范围以防止翻转
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);  // 限制垂直旋转角度
        rotationY += mouseX;
        rotationY = Mathf.Clamp(rotationY, minRotationY, maxRotationY);  // 限制水平旋转角度

        // 旋转手臂（只控制水平旋转）
        armTransform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // 根据 Q 和 E 键旋转玩家（左旋和右旋）
        if (Input.GetKey(KeyCode.Q) || rotationY == -40)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0));  // 使用玩家自身轴心旋转（左旋）
        }
        if (Input.GetKey(KeyCode.E) || rotationY == 40)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0));  // 使用玩家自身轴心旋转（右旋）
        }
        if (Input.GetKey(KeyCode.Q) && rotationY == -40)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -rotationSpeed *2* Time.deltaTime, 0));  // 使用玩家自身轴心旋转（左旋）
        }
        if (Input.GetKey(KeyCode.E) && rotationY == 40)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotationSpeed*2 * Time.deltaTime, 0));  // 使用玩家自身轴心旋转（右旋）
        }
        verticalRotation();
        //0-360 FLOAT
        //305-360 U 0-55
        //x<= 305 = 305
        //x>= 55 = 55
        float threshold = 2f; // Adjust as needed for the precision

        // Get the current X rotation (pitch) from the Euler angles
        float currentRotationX = playerCamera.transform.eulerAngles.x;

        // Check if the rotation is approximately 305f
        if (Mathf.Abs(currentRotationX - 305f) < threshold)
        {
            Debug.Log("111");
            //playerCamera.transform.rotation = Quaternion.Euler(-52, 0, 0f);
            vtgU = false;
            
        }
        // Check if the rotation is approximately 55f
        else if (Mathf.Abs(currentRotationX - 55f) < threshold)
        {
            Debug.Log("222");
            // playerCamera.transform.rotation = Quaternion.Euler(52, 0, 0f);
            vtgD = false;
            
        }
        else
        {
            vtgU = true;
            vtgD = true;
        }
        Debug.Log(playerCamera.transform.eulerAngles.x);
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            //moveSpeed = 0.5f;
            //rb.drag = dragOnCollision;  // 增加拖拽值，减缓玩家速度
        }
    }

    private void OnTriggerExit(Collider other)
    {
        moveSpeed = 5f;
    }

    bool IsCollidingWithWall(Vector3 movement)
    {
        // 从玩家的位置向移动方向发射一条射线
        RaycastHit hit;
        if (Physics.Raycast(rb.position, movement.normalized, out hit, movement.magnitude+1f))
        {
            // 如果射线与物体发生碰撞，返回 true
            // 检查碰撞体的 Layer 是否是 Default（墙体）
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                return true;
            }
        }
        return false;
    }
    void verticalRotation()
    {
        float hintUp, hintDown;
        // 限制摄像机的上下旋转，防止超出范围
        if ((minRotationX - maxRotationX ) < 50f)
        {
            
            if ( rotationX == minRotationX && minRotationX >= -50 && vtgU)
            {
                // 摄像机朝上旋转（向下看）

                playerCamera.transform.Rotate(-rotationSpeed * Time.deltaTime, 0, 0);
                
                
                //playerCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
                minRotationX -= 0.1f ; 
                maxRotationX -= 0.4f;
            }
            if (rotationX == maxRotationX && maxRotationX <= 50 && vtgD)
            {
                // 摄像机朝下旋转（向上看）
                playerCamera.transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
                //playerCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
                minRotationX += 0.4f;
                
                maxRotationX += 0.1f ;
            }
        }
        if (minRotationX >= maxRotationX)
        {
            minRotationX = -30f;
            maxRotationX = 30f;
        }

        
    }

    void verticalRotation2()
    {
        
    } 
    void WASD()
    {
        // 获取 WASD 键的输入（水平和垂直方向）
        float horizontal = Input.GetAxis("Horizontal"); // A/D 或左右箭头键
        float vertical = Input.GetAxis("Vertical");     // W/S 或上下箭头键

        // 获取摄像机的朝向来控制玩家的移动方向
        Vector3 forward = playerCamera.transform.forward; // 摄像机前方方向
        Vector3 right = playerCamera.transform.right;     // 摄像机右方方向

        // 确保玩家只在水平面上移动，不改变高度
        forward.y = 0;
        right.y = 0;

        // 归一化方向向量
        forward.Normalize();
        right.Normalize();

        // 计算玩家的移动向量
        Vector3 movement = (forward * vertical + right * horizontal) * moveSpeed * Time.deltaTime;
        //Running
        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 7.5f;
        }
        else
        {
            moveSpeed = 5;
        }

        // 使用射线检测玩家是否与墙体发生碰撞，并限制其移动
        if (!IsCollidingWithWall(movement))
        {
            // 如果没有碰撞到墙体，允许移动
            rb.MovePosition(rb.position + movement);

        }
        else
        {
            // 保持玩家在当前位置s
            rb.MovePosition(rb.position);
        }
    }
}
