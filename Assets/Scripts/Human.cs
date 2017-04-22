/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public float sightRangeAngle = 90f;
    public float sightSweepAngle = 30f;
    public float sightSweepSpeed = 2f;
    public float stareTurnSpeed = 20f;
    public CatAction cat;
    public float detectDistance = 2f;

    public Transform stareIntention;

    public HumanState _currentState;

    [SerializeField]
    private Transform _eyes;
    [SerializeField]
    private Light _sightLight;
    [SerializeField]
    private Color _peaceColor = Color.green;
    [SerializeField]
    private Color _suspiciousColor = Color.yellow;
    [SerializeField]
    private Color _hostileColor = Color.red;

    private int _sweepDirection = 1;
    private int _prevSweepDirection;
    private Transform _stareAt;

    void Start()
    {
        _prevSweepDirection = _sweepDirection;
    }

    void Update()
    {
        UpdateStare(stareIntention);
        UpdateSight();
        if (DetectCat())
        {
            stareIntention = cat.transform;
            if (cat.IsHoldingTreasure)
                _currentState = HumanState.Hostile;
            else
                _currentState = HumanState.Suspicious;
        }
        else
        {
            stareIntention = null;
            _currentState = HumanState.Peace;
        }
    }

    private void UpdateStare(Transform target)
    {
        if (target == _stareAt && target == null) return; // if isn't staring at anything all the time, then skip.
        _stareAt = target;
        if (target != null) // if staring at something
        {
            if (_sweepDirection != 0)
                _prevSweepDirection = _sweepDirection;
            _sweepDirection = 0;
            // turn to target
            var targetPos = target.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
            // fix eye sight
            _eyes.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            _sweepDirection = _prevSweepDirection;
        }
    }

    private void UpdateSight()
    {
        // update the light
        _sightLight.spotAngle = sightRangeAngle;
        // update the color
        switch (_currentState)
        {
            case HumanState.Peace:
                _sightLight.color = _peaceColor;
                break;
            case HumanState.Suspicious:
                _sightLight.color = _suspiciousColor;
                break;
            case HumanState.Hostile:
                _sightLight.color = _hostileColor;
                break;
        }

        // sweep
        var angle = _eyes.transform.localEulerAngles;
        angle.y += sightSweepSpeed * Time.deltaTime * _sweepDirection;        
        angle.y = VectorUtils.NormalizeAngle(angle.y);
        _eyes.transform.localEulerAngles = angle;
        if (Mathf.Abs(angle.y) >= sightSweepAngle)
        {
            _sweepDirection *= -1;
        }
    }

    private bool DetectCat()
    {
        var sightAngle = _eyes.transform.localEulerAngles.y;
        var sightRange = new Vector2(
            VectorUtils.NormalizeAngle(sightAngle - sightRangeAngle * .5f),
            VectorUtils.NormalizeAngle(sightAngle + sightRangeAngle * .5f)
            );
        var catRelDir = cat.transform.position - transform.position;
        catRelDir.y = 0;
        var catAngle = VectorUtils.NormalizeAngle(Vector3.Angle(catRelDir, transform.forward));
        //Debug.LogFormat("SightRange: {0}, Cat Angle: {1}", sightRange, catAngle);
        bool isCatInRange = catAngle >= sightRange.x && catAngle <= sightRange.y;
        bool isCatNearEnough = catRelDir.magnitude <= detectDistance;
        //Debug.LogFormat("Cat In Range? {0}, Cat In Distance? {1}({2})", isCatInRange, isCatNearEnough, catRelDir.magnitude);
        if(isCatInRange && isCatNearEnough) // raycast only if the cat is in sight range, in the consideration of performance.
        {
            // not accurate
            var catDir = cat.transform.position - transform.position;
            var sightRay = new Ray(transform.position, catDir);
            Debug.DrawRay(transform.position, catDir, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(sightRay, out hit, detectDistance))
            {
                return hit.collider.transform == cat.transform;
            }
        }
        return false;
    }
}

public enum HumanState
{
    Peace,
    Suspicious,
    Hostile
}