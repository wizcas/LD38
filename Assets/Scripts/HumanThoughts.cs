﻿/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public bool isFinished;
    public HumanAI owner;
    public float speed = -1;

    protected virtual string[] Speeches
    {
        get { return null; }
    }

    private SightTracer _tracer;

    public HumanThought(HumanAI owner)
    {
        this.owner = owner;
    }

    public virtual void Init()
    {
        _tracer = new SightTracer(owner);
    }

    public virtual void Update()
    {
        isFinished = CheckFinished();
        if (isFinished)
        {
            Debug.Log(string.Format("<color=red>Thought {0} is Finished</color>", GetType()), owner);
        }

        if (_tracer.IsInSight(owner.eyes, owner.cat.transform) && !owner.cat.isHiding)
        {
            Messenger.Broadcast("CatInSight");
        }
    }

    protected abstract bool CheckFinished();

    public virtual bool IsSame(HumanThought thought)
    {
        if (GetType() != thought.GetType()) return false;
        return state == thought.state &&
            isFinished == thought.isFinished &&
            owner == thought.owner;
    }

    public override string ToString()
    {
        string color;
        switch (state)
        {
            case HumanState.Peace: color = "green"; break;
            case HumanState.Suspicious: color = "yellow"; break;
            case HumanState.Hostile: color = "red"; break;
            default: color = ""; break;
        }

        return string.Format("<color={0}>[{1}]{2}</color>", color, GetType(), "{0}");
    }

    public string RandomSpeech()
    {
        if (Speeches == null || Speeches.Length == 0) return null;

        var rnd = UnityEngine.Random.Range(0, Speeches.Length);
        return Speeches[rnd];
    }
}

[System.Serializable]
public class HumanPatrolThought : HumanThought
{
    public Route route;
    public int index = -1;
    public int direction = 1;
    private Waypoint _currentWaypoint;

    protected override string[] Speeches
    {
        get
        {
            return new[]
            {
                "Hmmmmmm...",
                "I'm so tired of working",
                "Hope some one would love this game",
                "48 hours of sleepless :/"
            };
        }
    }

    public HumanPatrolThought(HumanAI owner) : base(owner)
    {
    }

    public override void Init()
    {
        base.Init();
        _currentWaypoint = NextWaypoint();
    }

    public Waypoint NextWaypoint()
    {
        _currentWaypoint = route.Next(ref index, ref direction);
        //Debug.Log("Current route index: " + index, owner);
        return _currentWaypoint;
    }

    protected override bool CheckFinished()
    {
        return _currentWaypoint == null;
    }

    public override string ToString()
    {
        return string.Format(base.ToString(), route);
    }

    public override bool IsSame(HumanThought thought)
    {
        if (!base.IsSame(thought)) return false;
        var t = (HumanPatrolThought)thought;
        return route == t.route;
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
    private SightTracer _tracer;

    protected override string[] Speeches
    {
        get
        {
            return new[]
            {
                "Hi there, my little Doggie",
                "What's up, girl?",
                "Come to papa ❤",
                "Nothing stupid",
                "I know, I know. Clean the litter, right?"
            };
        }
    }

    public HumanNoticeThought(HumanAI owner) : base(owner)
    {
        _tracer = new SightTracer(owner);
    }

    protected override bool CheckFinished()
    {
        return !_tracer.IsInSight(owner.eyes, StareAt) || owner.cat.isHiding;
    }

    public override string ToString()
    {
        return string.Format(base.ToString(), string.Format("Stare @ {0}", stareAt));
    }

    public override bool IsSame(HumanThought thought)
    {
        if (!base.IsSame(thought)) return false;
        var t = (HumanNoticeThought)thought;
        return stareAt == t.stareAt;
    }
}

[System.Serializable]
public class HumanChaseThought : HumanThought, IThinkWatch, IThinkGoto
{
    public Transform moveTo;
    public Transform stareAt;
    private SightTracer _tracer;

    protected override string[] Speeches
    {
        get
        {
            return new[]
            {
                "Hey! Give it back!",
                "Please! I need that!",
                "Don't you hide this one!",
                "Good girl...And don't escape!"
            };
        }
    }

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
    public HumanChaseThought(HumanAI owner) : base(owner)
    {
        _tracer = new SightTracer(owner);
    }

    protected override bool CheckFinished()
    {
        return !_tracer.IsInSight(owner.eyes, StareAt) || owner.cat.isHiding;
    }

    public override string ToString()
    {
        return string.Format(base.ToString(), string.Format("Stare @ {0}, Move {1}", stareAt, MoveTo.position));
    }

