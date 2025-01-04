using System;
using Mirror;
using UnityEngine;

namespace STARTING
{
    public class Player : NetworkBehaviour
    {
        public override async void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            if (isLocalPlayer)
            {
                InitPosition();
            }
        }
        
        private async void InitPosition()
        {
            transform.position = Managers.Game.playerData.Position;
            SavePosition();
            Debug.Log($"초기 위치 설정: {transform.position}");
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