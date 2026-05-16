using System;
using UnityEngine;

namespace Embers
{
    public static class AwaitableExtensions
    {
        public static async void Forget(this Awaitable task)
        {
            try
            {
                await task; // 비동기 실행
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}