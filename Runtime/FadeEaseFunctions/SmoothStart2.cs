using UnityEngine;

namespace MB6.URP.Fade
{
    public class SmoothStart2 : IEaseFunction
    {
        [Header("SmoothStart2 Ease Function")]
        [Range(0f, 1f)]
        public float StartValue;
        [Range(0f, 1f)]
        public float EndValue;

        private bool _directionSet;
        private float direction; 

        public SmoothStart2() : this(0f, 1f)
        {
        }
        public SmoothStart2(float startValue, float endValue)
        {
            StartValue = startValue;
            EndValue = endValue;

            _directionSet = false;
            SetDirection();
        }

        private void SetDirection()
        {
            if (StartValue > EndValue)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
        }

        public float Evaluate(float time)
        {
            if (_directionSet)
            {
                SetDirection();
                _directionSet = true;
            }
            
            if (direction == 1)
            {
                return Mathf.Clamp((time * time) * (EndValue - StartValue) + StartValue, 0f, 1f);
            }
            else
            {
                return Mathf.Clamp(StartValue - ((time * time) * (StartValue-EndValue)), 0f, 1f);
            }
        }
    }
}