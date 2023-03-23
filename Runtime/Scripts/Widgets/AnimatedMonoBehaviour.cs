using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrapeWidgets.Animations
{
    public abstract class AnimatedMonoBehaviour : MonoBehaviour
    {
        public virtual float Alpha { set { } }
        public virtual Vector3 Scale { set { } }
    }
}
