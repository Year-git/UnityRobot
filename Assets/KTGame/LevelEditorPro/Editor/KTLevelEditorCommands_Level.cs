using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;

[System.Serializable]
public class KTLevelEditorCommands_Level
{
    private Scene _lastLevelScene;
    private IEnumerator barracksIter;
    private IEnumerator barracksBatchIter;

    private KTLevel GetLevel()
    {
        return Object.FindObjectOfType<KTLevel>();
    }

    private Scene GetLevelScene()
    {
        var level = GetLevel();
        if (level == null)
        {
            return new Scene();
        }
        return level.gameObject.scene;
    }

    [ButtonGroup("LevelGroup")]
    [Button("新建关卡", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public KTLevel NewLevel()
    {
        return NewLevel(true);
    }

    private KTLevel NewLevel(bool createDefaultObjects)
    {
        var levelScene = GetLevelScene();
        if (levelScene.isLoaded)
        {
            if (!CloseLevel())
            {
                return null;
            }
        }

        var activeScene = SceneManager.GetActiveScene();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        var level = new GameObject("$Level").AddComponent<KTLevel>();
        if (createDefaultObjects)
        {
            level.CreateDefaultGameObjects();
        }
        EditorSceneManager.SetActiveScene(activeScene);
        return level;
    }


    [ButtonGroup("LevelGroup")]
    [Button("打开关卡", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public KTLevel LoadLevel()
    {
        var levelScene = GetLevelScene();
        if (levelScene.isLoaded)
        {
            if (!CloseLevel())
            {
                return null;
            }
        }

        var sceneAssetPath = EditorUtility.OpenFilePanel("Select Scene", "Assets/levels", "unity");
        if (string.IsNullOrEmpty(sceneAssetPath))
        {
            return null;
        }

        var scene = EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Additive);
        var levelGo = GameObject.Find("/$Level");
        if (levelGo == null)
        {
            Debug.LogErrorFormat("Invalid Level Scene {0} KTLevel Object Not Found", sceneAssetPath);
            return null;
        }

        var level = levelGo.GetComponent<KTLevel>();
        if (level == null)
        {
            Debug.LogErrorFormat("Invalid Level Scene {0} KTLevel Component Not Found", sceneAssetPath);
            return null;
        }
        return level;
    }


    [ButtonGroup("LevelGroup")]
    [Button("保存关卡", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public void SaveLevel()
    {
        var levelScene = GetLevelScene();
        if (levelScene.isLoaded)
        {
            EditorSceneManager.SaveScene(levelScene);
        }
    }


    [ButtonGroup("LevelGroup")]
    [Button("另存关卡", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public void SaveAsLevel()
    {
        var levelScene = GetLevelScene();
		if (levelScene.isLoaded)
		{
			var newScenePath = EditorUtility.SaveFilePanelInProject("Scene Save Path", string.Format("{0}_bak", levelScene.name), "unity", "Save As Scene");
			if (string.IsNullOrEmpty(newScenePath))
			{
				return;
			}

			var savedScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

			var prevActiveScene = SceneManager.GetActiveScene();
			SceneManager.SetActiveScene(savedScene);
			CopySceneRoots(levelScene, savedScene);
			RegenerateUUIDOfScene(savedScene);
			EditorSceneManager.SaveScene(savedScene, newScenePath);
			SceneManager.SetActiveScene(prevActiveScene);

			SceneManager.UnloadSceneAsync(savedScene);
		}
    }

    private void CopySceneRoots(Scene srcScene, Scene dstScene)
    {
        var allRoots = srcScene.GetRootGameObjects();
        foreach (var root in allRoots)
        {
            var go = Object.Instantiate(root);
            go.name = root.name;
        }
    }

    public void RegenerateUUIDOfLevelScene(bool force)
    {
        var levelScene = GetLevelScene();
        if (levelScene.isLoaded)
        {
            RegenerateUUIDOfScene(levelScene);
        }
    }

    public void RegenerateUUIDOfScene(Scene scene)
    {
        var allRoots = scene.GetRootGameObjects();
        foreach (var rootGo in allRoots)
        {
            var allEntities = rootGo.GetComponentsInChildren<KTLevelEntity>();
            foreach (var entity in allEntities)
            {
                entity.GenerateUUID();
            }
        }
    }

    [ButtonGroup("LevelGroup")]
    [Button("导出关卡", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public void ExportLevel()
    {
        var levelScene = GetLevelScene();
        if (levelScene.isLoaded)
        {
            var path = EditorUtility.SaveFilePanel("导出关卡", "", "level.json", "json");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            RegenerateUUIDOfScene(levelScene);

            new KTLevelJsonTranslator(GetLevel()).Export(path);

			AssetDatabase.Refresh();
        }
    }


    [ButtonGroup("LevelGroup")]
    [Button("关闭关卡", ButtonSizes.Medium)]
    [GUIColor(1.0f, 0.4f, 0.1f)]
    public bool CloseLevel()
    {
        var levelScene = GetLevelScene();
        if (levelScene.isLoaded)
        {
            if (levelScene.isDirty && !EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { levelScene }))
            {
                return false;
            }
            EditorSceneManager.CloseScene(levelScene, true);
        }
        return true;
    }

}
