using Mirror;
using UnityEngine;

namespace STARTING
{
    public class CameraFollow : NetworkBehaviour
    {
        public Transform cameraTarget; // 카메라가 따라갈 대상
        public Vector3 offset = new Vector3(0, 5, -10); // 카메라의 위치 오프셋
        public float smoothSpeed = 0.125f; // 카메라 움직임의 부드러움

        public Camera playerCamera;

        void Start()
        {
            // 로컬 플레이어인지 확인
            if (!isLocalPlayer)
            {
                return; // 로컬 플레이어가 아니라면 카메라를 비활성화
            }

            if (playerCamera == null)
            {
                GameObject cameraObj = new GameObject("PlayerCamera");
                playerCamera = cameraObj.AddComponent<Camera>();
            }

            // 카메라가 이 플레이어를 따라가도록 설정
            playerCamera.transform.SetParent(null); // 독립적으로 이동
            playerCamera.transform.position = cameraTarget.position + offset;
        }

        void Update()
        {
            // 로컬 플레이어가 아니면 동작하지 않음
            if (!isLocalPlayer || playerCamera == null) return;

            // 목표 위치 계산
            Vector3 desiredPosition = cameraTarget.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(playerCamera.transform.position, desiredPosition, smoothSpeed);

            // 카메라 위치 업데이트
            playerCamera.transform.position = smoothedPosition;

            // 카메라가 대상 쪽을 바라보게 설정
            playerCamera.transform.LookAt(cameraTarget);
        }
    }
}