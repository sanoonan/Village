//Rotates a door open when a character approaches, then closes it again once all characters have left the collider area.
using UnityEngine;
using System.Collections;

public class AutoDoorActionScript : MonoBehaviour 
{
    Transform _transform;
    TweenRotation _tween;
    byte containerCount;

    static Vector3 openInwards = new Vector3(0.0f, -100.0f, 0.0f);
    static Vector3 openOutwards = new Vector3(0.0f, 100.0f, 0.0f);

    public bool flipped;

	void Awake () 
    {
        _transform = transform;
        _tween = GetComponent<TweenRotation>();
        containerCount = 0;

        _transform.localRotation = Quaternion.identity;
	}

    void OnTriggerEnter(Collider other)
    {
        if (containerCount == 0)
        { 
            Vector3 toTarget = (other.transform.position - _transform.position).normalized;

            if (Vector3.Dot(toTarget, transform.forward) > 0)
                _tween.to = openInwards * (flipped ? -1 : 1);
            else
                _tween.to = openOutwards * (flipped ? -1 : 1);

            _tween.PlayForward();
        }

        ++containerCount;
    }

    void OnTriggerExit(Collider other)
    {
        --containerCount;

        if (containerCount == 0)
            _tween.PlayReverse();
    }
}
