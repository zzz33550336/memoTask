using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    private Vector3 _offset = new Vector3(0, 2, -10);
    private float _followSpeed = 5f; 
    private Vector2 _minBounds = new Vector2(-1, -1);
    private Vector2 _maxBounds = new Vector2(1, 1);
    private float pixelsPerUnit = 64f;
    void LateUpdate()
    {
        if (_target == null) return;


        Vector3 targetPos = new Vector3(
            _target.position.x + _offset.x,
            _target.position.y + _offset.y,
            _offset.z
        );
        // limit
        float clampedX = Mathf.Clamp(targetPos.x, _minBounds.x, _maxBounds.x);
        float clampedY = Mathf.Clamp(targetPos.y, _minBounds.y, _maxBounds.y);

        Vector3 clampedPos = new Vector3(clampedX, clampedY, targetPos.z);


        transform.position = Vector3.Lerp(transform.position, clampedPos, _followSpeed * Time.deltaTime);
       // SnapToPixel();
    }
     private void SnapToPixel()
    {
        Vector3 position = transform.position;
        position.x = RoundToNearestPixel(position.x);
        position.y = RoundToNearestPixel(position.y);
        transform.position = position;
    }

    private float RoundToNearestPixel(float value)
    {
        return Mathf.Round(value * pixelsPerUnit) / pixelsPerUnit;
    }
}
