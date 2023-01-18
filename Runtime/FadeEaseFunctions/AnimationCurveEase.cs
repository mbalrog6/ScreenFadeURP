using System;
using UnityEngine;

namespace MB6.ScreenFade
{
    [Serializable]
    public class AnimationCurveEase : IEaseFunction
    {
        [Header("AnimationCurve Ease")]
        public AnimationCurve _animationCurve;

        public AnimationCurveEase() : this(
            new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 0f),
                new Keyframe(1f, 1f)
            }))
        {
            
        }
        public AnimationCurveEase(AnimationCurve animationCurve)
        {
            _animationCurve = animationCurve;
        }
        public float Evaluate(float time)
        {
            return _animationCurve.Evaluate(time);
        }
    }
}