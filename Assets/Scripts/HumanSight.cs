/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        _prevSweepDirection = _sweepDirection;
        Messenger.AddListener<HumanThought>(HumanAI.HaveThoughtEvent, SetSightByThought);
    }

    void Update()
    {
        UpdateSight();
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

    public void SetSightByThought(HumanThought thought)
    {
        if (thought is IThinkWatch)
        {
            var watch = (IThinkWatch)thought;
            if (watch.StareAt != null)
            {
                DoStare(watch);
            }
        }
        else if (_sweepDirection == 0)
        {
            _sweepDirection = _prevSweepDirection;
        }
    }

    private void DoStare(IThinkWatch thought)
    {
        if (_sweepDirection != 0) // store the prev status only if this one was sweeping sight
        {
            _prevSweepDirection = _sweepDirection;
        }
        _sweepDirection = 0;
        // turn to target
        var targetPos = thought.StareAt.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
        // fix eye sight
        _eyes.transform.localEulerAngles = Vector3.zero;
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
