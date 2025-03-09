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
        [SerializeField] private Transform player;
        [SerializeField] private int textureSize = 512;
        [SerializeField] private int scanInterval = 4;
        [SerializeField] private FilterMode minimapFilterMode = FilterMode.Bilinear;
        [SerializeField] private float iconSize = 10f;

        [Tooltip("아이콘 업데이트 간격")]
        [SerializeField] private float iconUpdateInterval = 1f;

        [SerializeField] private Color groundColor = new Color(0.2f, 0.5f, 0.2f);
        [SerializeField] private Color pathColor = new Color(0.7f, 0.7f, 0.5f);
        [SerializeField] private Color waterColor = new Color(0.3f, 0.4f, 0.8f);
        [SerializeField] private Color buildingColor = new Color(0.6f, 0.3f, 0.3f);

        [Header("아이콘 Prefab")]
        [SerializeField] private GameObject playerIconPrefab;
        [SerializeField] private GameObject npcIconPrefab;
        [SerializeField] private GameObject enemyIconPrefab;

        private GameObject playerIconObject;
        private RectTransform playerIconRect;
        private Dictionary<GameObject, GameObject> npcIcons = new Dictionary<GameObject, GameObject>();
        private Dictionary<GameObject, GameObject> enemyIcons = new Dictionary<GameObject, GameObject>();
        private float lastIconUpdateTime;
        private Texture2D minimapTexture;
        private Dictionary<string, Color[]> chunkTextures = new Dictionary<string, Color[]>();
        private string currentChunkKey;
        private Vector3 lastPlayerPosition;
        private float chunkSize;
        private float pixelsPerUnit;
        private Vector2 minimapOffset = Vector2.zero;
        private Texture2D combinedTexture;
        private bool needsTextureUpdate = false;

        private void Start()
        {
            Init().Forget();
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            if (Vector3.Distance(lastPlayerPosition, player.position) > 5f)
            {
                UpdateCurrentChunk();
                lastPlayerPosition = player.position;
                needsTextureUpdate = true;
            }

            DrawPlayerIcon();

            if (needsTextureUpdate)
            {
                UpdateCombinedTexture();
                needsTextureUpdate = false;
            }

            UpdateMinimapBackground();

            // NPC와 Enemy 아이콘 업데이트 (일정간격)
            if (Time.time - lastIconUpdateTime > iconUpdateInterval)
            {
                UpdateNPCAndEnemyIcons();
                lastIconUpdateTime = Time.time;
            }
        }

        private async Awaitable Init()
        {
            while (NetworkClient.localPlayer == null)
            {
                await Awaitable.NextFrameAsync();
            }

            player = NetworkClient.localPlayer.gameObject.transform;

            // 청크 계산
            chunkSize = Singleton.Game.ChunkSize;
            pixelsPerUnit = textureSize / chunkSize;

            // 미니맵 텍스처 초기화
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

            InitPlayerIcon();
            UpdateCurrentChunk();
            ScanCurrentChunk();
            UpdateNPCAndEnemyIcons();
            lastIconUpdateTime = Time.time;
        }

        private void InitPlayerIcon()
        {
            playerIconObject = Instantiate(playerIconPrefab, transform);
            playerIconRect = playerIconObject.GetComponent<RectTransform>();

            float iconSize = this.iconSize * 2;
            playerIconRect.sizeDelta = new Vector2(iconSize, iconSize);
        }

        private void UpdateCurrentChunk()
        {
            int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
            int chunkZ = Mathf.FloorToInt(player.position.z / chunkSize);
            string newChunkKey = $"Chunk_{chunkX}_{chunkZ}";

            if (newChunkKey != currentChunkKey)
            {
                currentChunkKey = newChunkKey;
                ScanCurrentChunk();
                ScanAdjacentChunks(chunkX, chunkZ);
                needsTextureUpdate = true;
            }
        }

        private void ScanCurrentChunk()
        {
            if (chunkTextures.TryGetValue(currentChunkKey, out Color[] savedTexture))
            {
                minimapTexture.SetPixels(savedTexture);
                minimapTexture.Apply(true);
                return;
            }

            int chunkX = int.Parse(currentChunkKey.Split('_')[1]);
            int chunkZ = int.Parse(currentChunkKey.Split('_')[2]);
            ScanChunk(currentChunkKey, chunkX, chunkZ);

            minimapTexture.SetPixels(chunkTextures[currentChunkKey]);
            minimapTexture.Apply(true);
        }

        private void ScanAdjacentChunks(int centerX, int centerZ)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0) continue;

                    int adjX = centerX + x;
                    int adjZ = centerZ + z;
                    string adjChunkKey = $"Chunk_{adjX}_{adjZ}";

                    if (chunkTextures.ContainsKey(adjChunkKey)) continue;
                    ScanChunk(adjChunkKey, adjX, adjZ);
                }
            }
        }


        private void ScanChunk(string chunkKey, int chunkX, int chunkZ)
        {
            Color[] pixels = new Color[textureSize * textureSize];
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;

            for (int x = 0; x < textureSize; x += scanInterval)
            {
                for (int y = 0; y < textureSize; y += scanInterval)
                {
                    float worldX = chunkStartX + (x / pixelsPerUnit);
                    float worldZ = chunkStartZ + (y / pixelsPerUnit);

                    Vector3 rayStart = new Vector3(worldX, 1000f, worldZ);
                    Color pixelColor = DetectTerrainAtPosition(rayStart);

                    for (int i = 0; i < scanInterval && x + i < textureSize; i++)
                    {
                        for (int j = 0; j < scanInterval && y + j < textureSize; j++)
                        {
                            int index = (y + j) * textureSize + (x + i);
                            if (index < pixels.Length)
                            {
                                pixels[index] = pixelColor;
                            }
                        }
                    }
                }
            }

            chunkTextures[chunkKey] = pixels;
        }

        private Color DetectTerrainAtPosition(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, 5000f, -1, QueryTriggerInteraction.Ignore))
            {
                Color baseColor;

                if (hit.collider.CompareTag("Water"))
                {
                    baseColor = waterColor;
                }
                else if (hit.collider.CompareTag("Road") || hit.collider.CompareTag("Path"))
                {
                    baseColor = pathColor;
                }
                else if (hit.collider.CompareTag("Building"))
                {
                    baseColor = buildingColor;
                }
                else
                {
                    baseColor = groundColor;
                }

                float heightFactor = Mathf.Clamp01(hit.point.y / 20f);
                float slopeFactor = Vector3.Dot(hit.normal, Vector3.up);

                Color heightColor = Color.Lerp(baseColor * 0.7f, baseColor * 1.3f, heightFactor);
                Color slopeColor = Color.Lerp(baseColor * 0.8f, baseColor, slopeFactor);

                return Color.Lerp(slopeColor, heightColor, 0.3f);
            }

            return new Color(0, 0, 0, 0);
        }

        private void DrawPlayerIcon()
        {
            playerIconRect.anchoredPosition = Vector2.zero;

            int chunkX = int.Parse(currentChunkKey.Split('_')[1]);
            int chunkZ = int.Parse(currentChunkKey.Split('_')[2]);
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;
            float relativeX = (player.position.x - chunkStartX) / chunkSize;
            float relativeZ = (player.position.z - chunkStartZ) / chunkSize;
            RectTransform minimapRect = minimapImage.rectTransform;
            float offsetX = relativeX * minimapRect.rect.width - minimapRect.rect.width * 0.5f;
            float offsetY = relativeZ * minimapRect.rect.height - minimapRect.rect.height * 0.5f;
            minimapOffset = new Vector2(offsetX, offsetY);

            //플레이어가 바라보는 방향
            playerIconRect.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }

        private void UpdateNPCAndEnemyIcons()
        {
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            HashSet<GameObject> existingNPCs = new HashSet<GameObject>(npcIcons.Keys);
            HashSet<GameObject> existingEnemies = new HashSet<GameObject>(enemyIcons.Keys);

            foreach (GameObject npc in npcs)
            {
                existingNPCs.Remove(npc);

                if (npcIcons.TryGetValue(npc, out GameObject existingIcon))
                {
                    if (existingIcon != null)
                    {
                        UpdateIconPosition(npc, existingIcon);
                    }
                }
                else
                {
                    GameObject iconObj = Instantiate(npcIconPrefab, transform);
                    RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                    iconRect.sizeDelta = new Vector2(iconSize * 2, iconSize * 2);
                    npcIcons.Add(npc, iconObj);

                    UpdateIconPosition(npc, iconObj);
                }
            }

            foreach (GameObject enemy in enemies)
            {
                existingEnemies.Remove(enemy);

                if (enemyIcons.TryGetValue(enemy, out GameObject existingIcon))
                {
                    if (existingIcon != null)
                    {
                        UpdateIconPosition(enemy, existingIcon);
                    }
                }
                else
                {
                    GameObject iconObj = Instantiate(enemyIconPrefab, transform);
                    RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                    iconRect.sizeDelta = new Vector2(iconSize * 2, iconSize * 2);
                    enemyIcons.Add(enemy, iconObj);

                    UpdateIconPosition(enemy, iconObj);
                }
            }

            foreach (GameObject oldNPC in existingNPCs)
            {
                if (npcIcons.TryGetValue(oldNPC, out GameObject iconObj))
                {
                    Destroy(iconObj);
                    npcIcons.Remove(oldNPC);
                }
            }

            foreach (GameObject oldEnemy in existingEnemies)
            {
                if (enemyIcons.TryGetValue(oldEnemy, out GameObject iconObj))
                {
                    Destroy(iconObj);
                    enemyIcons.Remove(oldEnemy);
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

            int chunkX = int.Parse(currentChunkKey.Split('_')[1]);
            int chunkZ = int.Parse(currentChunkKey.Split('_')[2]);
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;
            float relativeX = (target.transform.position.x - chunkStartX) / chunkSize;
            float relativeZ = (target.transform.position.z - chunkStartZ) / chunkSize;
            RectTransform minimapRect = minimapImage.rectTransform;
            float posX = relativeX * minimapRect.rect.width;
            float posY = relativeZ * minimapRect.rect.height;

            posX -= minimapRect.rect.width * 0.5f;
            posY -= minimapRect.rect.height * 0.5f;

            //오프셋을 적용하여 아이콘 위치 조정
            posX -= minimapOffset.x;
            posY -= minimapOffset.y;

            // 미니맵 경계 확인
            float halfWidth = minimapRect.rect.width * 0.5f;
            float halfHeight = minimapRect.rect.height * 0.5f;

            // 미니맵 영역을 벗어나면 아이콘 비활성화
            if (Mathf.Abs(posX) > halfWidth || Mathf.Abs(posY) > halfHeight)
            {
                iconObject.SetActive(false);
                return;
            }
            iconObject.SetActive(true);
            iconRect.anchoredPosition = new Vector2(posX, posY);
            iconRect.rotation = Quaternion.Euler(0, 0, -target.transform.eulerAngles.y);
        }

        private void UpdateCombinedTexture()
        {
            int centerX = int.Parse(currentChunkKey.Split('_')[1]);
            int centerZ = int.Parse(currentChunkKey.Split('_')[2]);

            for (int z = -1; z <= 1; z++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    int chunkX = centerX + x;
                    int chunkZ = centerZ + z;
                    string chunkKey = $"Chunk_{chunkX}_{chunkZ}";

                    if (!chunkTextures.ContainsKey(chunkKey))
                    {
                        ScanChunk(chunkKey, chunkX, chunkZ);
                    }

                    Color[] chunkPixels = chunkTextures[chunkKey];

                    // 결합된 텍스처에서의 위치 계산 (좌표계 변환)
                    // x, z는 -1, 0, 1 범위이므로 +1하여 0, 1, 2 범위로 변환
                    int targetX = (x + 1) * textureSize;
                    int targetZ = (z + 1) * textureSize;

                    combinedTexture.SetPixels(targetX, targetZ, textureSize, textureSize, chunkPixels);
                }
            }

            combinedTexture.Apply(true);
        }

        private void UpdateMinimapBackground()
        {
            int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
            int chunkZ = Mathf.FloorToInt(player.position.z / chunkSize);
            float chunkStartX = chunkX * chunkSize;
            float chunkStartZ = chunkZ * chunkSize;
            float localRelativeX = (player.position.x - chunkStartX) / chunkSize;
            float localRelativeZ = (player.position.z - chunkStartZ) / chunkSize;
            float gridX = (localRelativeX + 1.0f) / 3.0f;
            float gridZ = (localRelativeZ + 1.0f) / 3.0f;

            // UV 좌표 계산 (플레이어가 중앙에 오도록)
            Rect uvRect = minimapImage.uvRect;
            uvRect.width = 1.0f / 3.0f;
            uvRect.height = 1.0f / 3.0f;
            uvRect.x = gridX - (uvRect.width / 2.0f);
            uvRect.y = gridZ - (uvRect.height / 2.0f);

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

            foreach (var icon in npcIcons.Values)
            {
                if (icon != null)
                    Destroy(icon);
            }

            foreach (var icon in enemyIcons.Values)
            {
                if (icon != null)
                    Destroy(icon);
            }

            npcIcons.Clear();
            enemyIcons.Clear();
        }
    }
}
