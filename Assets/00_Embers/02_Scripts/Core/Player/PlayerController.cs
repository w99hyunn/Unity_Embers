using Mirror;
using UnityEngine;

namespace NOLDA
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        public UnityEngine.InputSystem.PlayerInput playerInput;
        public CharacterController controller;
        public PlayerInput input;
        public GameObject mainCamera;
        public Player player;
        public PlayerSkillHandler skillHandler;

        [Header("Player")]
        [Tooltip("캐릭터의 이동 속도(m/s)")]
        public float MoveSpeed = 2.0f;

        [Tooltip("캐릭터의 달리기 속도(m/s)")]
        public float SprintSpeed = 5.335f;

        [Tooltip("캐릭터가 이동 방향을 향해 회전하는 속도")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("가속 및 감속")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("캐릭터가 점프할 수 있는 높이")]
        public float JumpHeight = 1.2f;

        [Tooltip("캐릭터가 사용하는 중력값. 엔진 기본값은 -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("다시 점프하기 전에 필요한 대기 시간. 0f로 설정하면 즉시 다시 점프 가능")]
        public float JumpTimeout = 0.50f;

        [Tooltip("낙하 상태로 진입하기 전에 필요한 대기 시간. 계단을 내려갈 때 유용")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("캐릭터가 지면에 있는지 여부. CharacterController의 내장 지면 체크와는 별개")]
        public bool Grounded = true;

        [Tooltip("거친 지면에 유용")]
        public float GroundedOffset = -0.14f;

        [Tooltip("지면 체크의 반경. CharacterController의 반경과 일치해야 함")]
        public float GroundedRadius = 0.28f;

        [Tooltip("캐릭터가 지면으로 사용하는 레이어")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("카메라가 따라갈 Cinemachine Virtual Camera의 타겟")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("카메라를 위로 얼마나 움직일 수 있는지(각도)")]
        public float TopClamp = 70.0f;

        [Tooltip("카메라를 아래로 얼마나 움직일 수 있는지(각도)")]
        public float BottomClamp = -30.0f;

        [Tooltip("카메라 위치를 미세 조정할 때 유용한 추가 각도")]
        public float CameraAngleOverride = 0.0f;

        [Header("NPC 대화 중")]
        public bool isNpcTalk = false;

        [Header("스킬 사용 중")]
        public bool isUseSkill = false;

        #region private variables
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // 플레이어
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // 타임아웃 델타타임
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // 애니메이션 ID
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        #endregion

        private void Start()
        {
            if (!isLocalPlayer)
            {
                mainCamera.SetActive(false);
                playerInput.enabled = false;
            }
            else
            {
                playerInput.enabled = true;
            }

            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            AssignAnimationIDs();

            // 시작 시 타임아웃 초기화
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (!isLocalPlayer || isUseSkill || isNpcTalk)
                return;

            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();

            //청크 로드
            //TODO: Update에서 프레임단위로 호출되는 코드이므로, 좋은 방법을 생각해서 부하 줄이기
            Singleton.Map.UpdateChunks(this.transform.position).Forget();
        }

        private void LateUpdate()
        {
            if (!isLocalPlayer || player.lockCursor)
                return;

            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // 오프셋이 적용된 구체 위치 설정
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // 캐릭터를 사용하는 경우 애니메이터 업데이트
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // 입력이 있고 카메라 위치가 고정되지 않은 경우
            if (input.look.sqrMagnitude >= _threshold)
            {
                //마우스 입력에 Time.deltaTime을 곱하지 않음
                float deltaTimeMultiplier = 1.0f;

                _cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
            }

            // 회전값을 360도 이내로 제한
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine이 이 타겟을 따라갈 것임
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // 이동 속도, 달리기 속도 및 달리기 키 입력 여부에 따라 목표 속도 설정
            float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;

            // 간단한 가속 및 감속 구현. 쉽게 제거, 교체 또는 반복할 수 있도록 설계됨

            // 참고: Vector2의 == 연산자는 근사치를 사용하므로 부동소수점 오류에 영향을 받지 않으며, magnitude보다 저렴함
            // 입력이 없으면 목표 속도를 0으로 설정
            if (input.move == Vector2.zero) targetSpeed = 0.0f;

            // 플레이어의 현재 수평 속도 참조
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = 1f;

            // 목표 속도까지 가속 또는 감속
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // 선형이 아닌 곡선 결과를 만들어 더 자연스러운 속도 변화 제공
                // Lerp의 T가 제한되어 있으므로 속도를 따로 제한할 필요 없음
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // 속도를 소수점 3자리까지 반올림
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // 입력 방향 정규화
            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

            // 참고: Vector2의 != 연산자는 근사치를 사용하므로 부동소수점 오류에 영향을 받지 않으며, magnitude보다 저렴함
            // 이동 입력이 있을 때 플레이어가 이동하는 방향으로 회전
            if (input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // 카메라 위치를 기준으로 입력 방향을 향해 회전
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // 플레이어 이동
            controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // 캐릭터를 사용하는 경우 애니메이터 업데이트
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // 낙하 타임아웃 타이머 초기화
                _fallTimeoutDelta = FallTimeout;

                // 캐릭터를 사용하는 경우 애니메이터 업데이트
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // 지면에 있을 때 수직 속도가 무한히 떨어지는 것을 방지
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // 점프
                if (input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // 원하는 높이에 도달하는 데 필요한 속도 = H * -2 * G의 제곱근
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // 캐릭터를 사용하는 경우 애니메이터 업데이트
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // 점프 타임아웃
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // 점프 타임아웃 타이머 초기화
                _jumpTimeoutDelta = JumpTimeout;

                // 낙하 타임아웃
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // 캐릭터를 사용하는 경우 애니메이터 업데이트
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // 지면에 없을 때는 점프 불가
                input.jump = false;
            }

            // 종단 속도 이하일 때 시간에 따라 중력 적용(시간이 지날수록 선형적으로 속도 증가를 위해 델타 타임을 두 번 곱함)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // 선택됐을 때 지면 콜라이더의 위치와 반경에 맞는 기즈모 그리기
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (NetworkClient.localPlayer == null) return;

            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (NetworkClient.localPlayer == null) return;

            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }
}