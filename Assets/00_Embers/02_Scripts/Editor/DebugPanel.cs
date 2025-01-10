using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace STARTING
{
    public class DebugPanel : EditorWindow
    {
        private SceneData _sceneData;

        private const string BASE_SCENE_FOLDER_PATH = "Assets/00_Embers/01_Scenes/";
        private bool _loadAdditively = false;
        
        private int _selectedIndex = 0;
        private readonly string[] _options = { "HP +500", "MP +500", "HXP +450", "HP -500", "MP -500" };

        private Vector2 _scrollPosition;

        [MenuItem("STARTING/Debug Panel")]
        public static void ShowWindow()
        {
            GetWindow<DebugPanel>("Debug Panel");
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // 창이 열려 있는 경우 강제로 다시 그리기
            var window = GetWindow<DebugPanel>();
            if (window != null)
            {
                window.Repaint();
            }
        }
        
        private void OnEnable()
        {
            _sceneData = Resources.Load<SceneData>("SceneData");
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/00_Embers/04_Images/Editor/GameLevel Icon.png");
            titleContent = new GUIContent("Debug Panel", icon);
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("캐릭터 디버그 :", EditorStyles.boldLabel);
            _selectedIndex = EditorGUILayout.Popup("Option:", _selectedIndex, _options);

            // Button to execute selected option
            if (GUILayout.Button("Execute"))
            {
                ExecuteOption(_selectedIndex);
            }

            GUILayout.Space(10);
            GUILayout.Label("씬 전환 :", EditorStyles.boldLabel);
            _sceneData = (SceneData)EditorGUILayout.ObjectField("Scene Data", _sceneData, typeof(SceneData), false);
            
            if (_sceneData == null)
            {
                EditorGUILayout.HelpBox("Scene Data not found at specified path!", MessageType.Error);
                EditorGUILayout.EndScrollView();
                return;
            }
            
            // Additive 로드 옵션
            _loadAdditively = EditorGUILayout.Toggle("Additive로 불러오기", _loadAdditively);
            GUILayout.Space(10);
            foreach (var sceneInfo in _sceneData.scenes)
            {
                if (GUILayout.Button(sceneInfo.sceneIdentifier))
                {
                    string scenePath = FindScenePath(sceneInfo.sceneName);

                    if (!string.IsNullOrEmpty(scenePath))
                    {
                        if (Application.isPlaying)
                        {
                            if (_loadAdditively)
                            {
                                SceneManager.LoadScene(sceneInfo.sceneName, LoadSceneMode.Additive);
                                Debug.Log($"Loaded scene (Additive): {sceneInfo.sceneName}");
                            }
                            else
                            {
                                SceneManager.LoadScene(sceneInfo.sceneName);
                                Debug.Log($"Loaded scene: {sceneInfo.sceneName}");
                            }
                        }
                        else
                        {
                            if (_loadAdditively)
                            {
                                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                                Debug.Log($"Opened scene (Additive): {scenePath}");
                            }
                            else
                            {
                                EditorSceneManager.OpenScene(scenePath);
                                Debug.Log($"Opened scene: {scenePath}");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"Scene file not found for: {sceneInfo.sceneName}");
                    }
                }
                GUILayout.Space(10);
            }

            GUILayout.Space(10);
            GUILayout.Label("\n빌드 세팅에 세팅된 씬을 모두 불러옵니다.", EditorStyles.boldLabel);

            if (GUILayout.Button("빌드 세팅의 씬 불러오기"))
            {
                if (_sceneData != null)
                {
                    _sceneData.LoadScenesFromBuildSettings();
                    EditorUtility.SetDirty(_sceneData);
                    Debug.Log("Scenes loaded from build settings.");
                }
                else
                {
                    Debug.LogError("Scene Data is not assigned!");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private string FindScenePath(string sceneName)
        {
            string[] allScenePaths = AssetDatabase.FindAssets("t:Scene", new[] { BASE_SCENE_FOLDER_PATH });

            foreach (string guid in allScenePaths)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == sceneName)
                {
                    return path;
                }
            }

            return null;
        }
        
        private void ExecuteOption(int index)
        {
            if (Managers.Game == null)
                return;
            
            switch (index)
            {
                case 0:
                    Managers.Game.playerData.Hp += 500;
                    Debug.Log("Managers.Game.playerData.Hp += 500");
                    break;
                case 1:
                    Managers.Game.playerData.Mp += 500;
                    Debug.Log("Managers.Game.playerData.Mp += 500;");
                    break;
                case 2:
                    Managers.Game.playerData.Hxp += 450;
                    Debug.Log("Managers.Game.playerData.Hxp += 450;");
                    break;
                case 3:
                    Managers.Game.playerData.Hp -= 500;
                    Debug.Log("Managers.Game.playerData.Hp -= 500;");
                    break;
                case 4:
                    Managers.Game.playerData.Mp -= 500;
                    Debug.Log("Managers.Game.playerData.Mp -= 500;");
                    break;
                default:
                    Debug.LogWarning("Invalid Option Selected");
                    break;
            }
        }
    }
}
