using UnityEngine;

public class Jumper : MonoBehaviour
{

    [SerializeField]
    private PlayerCamera playerCamera;

    [SerializeField]
    private float _v0;

    [SerializeField, Min(0.001f)]
    private float _jumpPow;

    [SerializeField]
    private float _gravity;

    [SerializeField, Min(1)]
    private float _resistance;

    [SerializeField]
    private float _mass;

    [SerializeField, Min(1)]
    private int _defaultJumpCount;

    private float vertical;

    private float _verticalVelocity = 0;

    private float _finalVelocity;
    private bool _isLastGround = true;
    private float _jumpTimer = 0;

    private int _jumpCount;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool jumppunch = false;
    public bool Jumppunch
    {
        get => jumppunch; set => jumppunch = value;
    }

    [SerializeField]
    private float punch;

    [SerializeField]
    private float punchone;
    [SerializeField]
    private float punchtwo;
    [SerializeField]
    private float punchthree;

    /// <summary>
    /// PlayerSound�擾
    /// </summary>
    private PlayerSoundC _cPlayerSound;

    private PlayerPunch _punch;

    // Start is called before the first frame update
    private void Start()
    {
        UpdateFinalVelocity();

        _jumpCount = _defaultJumpCount;

        _cPlayerSound = GetComponent<PlayerSoundC>();

        _punch = GetComponent<PlayerPunch>();
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (this.transform.position.y < -20)
        {
            UnityEditor.EditorApplication.isPlaying = false;

        }

#endif
        if (Input.GetKeyDown("joystick button 2") && _jumpCount == 1 ||
            Input.GetKeyDown("joystick button 3") && _jumpCount == 1)
        {
            JumpSE();
        }

        if (Input.GetKeyUp("joystick button 2") && !PlayerManager.IsPlayerMoveRock ||
            Input.GetKeyUp("joystick button 3") && !PlayerManager.IsPlayerMoveRock)
        {
            _jumpTimer = 0;

            _punch.PunchReset();

            if (!CheckGround())
                --_jumpCount;
        }

        if (Input.GetKeyUp("joystick button 4") && !PlayerManager.IsPlayerMoveRock)
        {
            int hoge = GetComponent<PlayerPunch>().punch;

            if (hoge == 1)
            {
                punch = punchone;
            }
            else if (hoge == 2)
            {
                punch = punchtwo;
            }
            else if (hoge == 3)
            {
                punch = punchthree;
            }

            vertical = -Input.GetAxis("Vertical");

            _jumpTimer = 0;

            //Jumppunch = true;

            if (!CheckGround())
                --_jumpCount;

        }

        if (_isLastGround != CheckGround())
        {
            jumppunch = false;

            if (_isLastGround)
            {
                if (_jumpTimer < Mathf.Epsilon)
                    --_jumpCount;
            }
            else
            {
                _jumpCount = _defaultJumpCount;
            }



        }

        _isLastGround = CheckGround();

        if (CheckBury())
            Correction_Up();

        if (OnCeiling())
            Correction_Down();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (CheckGround())
        {
            _verticalVelocity = 0;

        }
        else
        {
            ApplyingGravity();
        }

        if (Input.GetKey("joystick button 2") && JumpAble() && !PlayerManager.IsPlayerMoveRock ||
            Input.GetKey("joystick button 3") && JumpAble() && !PlayerManager.IsPlayerMoveRock)
        {
            Jump();
        }

        if (OnCeiling() && _verticalVelocity > 0)
        {
            _verticalVelocity = 0;

            _jumpTimer = _v0;

            --_jumpCount;
        }

        if (Jumppunch == true)
        {
            JumpPunch();
        }

        Move();


        GetComponent<PlayerAnimC>()._isJumping = !JumpAble();
    }

    private void UpdateFinalVelocity()
    {
        _finalVelocity = _mass * _gravity / _resistance;
    }

    private void Jump()
    {
        GetComponent<PlayerPunch>().PunchReset();

        _jumpTimer += Time.deltaTime / _jumpPow;

        if (_v0 < _jumpTimer || _verticalVelocity > _finalVelocity)
        {
            return;
        }
        _cPlayerSound.SoundStart(6);
        _verticalVelocity = _v0 - _jumpTimer;
    }

