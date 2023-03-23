using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace GrapeWidgets
{
    public class WidgetManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject widgetsRoot;
        [SerializeField] private GameObject openWidgetsRoot;

        [SerializeField] private GameObject background;
        [SerializeField] private Cover cover;

        [Tooltip("Script should be placed on camera")]
        [SerializeField]
        private Screenshot screenshot;

        [Tooltip("Path to load from like Resources.Load<Type>(path + Type)")]
        [SerializeField] private string pathAtResources = "Widgets";

        private Dictionary<Type, BaseWidget> widgets = new Dictionary<Type, BaseWidget>();
        private List<BaseWidget> openWidgets = new List<BaseWidget>();
        private Task animationTransition = Task.CompletedTask;

        void Awake()
        {
            Assert.IsNotNull(canvas);
            Assert.IsNotNull(widgetsRoot);
            Assert.IsNotNull(openWidgetsRoot);
            Assert.IsNotNull(background);
            Assert.IsNotNull(cover);

            background.transform.SetParent(openWidgetsRoot.transform);
            
            cover.transform.SetParent(openWidgetsRoot.transform);
            cover.Init(Camera.main.GetComponent<Screenshot>());
            cover.OnCoverClick += OnCoverClick;

            ReorderWidgets();
        }

        public async Task Show<WidgetType>()
            where WidgetType : BaseWidget
        {
            await Show<WidgetType>(null);
        }

        public async Task Show<WidgetType>(object initData)
            where WidgetType : BaseWidget
        {
            BaseWidget widget;
            if (!widgets.TryGetValue(typeof(WidgetType), out widget))
            {
                var prefab = Resources.Load<WidgetType>($"{pathAtResources}/{typeof(WidgetType)}");
                widget = GameObject.Instantiate<WidgetType>(prefab, widgetsRoot.transform);

                widgets.Add(typeof(WidgetType), widget);
            }

            if (animationTransition != null)
                await animationTransition;

            AddToOpen(widget);

            await widget.Init(initData, this);
            widget.gameObject.SetActive(true);
            await widget.Show();

            await Task.CompletedTask;
        }

        private void AddToOpen(BaseWidget widget)
        {
            if (!openWidgets.Contains(widget))
                openWidgets.Add(widget);
            else
                Debug.LogError($"widget is already open: {widget}");

            widget.transform.SetParent(openWidgetsRoot.transform);
            widget.transform.SetAsLastSibling();

            ReorderWidgets();
        }

        public async Task Hide<WidgetType>(WidgetType widget)
            where WidgetType : BaseWidget
        {
            if (widgets.ContainsValue(widget))
                await HideWidget(widget);
            else
                Debug.LogFormat("cant hide never opened widget: {0}", widget.name);

            await Task.CompletedTask;
        }

        public async Task Hide<WidgetType>()
            where WidgetType : BaseWidget
        {
            if (widgets.ContainsKey(typeof(WidgetType)))
            {
                var widget = widgets[typeof(WidgetType)];
                await HideWidget(widget);
            }
            else
                Debug.LogFormat("cant hide never opened widget: {0}", typeof(WidgetType));

            await Task.CompletedTask;
        }

        private async Task HideWidget(BaseWidget widget)
        {
            if (!widget.gameObject.activeSelf)
                Debug.LogWarningFormat("widget hasnt been active: {0}", widget.name);

            await animationTransition;

            openWidgets.Remove(widget);
            widget.transform.SetParent(widgetsRoot.transform);
            widget.gameObject.SetActive(false);
            ReorderWidgets();

            await widget.Hide();

            await Task.CompletedTask;
        }

        public async Task Close<WidgetType>()
            where WidgetType : BaseWidget
        {
            if (widgets.ContainsKey(typeof(WidgetType)))
            {
                var widget = widgets[typeof(WidgetType)];
                await HideWidget(widget);
                widgets.Remove(typeof(WidgetType));
                GameObject.Destroy(widget.gameObject);
            }
            else
                Debug.LogFormat("cant close never opened widget: {0}", typeof(WidgetType));

            await Task.CompletedTask;
        }

        public async Task CloseAllHidden()
        {
            await animationTransition;

            List<Type> keys = new List<Type>(widgets.Keys);
            foreach (var key in keys)
            {
                var widget = widgets[key];
                if (openWidgets.Contains(widget))
                    continue;

                widgets.Remove(key);
                GameObject.Destroy(widget.gameObject);
            }

            await Task.CompletedTask;
        }

        private void ReorderWidgets()
        {
            bool hideOther = false;
            BaseWidget widgetWithBackground = null;
            BaseWidget coveredWidget = null;

            // from the end to the background
            for (int i = openWidgets.Count - 1; i >= 0; i--)
            {
                var widget = openWidgets[i];

                if (widget.type == BaseWidget.WidgetTypes.loading)
                {
                    widget.transform.SetAsLastSibling();
                    break;
                }

                if (hideOther && widget.transform.parent != widgetsRoot.transform)
                {
                    widget.gameObject.SetActive(false);
                    continue;
                }

                widget.gameObject.SetActive(true);

                if (widget.backgroundType == BaseWidget.BackgroundType.image)
                {
                    hideOther = true;
                    widgetWithBackground = widget;
                }

                switch (widget.coverType)
                {
                    case BaseWidget.ClickCoverType.none:
                        break;
                    case BaseWidget.ClickCoverType.transparent:
                    case BaseWidget.ClickCoverType.close:
                        coveredWidget = widget;
                        break;
                }
            }

            // put background and cover into a right place
            if (widgetWithBackground != null)
            {
                background.SetActive(true);
                background.transform.SetSiblingIndex(widgetWithBackground.transform.GetSiblingIndex() - 1);
            }
            else
            {
                background.SetActive(false);
            }

            if (coveredWidget != null)
            {
                cover.transform.SetSiblingIndex(coveredWidget.transform.GetSiblingIndex() - 1);
                cover.gameObject.SetActive(true);
            }
            else
            {
                cover.gameObject.SetActive(false);
            }
        }

        public async void OnCoverClick()
        {
            if (openWidgets.Count <= 0)
                return;

            if (!animationTransition.IsCompleted)
                return;

            var widget = openWidgets[openWidgets.Count - 1];
            if (widget.coverType == BaseWidget.ClickCoverType.close)
                await HideWidget(widget);
        }
    }
}
