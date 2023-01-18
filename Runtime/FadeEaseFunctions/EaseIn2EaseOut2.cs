using UnityEngine;

namespace MB6.URP.Fade
{
    public class EaseIn2EaseOut2 : IEaseFunction
    {
        [Header("EaseIn2 EaseOut2 Function")]
        [Range(0f, 1f)]
        public float StartValue;
        [Range(0f, 1f)]
        public float EndValue;

        private IEaseFunction _easeIn2;
        private IEaseFunction _easeOut2;
        private bool _valuesSet;

        public EaseIn2EaseOut2() : this(0f, 1f)
        {
            
        }

        public EaseIn2EaseOut2(float startValue, float endValue)
        {
            _easeIn2 = new SmoothStart2(StartValue, EndValue);
            _easeOut2 = new SmoothStop2(StartValue, EndValue);
            StartValue = startValue;
            EndValue = endValue;
            _valuesSet = false;
        }


        public float Evaluate(float time)
        {
            if (!_valuesSet)
            {
                _valuesSet = true;
                ((SmoothStart2)_easeIn2).StartValue = StartValue;
                ((SmoothStop2)_easeOut2).StartValue = StartValue;
                ((SmoothStart2)_easeIn2).EndValue = EndValue;
                ((SmoothStop2)_easeOut2).EndValue = EndValue;
            }
            var func1Result = _easeIn2.Evaluate(time);
            var result = func1Result - time * (_easeOut2.Evaluate(time) - func1Result);
            return result;
        }
    }
}