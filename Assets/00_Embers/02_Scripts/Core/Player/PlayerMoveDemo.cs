using Mirror;
using UnityEngine;

public class PlayerMoveDemo : MonoBehaviour
{
    public float moveSpeed = 5f;           // 이동 속도
    public float jumpHeight = 2f;         // 점프 높이
    public float gravity = -9.81f;        // 중력
    public CharacterController controller; // 캐릭터 컨트롤러 컴포넌트

    private Vector3 velocity;             // 플레이어의 속도
    private bool isGrounded;              // 땅에 닿았는지 확인

    void Start()
    {
        // CharacterController가 연결되어 있는지 확인
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (!NetworkClient.localPlayer)
            return;
        
        // 땅에 닿아 있는지 확인
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 땅에 붙어 있도록 약간의 힘만 유지
        }

        // WASD 입력 처리
        float moveX = Input.GetAxis("Horizontal"); // A, D 또는 왼쪽/오른쪽 화살표
        float moveZ = Input.GetAxis("Vertical");   // W, S 또는 위/아래 화살표
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 이동 처리
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 점프 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}