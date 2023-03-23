using System.Threading.Tasks;
using UnityEngine;

namespace GrapeWidgets.Animations
{
    public class ControlScaleAnimation : MonoBehaviour
    {
        [SerializeField] AnimationCurve curve;
        [SerializeField] [Range(0, 3)] float duration = 1f;
        [SerializeField] float scaleAmount = 1f;

        void Awake()
        {
            if (curve == null)
                Debug.LogError($"curve is null at: {name}");

            //hud = this.GetComponent<UnitHud>();
            //if (!hud)
            //    Debug.LogError($"cant find UnitHud at: {name}");
            //else
            //    hud.OnValueChanged += OnValueUpdated;
        }

        private async Task OnValueUpdated(GameObject gameObject)
        {
            if (!this.isActiveAndEnabled)
                return;

            await AnimationRoutine(gameObject);
        }

        private async Task AnimationRoutine(GameObject gameObject)
        {
            float time = 0f;

            var startScale = gameObject.transform.localScale;

            while (time < duration)
            {
                var progress = curve.Evaluate(time);
                gameObject.transform.localScale = startScale + (Vector3.one * scaleAmount * progress);
                time += Time.deltaTime;

                //yield return new WaitForEndOfFrame();
                //await Task.Yield();
                await Task.Delay(200);
            }

            gameObject.transform.localScale = startScale;
        }
    }
}
