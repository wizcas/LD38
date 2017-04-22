/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanAI : MonoBehaviour
{
    public const string HaveThoughtEvent = "HaveThought";

    public float sightRangeAngle = 90f;
    public float sightSweepAngle = 30f;
    public float sightSweepSpeed = 10f;
    public float stareTurnSpeed = 20f;
    public float detectDistance = 10f;
    public float hearDistance = 10f;

    public CatAction cat;
    public Transform stareIntention;
    public Transform investigateIntention;

    [SerializeField]
    private HumanState _currentState;
    public HumanState CurrentState
    {
        get { return _currentState; }
        set
        {
            UpdateState(value, _currentState);
        }
    }

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
    private Transform _investigating;

    void Start()
    {
        _prevSweepDirection = _sweepDirection;
        Messenger.AddListener<Transform>("SoundMade", OnSoundMade);
    }

    private void OnSoundMade(Transform source)
    {
        if (VectorUtils.SqrDistance(source.position, transform.position) > hearDistance * hearDistance) return;

        investigateIntention = source;
    }

    void Update()
    {
        UpdateStare(stareIntention);
        UpdateSight();
        UpdateAction();
    }

    //private void UpdateStare(Transform target)
    //{
    //    if (target == _stareAt && target == null) return; // if isn't staring at anything all the time, then skip.
    //    _stareAt = target;
    //    if (target != null) // if staring at something
    //    {
    //        if (_sweepDirection != 0)
    //            _prevSweepDirection = _sweepDirection;
    //        _sweepDirection = 0;
    //        // turn to target
    //        var targetPos = target.position;
    //        targetPos.y = transform.position.y;
    //        transform.LookAt(targetPos);
    //        // fix eye sight
    //        _eyes.transform.localEulerAngles = Vector3.zero;
    //    }
    //    else
    //    {
    //        _sweepDirection = _prevSweepDirection;
    //    }
    //}

    //private void UpdateSight()
    //{
    //    // update the light parameters
    //    _sightLight.spotAngle = sightRangeAngle + 10;
    //    _sightLight.range = detectDistance;

    //    // sweep
    //    var angle = _eyes.transform.localEulerAngles;
    //    angle.y += sightSweepSpeed * Time.deltaTime * _sweepDirection;        
    //    angle.y = VectorUtils.NormalizeAngle(angle.y);
    //    _eyes.transform.localEulerAngles = angle;
    //    if (Mathf.Abs(angle.y) >= sightSweepAngle)
    //    {
    //        _sweepDirection *= -1;
    //    }
    //}

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

    private void UpdateAction()
    {
        if (DetectCat())
        {
            stareIntention = cat.transform;
            if (cat.IsHoldingTreasure)
                CurrentState = HumanState.Hostile;
            else
                CurrentState = HumanState.Suspicious;
        }
        else
        {
            stareIntention = null;
            CurrentState = HumanState.Peace;
        }
    }

    private void UpdateState(HumanState state, HumanState oldState)
    {
        _currentState = state;
        // update the color
        switch (_currentState)
        {
            case HumanState.Peace:
                _sightLight.color = _peaceColor;
                    Messenger.Broadcast("HumanResume");
                break;
            case HumanState.Suspicious:
                _sightLight.color = _suspiciousColor;
                Messenger.Broadcast("HumanStop");
                break;
            case HumanState.Hostile:
                _sightLight.color = _hostileColor;
                Messenger.Broadcast("HumanChase", cat.transform);
                break;
        }
    }
}

public enum HumanState
{
    Peace,
    Suspicious,
    Hostile
}

public interface IThinkGoto
{
    Transform MoveTo { get; }
}

public interface IThinkWatch
{
    Transform StareAt { get; }
}

[System.Serializable]
public abstract class HumanThought
{
    public HumanState state;
}

[System.Serializable]
public class HumanPatrolThought : HumanThought
{
    public Route route;
    public int index = -1;
    public int direction = 1;

    public Waypoint NextWaypoint()
    {
        return route.Next(index, direction);
    }
}

[System.Serializable]
public class HumanNoticeThought : HumanThought, IThinkWatch
{
    public Transform stareAt;

    public Transform StareAt
    {
        get
        {
            return stareAt;
        }
    }
}

[System.Serializable]
public class HumanChaseThought : HumanThought, IThinkWatch, IThinkGoto
{
    public Transform moveTo;
    public Transform stareAt;

    public Transform MoveTo
    {
        get
        {
            return moveTo;
        }
    }

    public Transform StareAt
    {
        get
        {
            return stareAt;
        }
    }
}

[System.Serializable]
public class HumanInvestigateThought: HumanThought, IThinkGoto
{
    public Transform moveTo;

    public Transform MoveTo
    {
        get
        {
            return moveTo;
        }
    }

}
