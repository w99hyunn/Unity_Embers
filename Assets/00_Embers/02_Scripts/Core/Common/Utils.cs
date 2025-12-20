using UnityEngine;

namespace NOLDA
{
    public static class Utils
    {
        /// <summary>
        /// 각도를 기준으로 원의 둘레 위치를 구함
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 GetPositionFromAngle(float radius, float angle)
        {
            Vector3 position = Vector3.zero;

            angle = DegreeToRadian(angle);

            position.x = Mathf.Cos(angle) * radius;
            position.y = Mathf.Sin(angle) * radius;

            return position;
        }

        /// <summary>
        /// Degree 값을 Radian 값으로 변환
        /// 1도는 "PI/180" radian
        /// angle도는 "PI/180 * angle" radian
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float DegreeToRadian(float angle)
        {
            return Mathf.PI * angle / 180;
        }
    }
}