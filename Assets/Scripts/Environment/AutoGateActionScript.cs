//Opens an entrance gate when a character approaches, then closes it again once all characters have left the collider area.
using UnityEngine;
using System.Collections;

public class AutoGateActionScript : MonoBehaviour 
{
    Transform _transform;
    TweenPosition _tween;
    byte containerCount;

    void Awake()
    {
        _transform = transform;
        _tween = GetComponent<TweenPosition>();
        containerCount = 0;

        _transform.localPosition = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (containerCount == 0)
            _tween.PlayForward();

        ++containerCount;
    }

    void OnTriggerExit(Collider other)
    {
        --containerCount;

        if (containerCount == 0)
            _tween.PlayReverse();
    }
}
