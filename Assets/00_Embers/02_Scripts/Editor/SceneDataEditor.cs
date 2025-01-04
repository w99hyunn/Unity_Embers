using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace STARTING
{
    public class SceneDataEditor : EditorWindow
    {
        private SceneData _sceneData;

        private const string BASE_SCENE_FOLDER_PATH = "Assets/00_Embers/01_Scenes/";
        private bool _loadAdditively = false;

        [MenuItem("STARTING/Scene Data Wizard")]
        public static void ShowWindow()
        {
            GetWindow<SceneDataEditor>("Scene Data Wizard");
        }

        private void OnEnable()
        {
            _sceneData = Resources.Load<SceneData>("SceneData");
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/00_Embers/04_Images/Editor/GameLevel Icon.png");
            titleContent = new GUIContent("Scene Data Wizard", icon);
        }

        private void OnGUI()
        {
            _sceneData = (SceneData)EditorGUILayout.ObjectField("Scene Data", _sceneData, typeof(SceneData), false);

            if (_sceneData == null)
            {
                EditorGUILayout.HelpBox("Scene Data not found at specified path!", MessageType.Error);
                return;
            }

            GUILayout.Label("\n씬 전환 :", EditorStyles.boldLabel);

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

            GUILayout.Space(20);
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
    }
}