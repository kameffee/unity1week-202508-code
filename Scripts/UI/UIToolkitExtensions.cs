using System;
using R3;
using UnityEngine.UIElements;

namespace Unity1week202508.UI
{
    /// <summary>
    /// UI ToolkitとR3を統合するための拡張メソッド
    /// </summary>
    public static class UIToolkitExtensions
    {
        /// <summary>
        /// ButtonのクリックイベントをObservableに変換
        /// </summary>
        public static Observable<Unit> OnClickAsObservable(this Button button)
        {
            return Observable.FromEvent<EventCallback<ClickEvent>, ClickEvent>(
                handler => evt => handler(evt),
                handler => button.clicked += () => handler(new ClickEvent()),
                handler => button.clicked -= () => handler(new ClickEvent())
            ).AsUnitObservable();
        }

        /// <summary>
        /// Sliderの値変更イベントをObservableに変換
        /// </summary>
        public static Observable<float> OnValueChangedAsObservable(this Slider slider)
        {
            return Observable.FromEvent<EventCallback<ChangeEvent<float>>, ChangeEvent<float>>(
                handler => evt => handler(evt),
                handler => slider.RegisterValueChangedCallback(handler),
                handler => slider.UnregisterValueChangedCallback(handler)
            ).Select(evt => evt.newValue);
        }

        /// <summary>
        /// ToggleButtonの値変更イベントをObservableに変換
        /// </summary>
        public static Observable<bool> OnValueChangedAsObservable(this Toggle toggle)
        {
            return Observable.FromEvent<EventCallback<ChangeEvent<bool>>, ChangeEvent<bool>>(
                handler => evt => handler(evt),
                handler => toggle.RegisterValueChangedCallback(handler),
                handler => toggle.UnregisterValueChangedCallback(handler)
            ).Select(evt => evt.newValue);
        }

        /// <summary>
        /// TextFieldの値変更イベントをObservableに変換
        /// </summary>
        public static Observable<string> OnValueChangedAsObservable(this TextField textField)
        {
            return Observable.FromEvent<EventCallback<ChangeEvent<string>>, ChangeEvent<string>>(
                handler => evt => handler(evt),
                handler => textField.RegisterValueChangedCallback(handler),
                handler => textField.UnregisterValueChangedCallback(handler)
            ).Select(evt => evt.newValue);
        }

        /// <summary>
        /// DropdownFieldの値変更イベントをObservableに変換
        /// </summary>
        public static Observable<string> OnValueChangedAsObservable(this DropdownField dropdownField)
        {
            return Observable.FromEvent<EventCallback<ChangeEvent<string>>, ChangeEvent<string>>(
                handler => evt => handler(evt),
                handler => dropdownField.RegisterValueChangedCallback(handler),
                handler => dropdownField.UnregisterValueChangedCallback(handler)
            ).Select(evt => evt.newValue);
        }

        /// <summary>
        /// VisualElementのマウスエンターイベントをObservableに変換
        /// </summary>
        public static Observable<Unit> OnMouseEnterAsObservable(this VisualElement element)
        {
            return Observable.FromEvent<EventCallback<MouseEnterEvent>, MouseEnterEvent>(
                handler => evt => handler(evt),
                handler => element.RegisterCallback<MouseEnterEvent>(handler),
                handler => element.UnregisterCallback<MouseEnterEvent>(handler)
            ).AsUnitObservable();
        }

        /// <summary>
        /// VisualElementのマウスリーブイベントをObservableに変換
        /// </summary>
        public static Observable<Unit> OnMouseLeaveAsObservable(this VisualElement element)
        {
            return Observable.FromEvent<EventCallback<MouseLeaveEvent>, MouseLeaveEvent>(
                handler => evt => handler(evt),
                handler => element.RegisterCallback<MouseLeaveEvent>(handler),
                handler => element.UnregisterCallback<MouseLeaveEvent>(handler)
            ).AsUnitObservable();
        }

        /// <summary>
        /// VisualElementの表示/非表示を切り替え
        /// </summary>
        public static void SetVisible(this VisualElement element, bool visible)
        {
            if (visible)
            {
                element.RemoveFromClassList("hidden");
                element.style.display = DisplayStyle.Flex;
            }
            else
            {
                element.AddToClassList("hidden");
                element.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// VisualElementが表示されているかどうかを取得
        /// </summary>
        public static bool IsVisible(this VisualElement element)
        {
            return !element.ClassListContains("hidden") && 
                   element.style.display != DisplayStyle.None;
        }

        /// <summary>
        /// アニメーション用のフェードイン
        /// </summary>
        public static void FadeIn(this VisualElement element, float duration = 0.3f)
        {
            element.SetVisible(true);
            element.style.opacity = 0f;
            // 簡単なフェードイン（アニメーションライブラリなしの場合）
            element.style.opacity = 1f;
        }

        /// <summary>
        /// アニメーション用のフェードアウト
        /// </summary>
        public static void FadeOut(this VisualElement element, float duration = 0.3f, Action onComplete = null)
        {
            element.style.opacity = 0f;
            // フェードアウト完了後の処理
            element.SetVisible(false);
            onComplete?.Invoke();
        }
    }
}