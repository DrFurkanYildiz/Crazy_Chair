using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHandler : MonoBehaviour
{
    public HumanState State;
    public List<GameObject> humanPrefabList = new List<GameObject>();
    [HideInInspector] public Animator animator;
    private Rigidbody rb;
    public Transform camTransform;
    private bool _isFirstGuest;

    [HideInInspector] public Vector3 targetPosition;
    private Vector3 _throwPosition;
    [SerializeField] private Vector3 _fallPosition;
    private Action onArrivedAtPosition;
    private Action onAnimComplete;
    private event EventHandler onRandomNewIdle;

    float _flyingSpeed = 120f;
    float _fallSpeed = 180f;

    [SerializeField] private float _flyingCost;
    public float FlyingCost
    {
        get { return _flyingCost; }
        set { _flyingCost = value; }
    }

    private float _flightDistance;
    public float FlightDistance { get { return _flightDistance;} }
    private ParticleSystem fallSmokeParticle;
    private bool isGround;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        fallSmokeParticle = GetComponentInChildren<ParticleSystem>();

        onRandomNewIdle += GetRandomNewIdle;
    }
    private void FixedUpdate()
    {
        HumanHandsUp();
        StateMachineHandler();
    }
    private void StateMachineHandler()
    {
        switch (State)
        {
            case HumanState.Walking:
                Move();
                break;
            case HumanState.Idle:
                Idle();
                break;
            case HumanState.Stand:
                SitDown();
                break;
            case HumanState.Ready:
                Ready();
                break;
            case HumanState.FallingOut:
                FallingOutChair();
                break;
            case HumanState.Thrown:
                Thrown();
                break;
            case HumanState.Fall:
                Fall();
                break;
            case HumanState.Crash:
                Crash();
                break;
            case HumanState.Empty:
                //Debug.Log("Bekle");
                break;
        }
    }
    private void OnDisable()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        State = HumanState.Idle;
        targetPosition = Vector3.zero;
        _throwPosition = Vector3.zero;
        _fallPosition = Vector3.zero;
        FlyingCost = 0f;
        _flightDistance = 0f;
    }
    private void OnEnable()
    {
        humanPrefabList.ForEach(h => h.SetActive(false));
        RandomType();
    }


    private void RandomType()
    {
        humanPrefabList[UnityEngine.Random.Range(0, humanPrefabList.Count)].SetActive(true);
    }
    public void SetFirstGuest()
    {
        _isFirstGuest = true;
    }
    public bool GetFirstGuest()
    {
        return _isFirstGuest;
    }


    private void Move()
    {
        if (Vector3.Distance(transform.position, targetPosition) > .3f)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            transform.position = transform.position + moveDir * 25f * Time.deltaTime;

            animator.SetBool("Idle", false);
            animator.SetBool("Walking", true);
        }
        else
        {
            if (onArrivedAtPosition != null)
            {
                Action tmpAction = onArrivedAtPosition;
                onArrivedAtPosition = null;
                tmpAction();
                
                if(Vector3.Distance(transform.position,targetPosition) <= .3f){
                    State = HumanState.Idle;
                }
            }
        }
    }
    private void Idle()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Walking", false);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            onRandomNewIdle?.Invoke(this, EventArgs.Empty);

        if (ChairManager.Instance.GetChair() != null)
            if (targetPosition == ChairManager.Instance.GetChair().GetPosition())
                State = HumanState.Stand;
    }
    private void GetRandomNewIdle(object sender, EventArgs e)
    {
        animator.SetInteger("IdleWaiting", UnityEngine.Random.Range(0, 7));
    }
    private void SitDown()
    {
        if (onAnimComplete != null)
        {
            Action tmpAction = onAnimComplete;
            onAnimComplete = null;
            State = HumanState.Ready;
            tmpAction();
            SaveManager.Instance.OnSave();
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0, 0.1f, 0.1f), .5f * Time.deltaTime);
        }
    }
    private void Ready()
    {
        animator.SetTrigger("Firlatilabilir");
        ChairManager.Instance.IsReadyToThrow = true;
        if (ChairManager.Instance.GetChair().IsBrokenChair())
        {
            State = HumanState.FallingOut;
        }
    }
    private void Thrown()
    {
        if (Vector3.Distance(transform.position, _throwPosition) > 2f)
        {
            Vector3 moveDir = (_throwPosition - transform.position).normalized;

            transform.position = transform.position + moveDir * _flyingSpeed * Time.fixedDeltaTime;

            CameraManager.Instance.SetFollowCameraNear(0.3f);

            _flightDistance = Vector3.Magnitude(transform.position);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            rb.freezeRotation = true;
            State = HumanState.Fall;
        }
    }
    private void Fall()
    {
        if (isGround)
        {
            animator.SetTrigger("Cakilma");
            SoundManager.Instance.EffectOneShot(SoundManager.Instance.guestFallEffect[UnityEngine.Random.Range(0, 2)]);
            State = HumanState.Crash;
        }
        else 
        {
            if (Vector3.Distance(transform.position, _fallPosition) > 2f)
            {
                Vector3 moveDir = (_fallPosition - transform.position).normalized;

                transform.position = transform.position + moveDir * _fallSpeed * Time.fixedDeltaTime;

                CameraManager.Instance.SetFollowCameraNear(30f);
            }
            else
            {
                animator.SetTrigger("Cakilma");
                SoundManager.Instance.EffectOneShot(SoundManager.Instance.guestFallEffect[UnityEngine.Random.Range(0, 2)]);
                State = HumanState.Crash;
            }
        }
    }
    private void Crash()
    {
        FunctionTimer.Create(() =>
        {
            ChairManager.Instance.IsGuestFlying = false;
            HumanSpawner.Instance.GuestDespawn(this, _isFirstGuest);
        }, 2f);

        ChairManager.Instance.CalculateShotGain(_flightDistance);

        if (FlyingCost >= 300f && FlyingCost <= 5000f)
        {
            CoinCollectManager.Instance.AddCoins(Mathf.RoundToInt(FlyingCost / 100f));
            SoundManager.Instance.EffectOneShot(SoundManager.Instance.coinCollectEffect);
        }
        else if(FlyingCost > 5000f)
        {
            CoinCollectManager.Instance.AddCoins(CoinCollectManager.Instance.GetMaxCoin);
            SoundManager.Instance.EffectOneShot(SoundManager.Instance.coinCollectEffect);
        }

        HapticsManager.Instance.CreateConstantHaptic(.5f, .5f, .1f);
        fallSmokeParticle.Play();
        State = HumanState.Empty;
    }

    private void FallingOutChair()
    {
        ChairManager.Instance.IsReadyToThrow = false;
        ChairManager.Instance.GetChair().transform.GetComponent<ChairMovement>().RotationReset();
        transform.localPosition = Vector3.Lerp(transform.position, new Vector3(0f, -0.2f, 0.5f), 10f);

        animator.SetTrigger("SDusme");
        ChairManager.Instance.StopSparkParticle();

        SoundManager.Instance.EffectOneShot(SoundManager.Instance.brokenChairEffect);
        State = HumanState.Empty;
    }
    public void MoveTo(Vector3 position, Action onArrivedAtPosition = null)
    {
        SetTargetPosition(position);

        this.onArrivedAtPosition = onArrivedAtPosition;
        State = HumanState.Walking;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    public void PlayAnimationSitDown(Action onAnimComplete)
    {
        ChairManager.Instance.GetChair().transform.GetComponent<ChairMovement>().RotationReset();
        animator.SetTrigger("Stand");
        State = HumanState.Stand;
        StartCoroutine(PlayAnim(onAnimComplete));
    }
    IEnumerator PlayAnim(Action onAnimComplete)
    {
        transform.SetParent(ChairManager.Instance.GetChair().ChairTopTransform, true);
        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3(0, 0.1f, 0.5f);

        yield return new WaitForSeconds(1f);
        this.onAnimComplete = onAnimComplete;
    }
    public void GuestMoveToThrow(Vector3 position)
    {
        SetThrowPosition(position);
        animator.SetBool("Firla", true);
        State = HumanState.Thrown;
    }
    public void SetThrowPosition(Vector3 throwPosition)
    {
        _throwPosition = throwPosition;
    }
    public void SetFallPosition(Vector3 fallPosition)
    {
        _fallPosition = fallPosition;
    }
    

    private void HumanHandsUp()
    {
        float turnAmount = ChairManager.Instance.TurnAmount;
        if (turnAmount < 500f)
        {
            animator.SetBool("Whistling", false);
            animator.SetBool("HandsUp", false);
        }
        else if (turnAmount >= 500f && turnAmount < 800f)
            animator.SetBool("Whistling", true);
        else if (turnAmount >= 800f)
            animator.SetBool("HandsUp", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer == 9)
            isGround = true;
        else
            isGround = false;
    }
}

[Serializable]
public enum HumanState
{
    Walking,
    Idle,
    Stand,
    Ready,
    FallingOut,
    Thrown,
    Fall,
    Crash,
    Empty
}