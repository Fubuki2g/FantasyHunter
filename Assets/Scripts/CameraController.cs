using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // カメラの操作スピード
    private Vector3 speed;

    // プレイヤー追従
    public GameObject TargetObject; // プレイヤーオブジェクト
    public float Height = 1.5f; // カメラの高さのオフセット
    public float Distance = 10.0f; // カメラとのオフセット
    public float RotAngle = 0.0f; // 水平方向(横)方向のカメラ角度
    public float HeightAngle = 10.0f; // 垂直(縦)方向のカメラの角度
    public float dis_min = 5f; // 見上げた時のカメラ距離(任意)
    public float dis_mdl = 10.0f; // 通常のカメラ距離(任意)
    private Vector3 nowPos; // 現在のプレイヤー位置
    public float nowRotAngle; // 現在の水平方向(横)方向のカメラ角度
    public float nowHeightAngle; // 現在の垂直(縦)方向のカメラ角度

    // 減衰挙動
    public bool EnableAtten = true;
    public float AttenRate = 3.0f;
    public float ForwardDistance = 2.0f;
    private Vector3 addForward;
    private Vector3 prevTargetPos;
    public float RotAngleAttenRate = 5.0f;
    public float AngleAttenRate = 1.0f;

    void Start()
    {
        nowPos = TargetObject.transform.position;
    }

    void LateUpdate()
    {
        RotAngle -= speed.x * Time.deltaTime * 50.0f;
        HeightAngle += speed.z * Time.deltaTime * 20.0f;
        HeightAngle = Mathf.Clamp(HeightAngle, -40.0f, 60.0f);
        Distance = Mathf.Clamp(Distance, 5.0f, 40.0f);
        // 減衰
        if (EnableAtten)
        {
            var target = TargetObject.transform.position;

            var halfPos = (TargetObject.transform.position + target) / 2;
            var deltaPos = halfPos - prevTargetPos;
            prevTargetPos = halfPos;
            deltaPos *= ForwardDistance;

            addForward += deltaPos * Time.deltaTime * 20.0f;
            addForward = Vector3.Lerp(addForward, Vector3.zero, Time.deltaTime * AttenRate);

            nowPos = Vector3.Lerp(nowPos, halfPos + Vector3.up * Height + addForward, Mathf.Clamp01(Time.deltaTime * AttenRate));
        }
        else nowPos = TargetObject.transform.position + Vector3.up * Height;

        if (EnableAtten) nowRotAngle = Mathf.Lerp(nowRotAngle, RotAngle, Time.deltaTime * RotAngleAttenRate);
        else nowRotAngle = RotAngle;

        if (EnableAtten) nowHeightAngle = Mathf.Lerp(nowHeightAngle, HeightAngle, Time.deltaTime * RotAngleAttenRate);
        else nowHeightAngle = RotAngle;

            if (HeightAngle > 30)
            {
                Distance = Mathf.Lerp(Distance, 5.0f * HeightAngle / 30.0f, Time.deltaTime);
            }
            else if (HeightAngle <= 30 && HeightAngle >= -3)
            {
                Distance = Mathf.Lerp(Distance, 5.0f, Time.deltaTime);
            }
            else if (HeightAngle < -3)
            {
                Distance = Mathf.Lerp(Distance, dis_min, Time.deltaTime);
            }
        // }

        var deg = Mathf.Deg2Rad;
        var cx = Mathf.Sin(nowRotAngle * deg) * Mathf.Cos(nowHeightAngle * deg) * Distance;
        var cz = -Mathf.Cos(nowRotAngle * deg) * Mathf.Cos(nowHeightAngle * deg) * Distance;
        var cy = Mathf.Sin(nowHeightAngle * deg) * Distance;
        transform.position = nowPos + new Vector3(cx, cy, cz);

        var rot = Quaternion.LookRotation((nowPos - transform.position).normalized);
        if (EnableAtten) transform.rotation = rot;
        else transform.rotation = rot;

    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        speed = new Vector3(context.ReadValue<Vector2>().x,0f, context.ReadValue<Vector2>().y);
    }

}

