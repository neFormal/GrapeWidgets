using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GrapeWidgets
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RawImage))]
    //[RequireComponent(typeof(Blur))]
    public class Cover : MonoBehaviour, IPointerClickHandler
    {
        private Screenshot screenshot;

        private CanvasRenderer _renderer;
        private RawImage image;
        private RenderTexture renderTexture;
        private RenderTexture blurTexture;
        private Blur blur;

        public event Action OnCoverClick = () => { };

        void Awake()
        {
            _renderer = this.GetComponent<CanvasRenderer>();
            image = this.GetComponent<RawImage>();
            blur = this.GetComponent<Blur>();
        }

        public void Init(Screenshot screenshot)
        {
            this.screenshot = screenshot;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnCoverClick();
        }

        async void OnEnable()
        {
            image.enabled = false;
            image.texture = null;
            _renderer.SetAlpha(1.0f);

            if (screenshot == null)
                return;

            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
                renderTexture.useMipMap = false;
            }

            if (blurTexture == null)
            {
                blurTexture = new RenderTexture(Screen.width, Screen.height, 16);
                blurTexture.useMipMap = false;
            }

            await screenshot.Make(renderTexture);
            image.texture = renderTexture;
            image.enabled = true;

            if (blur != null)
                await blur.BlurIt(renderTexture, blurTexture);
        }
    }
}
