using System.Threading.Tasks;
using UnityEngine;

namespace GrapeWidgets.Animations
{
    [RequireComponent(typeof(AnimatedMonoBehaviour))]
    public class ScaleAnimation : WidgetAnimation
    {
        [SerializeField] private float duration = 1f;

        //private TaskCompletionSource<bool> completionSource;

        public override async Task Show()
        {
            //if (currentAnimation != null && !currentAnimation.IsCompleted)
            //    Debug.LogErrorFormat("current animation is pending on show: {0}", currentAnimation.IsCompleted);

            //completionSource = new TaskCompletionSource<bool>();
            //currentAnimation = completionSource.Task;
            await PlayAnim(Vector3.zero, Vector3.one);
        }

        public override async Task Hide()
        {
            //if (currentAnimation != null && !currentAnimation.IsCompleted)
            //    Debug.LogErrorFormat("current animation is pending on hide: {0}", currentAnimation.IsCompleted);

            //completionSource = new TaskCompletionSource<bool>();
            //currentAnimation = completionSource.Task;
            await PlayAnim(Vector3.one, Vector3.zero);
        }

        private async Task PlayAnim(Vector3 from, Vector3 to)
        {
            var endTime = Time.time + duration;

            widget.Scale = from;

            while (Time.time < endTime)
            {
                var value = Vector3.Lerp(from, to, Mathf.Clamp((duration - (endTime - Time.time)) / duration, 0f, 1f));
                widget.Scale = value;

                await Task.Yield();
                //await Task.Delay(200);
            }

            gameObject.transform.localScale = to;

            //completionSource.SetResult(true);
        }
    }
}
