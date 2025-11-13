using UnityEngine;
using UnityEngine.UI; // Required for the Selectable class
using UnityEngine.EventSystems;
using System.Collections;

public class BouncyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Tooltip("The transform to scale. If left null, it will use this object's transform.")]
    public Transform targetTransform;

    [Tooltip("How small the button gets when pressed down.")]
    [Range(0.5f, 1.0f)]
    public float pressScale = 0.9f;

    [Tooltip("How much the button overshoots its original size when released.")]
    [Range(1.0f, 1.5f)]
    public float bounceScale = 1.1f;

    [Tooltip("The speed of the press and bounce animation.")]
    public float animationSpeed = 10f;

    private Vector3 _initialScale;
    private Coroutine _currentAnimation;
    private bool _isPointerDown = false;

    private Selectable _selectable;

    private void Awake()
    {
        if (targetTransform == null)
        {
            targetTransform = this.transform;
        }
        _initialScale = targetTransform.localScale;

        TryGetComponent<Selectable>(out _selectable);
    }
    
    private void Update()
    {
        if (_selectable != null && !_selectable.interactable && _isPointerDown)
        {
            _isPointerDown = false;
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }
            targetTransform.localScale = _initialScale;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_selectable != null && !_selectable.interactable) return;

        _isPointerDown = true;
        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
        }
        _currentAnimation = StartCoroutine(AnimateScale(pressScale * _initialScale));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_selectable != null && !_selectable.interactable) return;
        
        if (!_isPointerDown) return;
        _isPointerDown = false;

        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
        }
        _currentAnimation = StartCoroutine(AnimateBounceBack());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isPointerDown) return; 
        
        if (_isPointerDown)
        {
            _isPointerDown = false;
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }
            _currentAnimation = StartCoroutine(AnimateScale(_initialScale));
        }
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        while (Vector3.Distance(targetTransform.localScale, targetScale) > 0.01f)
        {
            targetTransform.localScale = Vector3.Lerp(targetTransform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
            yield return null;
        }
        targetTransform.localScale = targetScale;
    }

    private IEnumerator AnimateBounceBack()
    {
        Vector3 bounceTargetScale = bounceScale * _initialScale;
        
        yield return StartCoroutine(AnimateScale(bounceTargetScale));
        yield return StartCoroutine(AnimateScale(_initialScale));
    }
}