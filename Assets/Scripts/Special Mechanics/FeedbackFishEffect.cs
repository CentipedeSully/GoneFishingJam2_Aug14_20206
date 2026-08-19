using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackFishEffect : MonoBehaviour
{
    private FeedbackFishSpawner _feedbackSpawner;
    [SerializeField] private float _fadeInTime = 1.5f;
    [SerializeField] private float _currentFadeTime = 0;
    [SerializeField] private bool _isFadingIn = false;
    [SerializeField] private float _dangleTime = 1.5f;
    [SerializeField] private RectTransform _bucketDropPoint;
    [SerializeField] private float _dropTime = 1;
    [SerializeField] private AnimationCurve _dropCurve;
    private float _currentDropTime = 0;
    private Vector3 _startPosition;
    private bool _isDroppingIntoBucket = false;
    private RectTransform _rectTransform;

    private Image _image;
    private Color _originColor;
    private float _alpha;




    //monobehaviours
    private void Awake()
    {
        _image = GetComponent<Image>();
        GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        _rectTransform = GetComponent<RectTransform>();

        _originColor = _image.color;
        _alpha = 0;
        _image.color = new Color(_originColor.r, _originColor.g, _originColor.b, _alpha);
        _isFadingIn = true;
    }


    private void Update()
    {
        if (_isFadingIn)
            TickFadeIn();

        else if (_isDroppingIntoBucket)
            LerpFishIntoBucket();
    }




    //internals
    private void TickFadeIn()
    {
        _currentFadeTime += Time.unscaledDeltaTime;
        _alpha = _currentFadeTime / _fadeInTime;
        _image.color = new Color(_originColor.r, _originColor.g, _originColor.b, _alpha);

        if (_currentFadeTime >= _fadeInTime)
        {
            _isFadingIn = false;
            Invoke(nameof(DropFishIntoBucket), _dangleTime);
        }
    }

    private void DropFishIntoBucket()
    {
        _isDroppingIntoBucket = true;
        _startPosition = _rectTransform.position;
    }

    private void LerpFishIntoBucket()
    {
        _currentDropTime += Time.unscaledDeltaTime;

        float curveProgress = _dropCurve.Evaluate(_currentDropTime / _dropTime);



        _rectTransform.position = Vector3.Lerp(_startPosition, _bucketDropPoint.position, curveProgress);

        if (_currentDropTime >= _dropTime)
        {
            _isDroppingIntoBucket = false;
            _feedbackSpawner.PerformBucketReaction();
            Destroy(gameObject);
        }
    }


    //Externals
    public void SetBucketDropPosition(RectTransform bucketPoint)
    {
        _bucketDropPoint = bucketPoint;
    }
    public void SetFeedbackSpawner(FeedbackFishSpawner spawner) {  _feedbackSpawner = spawner; }



}
