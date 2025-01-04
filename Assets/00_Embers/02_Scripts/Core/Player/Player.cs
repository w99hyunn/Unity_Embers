using System;
using Mirror;
using UnityEngine;

namespace STARTING
{
    public class Player : NetworkBehaviour
    {
        private void Start()
        {
            //Managers.UI.FadeOut();
            if (isLocalPlayer)
            {
                InitPosition();
            }
        }

        private void InitPosition()
        {
            if (Managers.Game != null && Managers.Game.playerData != null)
            {
                transform.position = Managers.Game.playerData.Position;
                SavePosition();
                Debug.Log($"초기 위치 설정: {transform.position}");
            }
            else
            {
                Debug.LogError("플레이어 데이터가 초기화되지 않았습니다.");
            }
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
        public void CmdRemovePlayer()
        {
            NetworkServer.Destroy(connectionToClient.identity.gameObject);
        }
    }
}