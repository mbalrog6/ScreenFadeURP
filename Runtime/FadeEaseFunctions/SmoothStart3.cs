using UnityEngine;

namespace MB6.ScreenFade
{
    public class SmoothStart3 : IEaseFunction
    {
        [Header("SmoothStart3 Ease Function")]
        [Range(0f, 1f)]
        public float StartValue;
        [Range(0f, 1f)]
        public float EndValue;

        private bool _directionSet;
        private float direction; 

        public SmoothStart3() : this(0f, 1f)
        {
        }
        public SmoothStart3(float startValue, float endValue)
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
                return Mathf.Clamp((time * time * time) * (EndValue - StartValue) + StartValue, 0f, 1f);
            }
            else
            {
                return Mathf.Clamp(StartValue - ((time * time * time) * (StartValue-EndValue)), 0f, 1f);
            }
        }
        
    }
}