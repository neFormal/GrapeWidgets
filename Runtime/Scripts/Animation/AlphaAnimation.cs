using System.Threading.Tasks;
using UnityEngine;

namespace GrapeWidgets.Animations
{
    [RequireComponent(typeof(AnimatedMonoBehaviour))]
    public class AlphaAnimation : WidgetAnimation
    {
        [SerializeField] private float from = 0f;
        [SerializeField] private float to = 1f;
        [SerializeField] private float duration = 1f;

        public override async Task Show()
        {
            await PlayAnim(from, to);
        }

        public override async Task Hide()
        {
            await PlayAnim(to, from);
        }

        private async Task PlayAnim(float from, float to)
        {
            var widget = this.GetComponent<AnimatedMonoBehaviour>();

            var endTime = Time.time + duration;
            widget.Alpha = from;

            while (Time.time < endTime)
            {
                var value = Mathf.Lerp(from, to, Mathf.Clamp((duration - (endTime - Time.time)) / duration, 0f, 1f));

                widget.Alpha = value;
                //yield return new WaitForEndOfFrame();
                //await Task.Yield();
                await Task.Delay(200);
            }

            widget.Alpha = to;
            //Debug.LogFormat("alpha {0} => {1}", from, to);
        }
    }
}
