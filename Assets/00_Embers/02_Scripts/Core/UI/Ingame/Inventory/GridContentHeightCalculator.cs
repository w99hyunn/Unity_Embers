using UnityEngine;
using UnityEngine.UI;

namespace NOLDA
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridContentHeightCalculator : MonoBehaviour
    {
        private RectTransform contentRect; // Scroll View의 Content
        private GridLayoutGroup gridLayoutGroup;

        private void Awake()
        {
            TryGetComponent<RectTransform>(out contentRect);
            TryGetComponent<GridLayoutGroup>(out gridLayoutGroup);
        }

        private void Start()
        {
            UpdateContentHeight();
        }

        /// <summary>
        /// GridLayoutGroup의 설정값을 기반으로 콘텐츠 높이를 계산 및 업데이트
        /// </summary>
        public void UpdateContentHeight()
        {
            if (contentRect == null || gridLayoutGroup == null)
                return;

            int columnCount = CalculateColumnCount(); // 한 줄에 들어갈 셀 개수
            int totalChildCount = contentRect.childCount; // 자식 오브젝트 개수
            int rowCount = Mathf.CeilToInt((float)totalChildCount / columnCount); // 전체 행 개수

            // 콘텐츠 높이 계산
            float totalHeight = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom; // 상단/하단 패딩
            totalHeight += rowCount * gridLayoutGroup.cellSize.y; // 모든 행의 셀 높이 합
            totalHeight += (rowCount - 1) * gridLayoutGroup.spacing.y; // 행 간 간격 합

            // 콘텐츠 높이 적용
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        }

        /// <summary>
        /// 한 줄에 들어갈 셀의 개수 계산 (스크롤뷰의 가로 크기 기준)
        /// </summary>
        /// <returns>열의 개수</returns>
        private int CalculateColumnCount()
        {
            float contentWidth = contentRect.rect.width; // 콘텐츠의 너비
            float cellWidth = gridLayoutGroup.cellSize.x; // 셀의 가로 크기
            float spacingX = gridLayoutGroup.spacing.x; // 셀 간의 가로 간격

            // 한 줄에 들어갈 수 있는 최대 셀 개수 계산
            return Mathf.Max(1,
                Mathf.FloorToInt(
                    (contentWidth - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right + spacingX) /
                    (cellWidth + spacingX)));
        }
    }
}