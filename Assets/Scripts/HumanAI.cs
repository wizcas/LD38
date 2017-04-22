/************************************
** Created by Wizcas (wizcas.me)
************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class HumanAI : MonoBehaviour
{
    public const string HaveThoughtEvent = "HaveThought";

    public float sightRangeAngle = 90f;
    public float sightSweepAngle = 30f;
    public float sightSweepSpeed = 10f;
    public float stareTurnSpeed = 20f;
    public float detectDistance = 10f;
    public float hearDistance = 10f;

    public HumanPatrolThought defaultPatrolThought;
    public Transform eyes;

    public CatAction cat;

    [SerializeField]
    private string[] _editorThoughts;

    private Stack<HumanThought> _thoughts = new Stack<HumanThought>();

    public HumanThought CurrentThought
    {
        get
        {
            return _thoughts.Peek();
        }
    }

    void Awake()
    {
        Messenger.AddListener("CatInSight", LookForCat);
        Messenger.AddListener<Transform>("SoundMade", OnSoundMade);
    }

    void Start()
    {
        PushThought(defaultPatrolThought);
    }

    private void OnSoundMade(Transform source)
    {
        if (VectorUtils.SqrDistance(source.position, transform.position) > hearDistance * hearDistance) return;
        PushThought(new HumanInvestigateThought(this)
        {
            moveTo = source
        });
    }

    void Update()
    {
        if (_thoughts.Count > 0)
        {
            CurrentThought.Update();
            if (CurrentThought.isFinished)
                PopThought();
        }
    }

    public void LookForCat()
    {
        if (cat.IsHoldingTreasure)
            PushThought(new HumanChaseThought(this)
            {
                state = HumanState.Hostile,
                moveTo = cat.transform,
                stareAt = cat.transform
            });
        else
            PushThought(new HumanNoticeThought(this)
            {
                state = HumanState.Suspicious,
                stareAt = cat.transform
            });
    }
#region obsoleted
    //private bool DetectCat(Transform eyes)
    //{
    //    var sightAngle = eyes.transform.localEulerAngles.y;
    //    var sightRange = new Vector2(
    //        VectorUtils.NormalizeAngle(sightAngle - sightRangeAngle * .5f),
    //        VectorUtils.NormalizeAngle(sightAngle + sightRangeAngle * .5f)
    //        );
    //    var catRelDir = cat.transform.position - transform.position;
    //    catRelDir.y = 0;
    //    var catAngle = VectorUtils.NormalizeAngle(Vector3.Angle(catRelDir, transform.forward));
    //    //Debug.LogFormat("SightRange: {0}, Cat Angle: {1}", sightRange, catAngle);
    //    bool isCatInRange = catAngle >= sightRange.x && catAngle <= sightRange.y;
    //    bool isCatNearEnough = catRelDir.magnitude <= detectDistance;
    //    //Debug.LogFormat("Cat In Range? {0}, Cat In Distance? {1}({2})", isCatInRange, isCatNearEnough, catRelDir.magnitude);
    //    if (isCatInRange && isCatNearEnough) // raycast only if the cat is in sight range, in the consideration of performance.
    //    {
    //        // TODO: not accurate
    //        var catDir = cat.transform.position - transform.position;
    //        var sightRay = new Ray(transform.position, catDir);
    //        Debug.DrawRay(transform.position, catDir, Color.red);
    //        RaycastHit hit;
    //        if (Physics.Raycast(sightRay, out hit, detectDistance))
    //        {
    //            return hit.collider.transform == cat.transform;
    //        }
    //    }
    //    return false;
    //}
#endregion

    private void PushThought(HumanThought thought)
    {
        if(_thoughts.Count > 0 && CurrentThought.IsSame(thought))
        {
            return;
        }
        _thoughts.Push(thought);
        UpdateThoughtList();
        thought.Init();
        Messenger.Broadcast(HaveThoughtEvent, thought);
    }

    private void PopThought()
    {
        _thoughts.Pop();
        UpdateThoughtList();
        if (_thoughts.Count > 0)
        {
            Messenger.Broadcast(HaveThoughtEvent, _thoughts.Peek());
        }
    }

    private void UpdateThoughtList()
    {
        _editorThoughts = _thoughts.Select(t => t.ToString()).ToArray();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.color = Color.magenta;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, hearDistance);
#endif
    }
}

public enum HumanState
{
    Peace,
    Suspicious,
    Hostile
}
