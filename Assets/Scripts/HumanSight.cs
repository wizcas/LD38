/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanAI))]
public class HumanSight : MonoBehaviour
{
    public float sightRangeAngle = 90f;
    public float sightSweepAngle = 30f;
    public float sightSweepSpeed = 10f;
    public float stareTurnSpeed = 20f;
    public float detectDistance = 10f;
    public float hearDistance = 10f;


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

    void Awake()
    {
        Messenger.AddListener<HumanThought>(HumanAI.HaveThoughtEvent, HaveThought);
    }

    void Start()
    {
        _prevSweepDirection = _sweepDirection;
    }

    private void HaveThought(HumanThought thought)
    {
        Debug.Log(string.Format("<color=blue>sight has thought: {0}</color>", thought), this);
        SetSightColor(thought);
        if (thought is IThinkWatch)
        {
            var watch = (IThinkWatch)thought;
            _stareAt = watch.StareAt;
        }
        else
        {
            _stareAt = null;
            if (_sweepDirection == 0)
                _sweepDirection = _prevSweepDirection;
        }
    }

    void Update()
    {
        UpdateSight();
        UpdateStare();        
    }

    private void UpdateSight()
    {
        // update the light parameters
        _sightLight.spotAngle = sightRangeAngle + 10;
        _sightLight.range = detectDistance;

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

    private void UpdateStare()
    {
        if (_stareAt == null) return;
        if (_sweepDirection != 0) // store the prev status only if this one was sweeping sight
        {
            _prevSweepDirection = _sweepDirection;
        }
        _sweepDirection = 0;
        // turn to target
        var targetPos = _stareAt.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
        // fix eye sight
        _eyes.transform.localEulerAngles = Vector3.zero;
    }

    private void SetSightColor(HumanThought thought)
    {
        switch (thought.state)
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
    }

    /*
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
        */
}
