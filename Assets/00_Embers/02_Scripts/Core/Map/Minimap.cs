using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace NOLDA
{
    public class Minimap : MonoBehaviour
    {
        [Header("미니맵 설정")]
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private int textureSize = 512;
        [SerializeField] private int scanInterval = 4;
        [SerializeField] private FilterMode minimapFilterMode = FilterMode.Bilinear;
        [SerializeField] private float iconSize = 10f;

        [Tooltip("아이콘 업데이트 간격")]
        [SerializeField] private float iconUpdateInterval = 1f;

        [Tooltip("청크 캐시 최대 크기")]
        [SerializeField] private int maxChunkCacheSize = 20;

        [SerializeField] private Color groundColor = new Color(0.2f, 0.5f, 0.2f);
        [SerializeField] private Color pathColor = new Color(0.7f, 0.7f, 0.5f);
        [SerializeField] private Color waterColor = new Color(0.3f, 0.4f, 0.8f);
        [SerializeField] private Color buildingColor = new Color(0.6f, 0.3f, 0.3f);

        [Header("아이콘 Prefab")]
        [SerializeField] private GameObject playerIconPrefab;
        [SerializeField] private GameObject npcIconPrefab;
        [SerializeField] private GameObject enemyIconPrefab;

        private Transform player;
        private GameObject playerIconObject;
        private RectTransform playerIconRect;
        private Dictionary<GameObject, GameObject> npcIcons = new Dictionary<GameObject, GameObject>();
        private Dictionary<GameObject, GameObject> enemyIcons = new Dictionary<GameObject, GameObject>();
        private Texture2D minimapTexture;
        private Dictionary<string, Color[]> chunkTextures = new Dictionary<string, Color[]>();
        private Queue<string> chunkCacheOrder = new Queue<string>();
        private string currentChunkKey;
        private Vector3 lastPlayerPosition;
        private float chunkSize;
        private float pixelsPerUnit;
        private Vector2 minimapOffset = Vector2.zero;
        private Texture2D combinedTexture;
        private bool needsTextureUpdate = false;

        // 캐싱을 위한 변수들
        private RectTransform minimapRect;
        private float halfMinimapWidth;
        private float halfMinimapHeight;
        private readonly int[] chunkOffsets = { -1, 0, 1 };

        // 레이캐스트 결과 캐싱
        private RaycastHit hitInfo;

        private void Start()
        {
            Init().Forget();
        }

        private void Update()
        {
            if (player == null)
                return;

            if ((lastPlayerPosition - player.position).sqrMagnitude > 25f)
            {
                UpdateCurrentChunk();
                lastPlayerPosition = player.position;
                needsTextureUpdate = true;
            }

            DrawPlayerIcon();

            // 결합된 텍스처 업데이트 (필요한 경우)
            if (needsTextureUpdate)
            {
                UpdateCombinedTexture();
                needsTextureUpdate = false;
            }

            UpdateMinimapBackground();
        }

        private async Awaitable Init()
        {
            while (NetworkClient.localPlayer == null)
            {
                await Awaitable.NextFrameAsync();
            }

            player = NetworkClient.localPlayer.gameObject.transform;

            // 미니맵 RectTransform 캐싱
            minimapRect = minimapImage.rectTransform;
            halfMinimapWidth = minimapRect.rect.width * 0.5f;
            halfMinimapHeight = minimapRect.rect.height * 0.5f;

            // 청크 계산
            chunkSize = Singleton.Game.ChunkSize;
            pixelsPerUnit = textureSize / chunkSize;

            // 미니맵 텍스처 초기화 (미리 할당하여 GC 부담 감소)
            minimapTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
            minimapTexture.filterMode = minimapFilterMode;
            minimapTexture.wrapMode = TextureWrapMode.Clamp;
            minimapTexture.anisoLevel = 1;

            // 결합된 텍스처 초기화 (3x3 청크)
            combinedTexture = new Texture2D(textureSize * 3, textureSize * 3, TextureFormat.RGBA32, false);
            combinedTexture.filterMode = minimapFilterMode;
            combinedTexture.wrapMode = TextureWrapMode.Clamp;
            combinedTexture.anisoLevel = 1;

            // 결합된 텍스처를 미니맵 이미지 할당
            minimapImage.texture = combinedTexture;

            lastPlayerPosition = player.position;

            InitPlayerIcon();
            UpdateCurrentChunk();
            ScanCurrentChunk();
            UpdateNPCAndEnemyIcons();
            UpdateIconsAsync().Forget();
        }

        private async Awaitable UpdateIconsAsync()
        {
            while (true)
            {
                UpdateNPCAndEnemyIcons();
                await Awaitable.WaitForSecondsAsync(iconUpdateInterval);
            }
        }

        private void InitPlayerIcon()
        {
            playerIconObject = Instantiate(playerIconPrefab, transform);
            playerIconRect = playerIconObject.GetComponent<RectTransform>();

            float finalIconSize = iconSize * 2;
            playerIconRect.sizeDelta = new Vector2(finalIconSize, finalIconSize);
        }

        private void UpdateCurrentChunk()
        {
            // 게임 내 청크 좌표 계산
            int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
            int chunkZ = Mathf.FloorToInt(player.position.z / chunkSize);
            string newChunkKey = $"Chunk_{chunkX}_{chunkZ}";

            if (newChunkKey != currentChunkKey)
            {
                currentChunkKey = newChunkKey;
                ScanCurrentChunk();

                // 인접 청크도 함께 스캔
                ScanAdjacentChunks(chunkX, chunkZ);

                // 결합된 텍스처 업데이트 필요 표시
                needsTextureUpdate = true;
            }
        }

        private void ScanCurrentChunk()
        {
            if (chunkTextures.TryGetValue(currentChunkKey, out Color[] savedTexture))
            {
                // 저장된 청크 텍스처가 있으면 사용
                minimapTexture.SetPixels(savedTexture);
                minimapTexture.Apply(true);

                // 캐시 순서 업데이트 (LRU 캐시 구현)
                UpdateChunkCacheOrder(currentChunkKey);
                return;
            }

            // 현재 청크의 좌표 파싱
            string[] chunkCoords = currentChunkKey.Split('_');
            int chunkX = int.Parse(chunkCoords[1]);
            int chunkZ = int.Parse(chunkCoords[2]);

            // 청크 스캔
            ScanChunk(currentChunkKey, chunkX, chunkZ);

            // 텍스처 업데이트
            minimapTexture.SetPixels(chunkTextures[currentChunkKey]);
            minimapTexture.Apply(true);
        }

        // 청크 캐시 순서 업데이트 (LRU 캐시 구현)
        private void UpdateChunkCacheOrder(string chunkKey)
        {
            // 이미 큐에 있으면 제거
            if (chunkCacheOrder.Contains(chunkKey))
            {
                // 큐에서 제거하고 다시 추가하기 위해 임시 리스트 사용
                List<string> tempList = new List<string>(chunkCacheOrder);
                tempList.Remove(chunkKey);
                chunkCacheOrder = new Queue<string>(tempList);
            }

            // 가장 최근에 사용한 청크로 추가
            chunkCacheOrder.Enqueue(chunkKey);

            // 캐시 크기 제한
            while (chunkCacheOrder.Count > maxChunkCacheSize && chunkCacheOrder.Count > 0)
            {
                string oldestChunk = chunkCacheOrder.Dequeue();
                // 현재 사용 중인 청크가 아니면 제거
                if (oldestChunk != currentChunkKey && !IsAdjacentChunk(oldestChunk))
                {
                    chunkTextures.Remove(oldestChunk);
                }
            }
        }

        // 인접 청크인지 확인
        private bool IsAdjacentChunk(string chunkKey)
        {
            if (string.IsNullOrEmpty(currentChunkKey))
                return false;

            string[] currentCoords = currentChunkKey.Split('_');
            string[] checkCoords = chunkKey.Split('_');

            if (currentCoords.Length < 3 || checkCoords.Length < 3)
                return false;

            int currentX = int.Parse(currentCoords[1]);
            int currentZ = int.Parse(currentCoords[2]);
            int checkX = int.Parse(checkCoords[1]);
            int checkZ = int.Parse(checkCoords[2]);

            return Mathf.Abs(currentX - checkX) <= 1 && Mathf.Abs(currentZ - checkZ) <= 1;
        }

        private void ScanAdjacentChunks(int centerX, int centerZ)
        {
            // 주변 8개 청크 스캔 (배열 재사용)
            foreach (int x in chunkOffsets)
            {
                foreach (int z in chunkOffsets)
                {
                    // 중앙 청크는 이미 스캔했으므로 건너뜀
                    if (x == 0 && z == 0) continue;

                    int adjX = centerX + x;
                    int adjZ = centerZ + z;
                    string adjChunkKey = $"Chunk_{adjX}_{adjZ}";

                    // 이미 스캔한 청크는 건너뜀
                    if (chunkTextures.ContainsKey(adjChunkKey))
                    {
                        // 캐시 순서 업데이트
                        UpdateChunkCacheOrder(adjChunkKey);
                        continue;
                    }

                    // 인접 청크 스캔
                    ScanChunk(adjChunkKey, adjX, adjZ);
                }
            }
        }

        // 특정 청크 스캔
        private void ScanChunk(string chunkKey, int chunkX, int chunkZ)
        {
            // 청크 정보 생성
            Color[] pixels = new Color[textureSize * textureSize];

            // 청크의 월드 좌표 최소값 계산
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;

            // 레이캐스트로 지형 정보 스캔 (최적화된 루프)
            int pixelIndex = 0;
            for (int y = 0; y < textureSize; y += scanInterval)
            {
                for (int x = 0; x < textureSize; x += scanInterval)
                {
                    float worldX = chunkStartX + (x / pixelsPerUnit);
                    float worldZ = chunkStartZ + (y / pixelsPerUnit);

                    Vector3 rayStart = new Vector3(worldX, 1000f, worldZ);
                    Color pixelColor = DetectTerrainAtPosition(rayStart);

                    // 설정된 간격 블록으로 채움 (최적화된 루프)
                    for (int j = 0; j < scanInterval && y + j < textureSize; j++)
                    {
                        int rowStart = (y + j) * textureSize + x;
                        for (int i = 0; i < scanInterval && x + i < textureSize; i++)
                        {
                            pixelIndex = rowStart + i;
                            if (pixelIndex < pixels.Length)
                            {
                                pixels[pixelIndex] = pixelColor;
                            }
                        }
                    }
                }
            }

            // 생성한 청크 텍스처 저장
            chunkTextures[chunkKey] = pixels;

            // 캐시 순서 업데이트
            UpdateChunkCacheOrder(chunkKey);
        }

        private Color DetectTerrainAtPosition(Vector3 position)
        {
            // 레이캐스트 거리를 늘리고 모든 레이어 포함 (hitInfo 재사용)
            if (Physics.Raycast(position, Vector3.down, out hitInfo, 5000f, -1, QueryTriggerInteraction.Ignore))
            {
                Color baseColor;
                string tag = hitInfo.collider.tag;

                // 태그 비교 최적화 (switch 문 사용)
                switch (tag)
                {
                    case "Water":
                        return waterColor; // 물은 높이 계산 필요 없음
                    case "Road":
                    case "Path":
                        baseColor = pathColor;
                        break;
                    case "Building":
                        baseColor = buildingColor;
                        break;
                    default:
                        baseColor = groundColor;
                        break;
                }

                // 물이 아닌 경우에만 높이와 경사도 계산
                // 높이에 따른 색상 변형 (더 부드러운 보간)
                float heightFactor = Mathf.Clamp01(hitInfo.point.y / 20f); // 높이 범위 확장

                // 경사도 계산 (노멀 벡터 사용)
                float slopeFactor = Vector3.Dot(hitInfo.normal, Vector3.up); // 1 = 평평, 0 = 수직

                // 높이와 경사도를 결합하여 음영 계산
                Color heightColor = Color.Lerp(baseColor * 0.7f, baseColor * 1.3f, heightFactor);
                Color slopeColor = Color.Lerp(baseColor * 0.8f, baseColor, slopeFactor);

                // 최종 색상 결정 (높이 영향력에 따라 보간)
                return Color.Lerp(slopeColor, heightColor, 0.3f);
            }

            return new Color(0, 0, 0, 0); // 투명 (아무것도 없음)
        }

        private void DrawPlayerIcon()
        {
            // 플레이어 아이콘이 미니맵 중앙에 고정
            playerIconRect.anchoredPosition = Vector2.zero;

            // 현재 플레이어 위치를 기준으로 미니맵 오프셋 계산
            string[] chunkCoords = currentChunkKey.Split('_');
            int chunkX = int.Parse(chunkCoords[1]);
            int chunkZ = int.Parse(chunkCoords[2]);
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;

            // 플레이어의 청크 내 상대 위치 계산 (0~1 범위)
            float relativeX = (player.position.x - chunkStartX) / chunkSize;
            float relativeZ = (player.position.z - chunkStartZ) / chunkSize;

            // 미니맵 오프셋 계산 (플레이어 위치를 중심으로)
            float offsetX = relativeX * minimapRect.rect.width - halfMinimapWidth;
            float offsetY = relativeZ * minimapRect.rect.height - halfMinimapHeight;
            minimapOffset = new Vector2(offsetX, offsetY);

            // 방향 설정 (플레이어가 바라보는 방향)
            playerIconRect.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }

        private void UpdateNPCAndEnemyIcons()
        {
            // 현재 활성화된 NPC와 Enemy 찾기 (태그 기반 검색은 비용이 크므로 주의)
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // 기존 아이콘 목록 복사 (삭제 확인용)
            HashSet<GameObject> existingNPCs = new HashSet<GameObject>(npcIcons.Keys);
            HashSet<GameObject> existingEnemies = new HashSet<GameObject>(enemyIcons.Keys);

            // NPC 아이콘 업데이트
            UpdateIconsForTargets(npcs, npcIcons, npcIconPrefab, existingNPCs);

            // Enemy 아이콘 업데이트
            UpdateIconsForTargets(enemies, enemyIcons, enemyIconPrefab, existingEnemies);

            // 더 이상 존재하지 않는 아이콘 제거
            RemoveOldIcons(existingNPCs, npcIcons);
            RemoveOldIcons(existingEnemies, enemyIcons);
        }

        // 대상 객체들의 아이콘 업데이트 (코드 중복 제거)
        private void UpdateIconsForTargets(GameObject[] targets, Dictionary<GameObject, GameObject> iconDict,
                                          GameObject iconPrefab, HashSet<GameObject> existingTargets)
        {
            foreach (GameObject target in targets)
            {
                existingTargets.Remove(target); // 처리된 대상 제거

                // 이미 아이콘이 있는지 확인
                if (iconDict.TryGetValue(target, out GameObject existingIcon))
                {
                    // 기존 아이콘 위치 업데이트
                    if (existingIcon != null)
                    {
                        UpdateIconPosition(target, existingIcon);
                    }
                }
                else
                {
                    // 새 아이콘 생성
                    GameObject iconObj = Instantiate(iconPrefab, transform);
                    RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                    iconRect.sizeDelta = new Vector2(iconSize * 2, iconSize * 2);
                    iconDict.Add(target, iconObj);

                    // 아이콘 위치 업데이트
                    UpdateIconPosition(target, iconObj);
                }
            }
        }

        // 더 이상 존재하지 않는 아이콘 제거 (코드 중복 제거)
        private void RemoveOldIcons(HashSet<GameObject> oldTargets, Dictionary<GameObject, GameObject> iconDict)
        {
            foreach (GameObject oldTarget in oldTargets)
            {
                if (iconDict.TryGetValue(oldTarget, out GameObject iconObj))
                {
                    Destroy(iconObj);
                    iconDict.Remove(oldTarget);
                }
            }
        }

        private void UpdateIconPosition(GameObject target, GameObject iconObject)
        {
            if (target == null || iconObject == null)
                return;

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            if (iconRect == null)
                return;

            string[] chunkCoords = currentChunkKey.Split('_');
            int chunkX = int.Parse(chunkCoords[1]);
            int chunkZ = int.Parse(chunkCoords[2]);
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;
            float relativeX = (target.transform.position.x - chunkStartX) / chunkSize;
            float relativeZ = (target.transform.position.z - chunkStartZ) / chunkSize;

            // UI 좌표계로 변환 (캐싱된 값 사용)
            float posX = relativeX * minimapRect.rect.width - halfMinimapWidth;
            float posY = relativeZ * minimapRect.rect.height - halfMinimapHeight;

            // 오프셋을 적용하여 아이콘 위치 조정
            posX -= minimapOffset.x;
            posY -= minimapOffset.y;

            // 미니맵 영역을 벗어나면 아이콘 비활성화 (캐싱된 값 사용)
            if (Mathf.Abs(posX) > halfMinimapWidth || Mathf.Abs(posY) > halfMinimapHeight)
            {
                iconObject.SetActive(false);
                return;
            }

            // 미니맵 영역 내에 있으면 아이콘 활성화
            iconObject.SetActive(true);

            // 아이콘 위치 설정
            iconRect.anchoredPosition = new Vector2(posX, posY);

            // 방향 설정 (Enemy 아이콘에만 적용)
            if (target.CompareTag("Enemy"))
            {
                iconRect.rotation = Quaternion.Euler(0, 0, -target.transform.eulerAngles.y);
            }
            else
            {
                // NPC 아이콘은 회전 없이 기본 방향 유지
                iconRect.rotation = Quaternion.identity;
            }
        }

        // 결합된 텍스처 업데이트 (3x3 청크)
        private void UpdateCombinedTexture()
        {
            // 현재 청크의 좌표 파싱
            string[] chunkCoords = currentChunkKey.Split('_');
            int centerX = int.Parse(chunkCoords[1]);
            int centerZ = int.Parse(chunkCoords[2]);

            // 3x3 그리드의 모든 청크 텍스처 결합 (배열 재사용)
            foreach (int z in chunkOffsets)
            {
                foreach (int x in chunkOffsets)
                {
                    int chunkX = centerX + x;
                    int chunkZ = centerZ + z;
                    string chunkKey = $"Chunk_{chunkX}_{chunkZ}";

                    // 해당 청크의 텍스처가 없으면 스캔
                    if (!chunkTextures.ContainsKey(chunkKey))
                    {
                        ScanChunk(chunkKey, chunkX, chunkZ);
                    }

                    // 결합된 텍스처의 해당 영역에 청크 텍스처 복사
                    Color[] chunkPixels = chunkTextures[chunkKey];

                    // 결합된 텍스처에서의 위치 계산 (좌표계 변환)
                    // x, z는 -1, 0, 1 범위이므로 +1하여 0, 1, 2 범위로 변환
                    int targetX = (x + 1) * textureSize;
                    int targetZ = (z + 1) * textureSize;

                    // 청크 텍스처를 결합된 텍스처에 복사
                    combinedTexture.SetPixels(targetX, targetZ, textureSize, textureSize, chunkPixels);
                }
            }

            // 결합된 텍스처 적용
            combinedTexture.Apply(true);
        }

        // 미니맵 배경 이동 (플레이어 중심 미니맵)
        private void UpdateMinimapBackground()
        {
            // 청크 좌표 계산
            string[] chunkCoords = currentChunkKey.Split('_');
            int chunkX = int.Parse(chunkCoords[1]);
            int chunkZ = int.Parse(chunkCoords[2]);
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;

            // 현재 청크 내에서의 상대 위치 (0~1 범위)
            float localRelativeX = (player.position.x - chunkStartX) / chunkSize;
            float localRelativeZ = (player.position.z - chunkStartZ) / chunkSize;

            // 결합된 텍스처를 사용하는 경우 (3x3 청크)
            // 플레이어 위치를 3x3 그리드 내의 상대 좌표로 변환
            float gridX = (localRelativeX + 1.0f) / 3.0f;
            float gridZ = (localRelativeZ + 1.0f) / 3.0f;

            // UV 좌표 계산 (플레이어가 중앙에 오도록)
            Rect uvRect = minimapImage.uvRect;
            uvRect.width = 1.0f / 3.0f; // 전체 텍스처의 1/3만 표시
            uvRect.height = 1.0f / 3.0f;
            uvRect.x = gridX - (uvRect.width / 2.0f);
            uvRect.y = gridZ - (uvRect.height / 2.0f);

            // UV 좌표 제한 (0~1 범위)
            uvRect.x = Mathf.Clamp(uvRect.x, 0, 1 - uvRect.width);
            uvRect.y = Mathf.Clamp(uvRect.y, 0, 1 - uvRect.height);

            minimapImage.uvRect = uvRect;
        }

        private void OnDestroy()
        {
            // 모든 아이콘 정리
            if (playerIconObject != null)
            {
                Destroy(playerIconObject);
            }

            // 딕셔너리 순회 최적화
            foreach (var pair in npcIcons)
            {
                if (pair.Value != null)
                    Destroy(pair.Value);
            }

            foreach (var pair in enemyIcons)
            {
                if (pair.Value != null)
                    Destroy(pair.Value);
            }

            npcIcons.Clear();
            enemyIcons.Clear();
            chunkTextures.Clear();
            chunkCacheOrder.Clear();

            // 텍스처 메모리 해제
            if (minimapTexture != null)
            {
                Destroy(minimapTexture);
                minimapTexture = null;
            }

            if (combinedTexture != null)
            {
                Destroy(combinedTexture);
                combinedTexture = null;
            }
        }
    }
}
