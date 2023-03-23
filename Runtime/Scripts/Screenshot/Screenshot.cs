using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GrapeWidgets
{
    [RequireComponent(typeof(Camera))]
    public class Screenshot : MonoBehaviour
    {
        void Awake()
        {
            this.enabled = false;
        }

        private RenderTexture renderTexture;
        private Texture2D tempTex;

        private TaskCompletionSource<bool> completionSource;

        public Task Make(RenderTexture texture)
        {
            this.enabled = true;
            this.renderTexture = texture;

            completionSource = new TaskCompletionSource<bool>();
            return completionSource.Task;
        }

        private void OnEnable()
        {
            RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        }

        private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (renderTexture != null)
            {
                tempTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
                tempTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                tempTex.Apply();

                renderTexture.DiscardContents();
                Graphics.CopyTexture(tempTex, renderTexture);
                renderTexture = null;

                completionSource.SetResult(true);
            }

            this.enabled = false;
        }
    }
}
