using UniRx;
using System;

static public class BoolReactivePropertyExtension
{
    static public CompositeDisposable SubToggle<T>(this BoolReactiveProperty reactiveBool,
                                                   Action<bool> onToggle,
                                                   IObservable<T> otherStream,
                                                   Action<T> onOtherStreamObject)
    {
        CompositeDisposable disposables = new();

        reactiveBool
            .Subscribe(onToggle)
            .AddTo(disposables);

        reactiveBool
            .SubToggle(otherStream, onOtherStreamObject)
            .AddTo(disposables);

        return disposables;
    }

    static public CompositeDisposable SubToggle<T>(this BoolReactiveProperty reactiveBool,
                                                   IObservable<T> otherStream,
                                                   Action<T> onOtherStreamObject)
    {
        CompositeDisposable disposables = new();

        reactiveBool
            .WhereEqual(true)
            .Subscribe(toggle =>
            {
                otherStream
                    .TakeUntil(reactiveBool.WhereEqual(false))
                    .Subscribe(onOtherStreamObject)
                    .AddTo(disposables);
            })
            .AddTo(disposables);

        return disposables;
    }
}