    public override bool IsSame(HumanThought thought)
    {
        if (!base.IsSame(thought)) return false;
        var t = (HumanChaseThought)thought;
        return stareAt == t.stareAt &&
            moveTo == t.moveTo;
    }
}

[System.Serializable]
public class HumanInvestigateThought : HumanThought, IThinkGoto
{
    public Transform moveTo;
    public bool isTempTarget;

    public float investigateDuration = 3f;

    private float investigateEndTime;

    public Transform MoveTo
    {
        get
        {
            return moveTo;
        }
    }

    protected override string[] Speeches
    {
        get
        {
            return new[]
            {
                "Something wrong with this...",
                "Doggie! I know you did this!",
                "MLGB..(╯‵□′)╯︵┻━┻",
                "I'm enough of this"
            };
        }
    }

    public HumanInvestigateThought(HumanAI owner) : base(owner) {
        investigateEndTime = float.NaN;
    }

    protected override bool CheckFinished()
    {
        var mover = owner.GetComponent<HumanMover>();

        if (mover.IsStandingStill)
        {
            if (float.IsNaN(investigateEndTime))
            {
                investigateEndTime = Time.time + investigateDuration;
            }
        }
        else
        {
            return false;
        }

        // Make sure the human stops for certain time
        if (Time.time < investigateEndTime)
        {
            return false;
        }
                
        if (isTempTarget)
        {
            UnityEngine.Object.Destroy(moveTo.gameObject);
        }
        return true;
    }

    public override string ToString()
    {
        return string.Format(base.ToString(), string.Format("Move {0}", MoveTo.position));
    }

    public override bool IsSame(HumanThought thought)
    {
        if (!base.IsSame(thought)) return false;
        var t = (HumanInvestigateThought)thought;
        return moveTo == t.moveTo;
    }
}

public class SightTracer
{
    private const float scanDeltaAngle = 15f;
    private HumanAI _owner;
    public SightTracer(HumanAI owner)
    {
        _owner = owner;
    }

    private float minSightAngle
    {
        get { return -_owner.sightRangeAngle * .5f; }
    }

    private float maxSightAngle
    {
        get { return _owner.sightRangeAngle * .5f; }
    }

    public bool IsInSight(Transform eyes, Transform target)
    {
        var sightAngle = eyes.transform.localEulerAngles.y;
        var catRelDir = target.position - eyes.position;
        catRelDir.y = 0;
        var catAngle = VectorUtils.NormalizeAngle(Vector3.Angle(catRelDir, eyes.forward));
        //Debug.LogFormat("SightRange: {0}, Cat Angle: {1}", sightRange, catAngle);
        bool isCatInSightAngle = catAngle >= minSightAngle && catAngle <= maxSightAngle;
        bool isCatNearEnough = catRelDir.magnitude <= _owner.detectDistance;
        //Debug.LogFormat("Cat In Range? {0}, Cat In Distance? {1}({2})", isCatInRange, isCatNearEnough, catRelDir.magnitude);
        if (isCatInSightAngle && isCatNearEnough) // raycast only if the cat is in sight range, in the consideration of performance.
        {
            return ScanSight(eyes, target);
        }
        return false;
    }

    private bool ScanSight(Transform eyes, Transform target)
    {

        var scanCount = Mathf.RoundToInt(_owner.sightRangeAngle / scanDeltaAngle) + 1;
        var forward = eyes.forward;
        forward.y = 0;

        for (int x = 0; x < scanCount; x++)
        {
            var horizontalAngle = minSightAngle + scanDeltaAngle * x;
            //Debug.Log("x: " + xAngle);
            for (int y = 0; y < scanCount; y++)
            {
                var verticalAngle = minSightAngle + scanDeltaAngle * y;
                //Debug.Log("y: " + yAngle);
                //var rayDir = Quaternion.Euler(xAngle, yAngle, 0f) * forward;
                var rayDir = Quaternion.AngleAxis(verticalAngle, Vector3.right) * Quaternion.AngleAxis(horizontalAngle, Vector3.up) * forward;

                var sightRay = new Ray(eyes.position, rayDir);
                Debug.DrawRay(eyes.position, rayDir, Color.black);
                RaycastHit hit;

                if (Physics.Raycast(sightRay, out hit, _owner.detectDistance, Layers.GetLayerMasks(Layers.Cat, Layers.Environment, Layers.Wall)))
                {
                    if (hit.collider.transform == target)
                    {
                        Debug.DrawLine(eyes.position, hit.point, Color.red);
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
