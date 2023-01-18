using UnityEngine;

namespace MB6.ScreenFade
{
    public class LinearEase : IEaseFunction
    {
        [Header("Linear Ease Function")]
        [Range(0f, 1f)]
        public float StartValue;
        [Range(0f, 1f)]
        public float EndValue;

        public LinearEase() : this(0f, 1f)
        {
        }

        public LinearEase(float startValue, float endValue)
        {
            StartValue = startValue;
            EndValue = endValue;
        }

        public float Evaluate(float time)
        {
            return Mathf.Lerp(StartValue, EndValue, time);
        }
        
    }
}