using GrapeWidgets.Animations;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GrapeWidgets
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseWidget : AnimatedMonoBehaviour
    {
        public event Action OnClose = () => { };
        [Serializable]
        public enum WidgetTypes
        {
            loading = -1,
            common = 0,
            modal
        }

        [Serializable]
        public enum BackgroundType
        {
            none = 0,
            image
        }

        [Serializable]
        public enum ClickCoverType
        {
            none = 0,
            transparent,
            close
        }

        [SerializeField] public WidgetTypes type = WidgetTypes.common;

        [SerializeField] public BackgroundType backgroundType = BackgroundType.none;
        [SerializeField] public ClickCoverType coverType = ClickCoverType.none;

        private WidgetManager widgetManager;

        private CanvasGroup canvasGroup;
        public override float Alpha { set { canvasGroup.alpha = value; } }
        public override Vector3 Scale { set { gameObject.transform.localScale = value; } }

        protected object initData;

        protected virtual void Awake()
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
        }

        public async virtual Task Init(object initData, WidgetManager manager)
        {
            this.initData = initData;
            this.widgetManager = manager;
            await Task.CompletedTask;
        }

        protected virtual void Clear()
        {
            OnClose = () => { };
        }

        protected async void Close()
        {
            await widgetManager.Hide(this);
        }

        protected async void Close<T>(T widget) where T : BaseWidget
        {
            await widgetManager.Hide<T>();
        }

        #region animation
        public async virtual Task Show()
        {
            var animation = GetComponent<WidgetAnimation>();
            if (this.gameObject.activeSelf && animation != null)
                await animation.Show();
            else
                await Task.CompletedTask;
        }

        public async virtual Task Hide()
        {
            var animation = GetComponent<WidgetAnimation>();
            if (this.gameObject.activeSelf && animation != null)
                await animation.Hide();
            else
                await Task.CompletedTask;

            var close = OnClose;
            Clear();
            close();
        }
        #endregion

    }
}