    private void JumpSE()
    {
        _cPlayerSound.SoundStart(6);
    }
    public void JumpPunch()
    {
        float _v1 = _v0 + Mathf.Abs(vertical) * punch;

        _jumpTimer += Time.deltaTime / _jumpPow;

        if (_v1 < _jumpTimer || _verticalVelocity > _finalVelocity) return;

        _verticalVelocity = _v1 - _jumpTimer;

    }

    private void Move()
    {
        this.transform.position += this.transform.up * _verticalVelocity;
    }

    private void Correction_Down()
    {
        var rayTupleData = CreateRay_Up();

        RaycastHit2D hit = Physics2D.Raycast(
                                rayTupleData.Item1.origin,
                                rayTupleData.Item1.direction,
                                this.transform.localScale.y * 0.5f,
                                groundMask);

        if (hit.collider != null)
        {
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            var currentPosition = this.transform.position;

            currentPosition.y -= moveVal;

            this.transform.position = currentPosition;
        }
        else
        {
            hit = Physics2D.Raycast(
                       rayTupleData.Item2.origin,
                       rayTupleData.Item2.direction,
                       this.transform.localScale.y * 0.6f,
                       groundMask);

            if (hit.collider == null) return;

            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            var currentPosition = this.transform.position;

            currentPosition.y -= moveVal;

            this.transform.position = currentPosition;
        }
    }

    private void Correction_Up()
    {
        var rayTupleData = CreateRay_Down();

        RaycastHit2D hit = Physics2D.Raycast(
                                rayTupleData.Item1.origin,
                                rayTupleData.Item1.direction,
                                this.transform.localScale.y * 0.5f,
                                groundMask);
        if (hit.collider != null)
        {
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            var currentPosition = this.transform.position;

            currentPosition.y += moveVal;

            this.transform.position = currentPosition;
        }
        else
        {
            hit = Physics2D.Raycast(
                       rayTupleData.Item2.origin,
                       rayTupleData.Item2.direction,
                       this.transform.localScale.y * 0.5f,
                       groundMask);

            if (hit.collider == null) return;

            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            var currentPosition = this.transform.position;

            currentPosition.y += moveVal;

            this.transform.position = currentPosition;
        }
    }

    private void ApplyingGravity()
    {
        if (_verticalVelocity < _finalVelocity * -1) return;

        _verticalVelocity -= _gravity;

    }

    private bool CheckGround()
    {
        var rayTupleData = CreateRay_Down();

        return
                Physics2D.Raycast(
                rayTupleData.Item1.origin,
                rayTupleData.Item1.direction,
                this.transform.localScale.y * 0.5f,
                groundMask
                ) ||
                Physics2D.Raycast(
                rayTupleData.Item2.origin,
                rayTupleData.Item2.direction,
                this.transform.localScale.y * 0.5f,
                groundMask
                );
    }

    private bool CheckBury()
    {
        var rayTupleData = CreateRay_Down();

        return
                Physics2D.Raycast(
                rayTupleData.Item1.origin,
                rayTupleData.Item1.direction,
                this.transform.localScale.y * 0.5f - 0.01f,
                groundMask
                ) ||
                Physics2D.Raycast(
                rayTupleData.Item2.origin,
                rayTupleData.Item2.direction,
                this.transform.localScale.y * 0.5f - 0.01f,
                groundMask
                );
    }
    private (Ray, Ray) CreateRay_Down()
    {
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.x += this.transform.localScale.x * 0.4f;

        Vector3 leftOrigin = this.transform.position;
        leftOrigin.x -= this.transform.localScale.x * 0.4f;

        return (new Ray(rightOrigin, -this.transform.up),
                new Ray(leftOrigin, -this.transform.up));
    }

    private (Ray, Ray) CreateRay_Up()
    {
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.x += this.transform.localScale.x * 0.4f;

        Vector3 leftOrigin = this.transform.position;
        leftOrigin.x -= this.transform.localScale.x * 0.4f;

        return (new Ray(rightOrigin, this.transform.up),
                new Ray(leftOrigin, this.transform.up));
    }

    private bool OnCeiling()
    {
        var rayTupleData = CreateRay_Up();

        return
             Physics2D.Raycast(
                    rayTupleData.Item1.origin,
                    rayTupleData.Item1.direction,
                    this.transform.localScale.y * 0.5f,
                    groundMask) ||
           Physics2D.Raycast(
                    rayTupleData.Item2.origin,
                    rayTupleData.Item2.direction,
                    this.transform.localScale.y * 0.5f,
                    groundMask);


    }

    private bool JumpAble()
    {
        return _jumpCount > 0;
    }
}