#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SpawnerConfigValidator
{
    static SpawnerConfigValidator()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // При попытке войти в Play Mode (запуск игры)
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Найти все объекты SpawnerConfigSO в проекте
            string[] guids = AssetDatabase.FindAssets("t:SpawnerConfigSO");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SpawnerConfigSO config = AssetDatabase.LoadAssetAtPath<SpawnerConfigSO>(path);

                if (config != null && !config.IsValid(out string errorMessage))
                {
                    // Показываем ошибку и запрещаем вход в Play Mode
                    Debug.LogError($"Запуск Play Mode заблокирован: {errorMessage}");
                    EditorApplication.isPlaying = false;
                    return;
                }
            }
        }
    }
}
#endif