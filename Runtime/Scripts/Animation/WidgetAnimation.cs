using System.Threading.Tasks;
using UnityEngine;

namespace GrapeWidgets.Animations
{
    [RequireComponent(typeof(BaseWidget))]
    public abstract class WidgetAnimation : MonoBehaviour
    {
        //protected Task currentAnimation = Task.CompletedTask;
        protected AnimatedMonoBehaviour widget;

        protected void Awake()
        {
            widget = this.GetComponent<BaseWidget>();
        }

        public virtual async Task Show()
        {
            //await currentAnimation;
            await Task.CompletedTask;
        }

        public async virtual Task Hide()
        {
            //await currentAnimation;
            await Task.CompletedTask;
        }
    }
}
