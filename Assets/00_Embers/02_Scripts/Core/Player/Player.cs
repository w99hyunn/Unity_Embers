using System;
using Mirror;
using UnityEngine;

namespace STARTING
{
    public class Player : NetworkBehaviour
    {
        private bool isDestroyed = false;
        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            InitializePlayerPosition();
        }

        private async void InitializePlayerPosition()
        {
            // 동기화된 데이터 기반으로 위치 초기화
            CmdUpdateServerPosition(Managers.Game.playerData.Position);
            transform.position = Managers.Game.playerData.Position;
            Debug.Log($"Position set to {transform.position}");
            
            // 위치 지속적으로 저장
            SavePosition();
        }

        private async Awaitable SavePosition()
        {
            while (true)
            {
                await Awaitable.WaitForSecondsAsync(5f);
                Managers.Game.playerData.Position = transform.position;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdUpdateServerPosition(Vector3 position)
        {
            // 서버에서 클라이언트 위치 업데이트
            transform.position = position;
        }

        [Command(requiresAuthority = false)]
        public void CmdRemovePlayer()
        {
            // 플레이어 삭제 요청
            NetworkServer.Destroy(connectionToClient.identity.gameObject);
        }
    }
}