﻿using UnityEngine;

namespace MB6.ScreenFade
{
    public class SmoothStop3 : IEaseFunction
    {
        [Header("SmoothStop3 Ease Function")]
        [Range(0f, 1f)]
        public float StartValue;
        [Range(0f, 1f)]
        public float EndValue;

        private bool _directionSet;
        private float direction; 

        public SmoothStop3() : this(0f, 1f)
        {
        }
        public SmoothStop3(float startValue, float endValue)
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
                return Mathf.Clamp((1f - ((1f-time) * (1f-time) * (1f-time))) * (EndValue - StartValue) + StartValue, 0f, 1f);
            }
            else
            {
                return Mathf.Clamp(StartValue - (1f - ((1f-time) * (1f-time) * (1f-time))) * (StartValue-EndValue), 0f, 1f);
            }
        }
    }
}