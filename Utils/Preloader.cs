using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using Cysharp.Threading.Tasks;

public class Preloader : MonoBehaviour
{
    [SerializeField] public UnityEvent onAllScenesLoaded;

    async void Start()
    {
        onAllScenesLoaded.AddListener(() => Debug.Log($"All Loaded"));

        await LoadScenesAsync();

        Debug.Log($"All Loaded (clone)");
    }

    async UniTask LoadScenesAsync()
    {
        await UniTask
            .WhenAll(SceneManager
                     .sceneCountInBuildSettings
                     .ToRange()
                     .Select(i =>
                     {
                         Scene sc = default;
                         try { sc = SceneManager.GetSceneAt(i); }
                         catch
                         {
                             if (!sc.isLoaded)
                             {
                                 LoadSceneParameters parameters = new LoadSceneParameters
                                 {
                                     loadSceneMode = LoadSceneMode.Additive
                                 };

                                 return
                                     SceneManager
                                     .LoadSceneAsync(i, parameters)
                                     .ToUniTask();
                             }
                         }

                         return UniTask.CompletedTask;
                     })
            );

        onAllScenesLoaded?.Invoke();
    }
}
