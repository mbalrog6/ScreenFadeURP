using System;
using UnityEngine;

namespace MB6.ScreenFade
{
    public class ScreenFade : MonoBehaviour
    {
        private readonly int _shaderAlphaParameter = Shader.PropertyToID("_Alpha");
        private readonly int _shaderFadeColorParameter = Shader.PropertyToID("_FadeColor");
        public bool IsFadedIn { get; private set; }

        [Header("Dependancies")]
        [SerializeField]
        [Tooltip("This is the screen fade feature")] 
        private ScreenFadeFeature _screenFadeFeature = null;
        
        [Header("Parameters")]
        [SerializeField]
        [Tooltip("Default Color to Fade To...")]
        private Color _fadeToColor = Color.black;
        
        [SerializeField]
        [Tooltip("Default Duration of the Fade")]
        private float _durationOfFade = 1f;

        [SerializeField] 
        [Tooltip("If Checked the screen will start Faded to the Default Color with the Alpha set below.")] 
        private bool _shouldStartFadedIn;
        
        [SerializeField] 
        [Tooltip("If Should Start Faded In is check this is the alpha value that is used.")] 
        private float _startFadedAlpha = 1f;

        [Header("Ease Functions")]
        [SerializeField, SerializeReference]
        private IEaseFunction _fadeInEaseFunction;
        
        [SerializeField, SerializeReference]
        private IEaseFunction _fadeOutEaseFunction;
        
        private Material _fadeMaterial = null;
        private float? _timer = null;
        private float? _delayTimer = null;
        private float _workingDuration;

        public Action<bool> FinishedFading;

        public void Awake()
        {
            _fadeMaterial = Instantiate(_screenFadeFeature.Settings.Material);
            _screenFadeFeature.Settings.RunTimeMaterial = _fadeMaterial;
            _fadeMaterial.SetColor(_shaderFadeColorParameter, _fadeToColor);

            if (!HasFadeInEase())
            {
                _fadeInEaseFunction = new LinearEase(0f, 1f);
            }

            if (!HasFadeOutEase())
            {
                _fadeOutEaseFunction = new LinearEase(1f, 0f);
            }
            

            if (_shouldStartFadedIn)
            {
                IsFadedIn = true;
                _fadeMaterial.SetFloat(_shaderAlphaParameter, _startFadedAlpha);
            }
            else
            {
                IsFadedIn = false;
                _fadeMaterial.SetFloat(_shaderAlphaParameter, 0);
            }
        }

        private void Update()
        {
            if (!_timer.HasValue) return;
            if (_delayTimer.HasValue)
            {
                _delayTimer -= Time.deltaTime;
                if (_delayTimer.Value < 0f)
                {
                    _delayTimer = null;
                }
                return;
            }

            _timer += Time.deltaTime;

            if (IsFadedIn)
            {
                float value = _fadeInEaseFunction.Evaluate(_timer.Value / _workingDuration);
                _fadeMaterial.SetFloat(_shaderAlphaParameter, value);
            }
            else
            {
                float value = _fadeOutEaseFunction.Evaluate(_timer.Value / _workingDuration);
                _fadeMaterial.SetFloat(_shaderAlphaParameter, value);
            }

            if (_timer.Value > _workingDuration)
            {
                _timer = null;
                FinishedFading?.Invoke(IsFadedIn);
            }
        }

        public void Fade(bool fade, float? delayTime = null, Color? color = null, float? duration = null)
        {
            IsFadedIn = fade;
            _timer = 0f;

            _workingDuration = duration.HasValue ? duration.Value : _durationOfFade;
            
            if (color.HasValue)
            {
                _fadeMaterial.SetColor(_shaderFadeColorParameter, color.Value);
            }
            
            if (delayTime.HasValue && fade == false)
            {
                _delayTimer = delayTime;
            }
        }

        public void SetFadeInEase(IEaseFunction easeFunction) => _fadeInEaseFunction = easeFunction;
        public void SetFadeOutEase(IEaseFunction easeFunction) => _fadeOutEaseFunction = easeFunction;
        public bool HasFadeInEase() => _fadeInEaseFunction != null;
        public bool HasFadeOutEase() => _fadeOutEaseFunction != null;
    }
}