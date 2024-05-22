using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Fade))]
public class WeakPointView : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] Button button;

    public class Pool : MonoMemoryPool<WeakPointView>
    {
        public Action onWeakPointClicked;

        protected override void OnSpawned(WeakPointView item)
        {
            base.OnSpawned(item);

            item.button.interactable = true;

            item.fade.FadeIn();
        }

        protected override void OnCreated(WeakPointView item)
        {
            base.OnCreated(item);

            item.button.onClick.AddListener(() =>
            {
                DisableButtonAndDespawn(item);

                this.onWeakPointClicked.Invoke();
            });
        }

        public void DisableButtonAndDespawn(WeakPointView item)
        {
            item.button.interactable = false;

            var cancel = item.GetCancellationTokenOnDestroy();

            item.fade
                .FadeOut()
                .AttachExternalCancellation(cancel)
                .SuppressCancellationThrow()
                .ContinueWith(isCanceled =>
                {
                    if (isCanceled)
                        return;

                    Despawn(item);
                })
                .Forget();
        }
    }
}
