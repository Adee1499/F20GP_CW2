using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(EquipmentManager))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpellCaster))]
public class PlayerStateMachine : MonoBehaviour
{
    // Component references
    [SerializeField] Animator _animator;
    CharacterController _characterController;
    Controls _controls;
    PlayerInput _playerInput;
    CinemachineVirtualCamera _virtualCamera;
    [SerializeField] Inventory _inventory;
    [SerializeField] InventoryUI _inventoryUI;
    private Inventory _activeInventory;
    XPSystem _xpSystem;
    SpellCaster _spellCaster;
    Slider healthGlobe, manaGlobe, xpBar;
    TMP_Text playerLevel, goldCount;
    public Sprite weaponEquipActive, weaponEquipInactive, skill1Active, skill1Inactive, skill2Active, skill2Inactive;
    Image weaponEquip, skill1, skill2;

    // Animator hashed variables
    int _animMoveXHash;
    int _animMoveYHash;
    int _animMeleeAttackHash;
    int _animPickUpHash;
    int _animRollHash;
    int _animProjectileSpellHash;
    int _animAOESpellHash;
    int _animDrinkHash;
    int _animDeathHash;

    // Movement variables
    [Header("Controls & Movement")]
    [SerializeField] float _walkSpeed = 2f;
    [SerializeField] float _runSpeed = 2f;
    [SerializeField] float _rotationSpeed = 30f;
    Vector2 _currentMovementInput;
    Vector2 _currentAimInput;
    Vector2 _lastAimInput;
    Vector3 _currentTargetPosition;
    Vector3 _appliedMovement;
    float _movementMultiplier = 1f;
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isInteractPressed;
    bool _isRollPressed;
    bool _isLookAtPressed;
    [SerializeField] float _interactionRange = 2f;

    // HP & MP variables
    [Header("Health & Mana")]
    [SerializeField] float _maxPlayerHP;
    [SerializeField] float _maxPlayerMP;
    float _playerHP;
    float _playerMP;
    [Tooltip("Mana Points regenerated per second")]
    [SerializeField] float _manaRegenRate;

    // Combat
    bool _isAttackPressed;
    int _currentSelectedSkill;


    // State Machine variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // Getters & Setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; }}
    public Animator Animator { get { return _animator; }}
    public int AnimMeleeAttackHash { get { return _animMeleeAttackHash; }}
    public int AnimPickUpHash { get { return _animPickUpHash; }}
    public int AnimRollHash { get { return _animRollHash; }}
    public int AnimProjectileSpellHash { get { return _animProjectileSpellHash; }}
    public int AnimAOESpellHash { get { return _animAOESpellHash; }}
    public Vector3 CurrentMouseTargetPosition { get { return _currentTargetPosition; }}
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; }}
    public Vector3 AppliedMovement { get { return _appliedMovement; } set { _appliedMovement = value; }}
    public float MovementMultiplier { get { return _movementMultiplier; } set { _movementMultiplier = value; }}
    public bool IsMovementPressed { get { return _isMovementPressed; }}
    public bool IsRunPressed { get { return _isRunPressed; }}
    public bool IsAttackPressed { get { return _isAttackPressed; }}
    public bool IsInteractPressed { get { return _isInteractPressed; }}
    public bool IsRollPressed { get { return _isRollPressed; }}
    public bool IsLookAtPressed { get { return _isLookAtPressed; }}
    public float WalkSpeed { get { return _walkSpeed; }}
    public float RunSpeed { get { return _runSpeed; }}
    public float InteractionRange { get { return _interactionRange; }}
    public float MaxPlayerHealth { get { return _maxPlayerHP; }}
    public float MaxPlayerMana { get { return _maxPlayerMP; }}
    public float PlayerHealth { get { return _playerHP; }}
    public float PlayerMana { get { return _playerMP; }}
    public int CurrentSelectedSkill { get { return _currentSelectedSkill; }}
    public SpellCaster SpellCaster { get { return _spellCaster; }}

    // Events
    public static event Action<float> OnPlayerHealthChange;
    public static event Action<float> OnPlayerManaChange;
    public static event Action OnPlayerDead;

    bool _interactingWithUI = false;
    
    void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Equipment")) {
            Debug.Log("Ignoring collision?");
            Physics.IgnoreCollision(other.collider, GetComponent<Collider>());
        }
    }

    void Awake()
    {
        // Initialize reference variables
        _characterController = GetComponent<CharacterController>();
        _controls = new Controls();
        _playerInput = GetComponent<PlayerInput>();
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (_virtualCamera == null)
            Debug.LogError("Cinemachine virtual camera not found in the scene!");
        _activeInventory = Instantiate(_inventory);
        _inventoryUI.inventory = _activeInventory;
        _spellCaster = GetComponent<SpellCaster>();

        // Setup FSM
        _states = new PlayerStateFactory(this);
        _currentState = _states.Default();
        _currentState.EnterState();

        _playerHP = _maxPlayerHP;
        _playerMP = _maxPlayerMP;

        _currentSelectedSkill = 1;

        // Set animator hash variables
        _animMoveXHash = Animator.StringToHash("MoveX");
        _animMoveYHash = Animator.StringToHash("MoveY");
        _animMeleeAttackHash = Animator.StringToHash("MeleeAttack");
        _animPickUpHash = Animator.StringToHash("PickUp");
        _animRollHash = Animator.StringToHash("Roll");
        _animProjectileSpellHash = Animator.StringToHash("ProjectileSpell");
        _animAOESpellHash = Animator.StringToHash("AOESpell");
        _animDrinkHash = Animator.StringToHash("Drink");

        // Set PlayerInput callbacks
        _controls.Player.Move.started += OnMovementInput;
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;

        _controls.Player.Run.started += OnRunInput;
        _controls.Player.Run.canceled += OnRunInput;

        _controls.Player.Interact.started += OnInteractInput;
        _controls.Player.Interact.canceled += OnInteractInput;

        _controls.Player.Attack.started += OnAttackInput;
        _controls.Player.Attack.canceled += OnAttackInput;

        _controls.Player.Roll.started += OnRollInput;
        _controls.Player.Roll.canceled += OnRollInput;

        _controls.Player.Aim.started += OnAimInput;
        _controls.Player.Aim.performed += OnAimInput;
        _controls.Player.Aim.canceled += OnAimInput;

        _controls.Player.LookAt.started += OnLookAtInput;
        _controls.Player.LookAt.canceled += OnLookAtInput;

        _controls.Player.Inventory.started += OnInventoryInput;   
        _controls.Player.Inventory.canceled += OnInventoryInput;

        _controls.Player.Hotbar1.started += ctx => { 
            _currentSelectedSkill = 1;
            print($"Selected skill {_currentSelectedSkill}");
            weaponEquip.sprite = weaponEquipActive;
            skill1.sprite = skill1Inactive;
            skill2.sprite = skill2Inactive;
        };
        _controls.Player.Hotbar2.started += ctx => { 
            _currentSelectedSkill = 2; 
            print($"Selected skill {_currentSelectedSkill}");
            weaponEquip.sprite = weaponEquipInactive;   
            skill1.sprite = skill1Active;
            skill2.sprite = skill2Inactive;
        };
        _controls.Player.Hotbar3.started += ctx => { 
            _currentSelectedSkill = 3; 
            print($"Selected skill {_currentSelectedSkill}"); 
            weaponEquip.sprite = weaponEquipInactive;   
            skill1.sprite = skill1Inactive;
            skill2.sprite = skill2Active;
        };
        _controls.Player.Hotbar4.started += ctx => { _currentSelectedSkill = 4; print($"Selected skill {_currentSelectedSkill}"); };
        _controls.Player.Hotbar5.started += ctx => { _currentSelectedSkill = 5; print($"Selected skill {_currentSelectedSkill}"); };

        PotionItem.OnPotionConsumed += OnPotionConsumed;
        Gold.OnGoldCollected += OnGoldCollected;
    }

    void Start()
    {
        _xpSystem = new XPSystem();

        weaponEquip = GameObject.FindWithTag("WeaponEquip").GetComponent<Image>();
        skill1 = GameObject.FindWithTag("Skill1").GetComponent<Image>();
        skill2 = GameObject.FindWithTag("Skill2").GetComponent<Image>();
        playerLevel = GameObject.FindWithTag("PlayerLevel").GetComponent<TMP_Text>();
        goldCount = GameObject.FindWithTag("GoldCount").GetComponent<TMP_Text>();
        xpBar = GameObject.FindWithTag("XPBar").GetComponent<Slider>();
        healthGlobe = GameObject.FindWithTag("HealthGlobe").GetComponent<Slider>();
        healthGlobe.maxValue = _maxPlayerHP;
        healthGlobe.value = _maxPlayerHP;
        manaGlobe = GameObject.FindWithTag("ManaGlobe").GetComponent<Slider>();
        manaGlobe.maxValue = _maxPlayerMP;
        manaGlobe.value = _maxPlayerMP;

        // bind action of player being hit
        EnemyController.OnEnemyAttackPlayer += TakeDamage;
        Firebolt.OnProjectileHitPlayer += TakeDamage;
    }
   
    void OnMovementInput (InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput != Vector2.zero;
    }

    void OnRunInput (InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void OnInteractInput (InputAction.CallbackContext context)
    {
        _isInteractPressed = context.ReadValueAsButton();
    }

    void OnAttackInput (InputAction.CallbackContext context)
    {
        if (!_interactingWithUI)
            _isAttackPressed = context.ReadValueAsButton();
    }

    void OnRollInput (InputAction.CallbackContext context)
    {
        _isRollPressed = context.ReadValueAsButton();
    }

    void OnAimInput (InputAction.CallbackContext context)
    {
        _currentAimInput = context.ReadValue<Vector2>();
    }

    void OnLookAtInput (InputAction.CallbackContext context)
    {
        _isLookAtPressed = context.ReadValueAsButton();
    }

    void OnInventoryInput (InputAction.CallbackContext context)
    {
        InventoryUI.Instance.UI_Merchant.SetActive(false);
        if(context.ReadValueAsButton()) {
            if (InventoryUI.Instance.UI_Inventory.activeSelf || InventoryUI.Instance.UI_Equipment.activeSelf) {
                InventoryUI.Instance.UI_Inventory.SetActive(false);
                InventoryUI.Instance.UI_Equipment.SetActive(false);
                InventoryUI.Instance.ItemTooltip.SetActive(false);
            } else {
                InventoryUI.Instance.UI_Inventory.SetActive(true);
                InventoryUI.Instance.UI_Equipment.SetActive(true);
            }
        }
    }

    void HandleRotation()
    {
        Quaternion currentRotation = transform.rotation;
        if (_isLookAtPressed) {
            Ray ray = Camera.main.ScreenPointToRay(_currentAimInput);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                _currentTargetPosition = hit.point;
                Vector3 targetDirection = hit.point - transform.position;
                targetDirection.y = 0f;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        } else {
            if (new Vector2(_appliedMovement.x, _appliedMovement.z).magnitude > 0f) {
                Vector3 positionToLookAt = new Vector3(_appliedMovement.x, 0f, _appliedMovement.z);
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt, Vector3.up);
                
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
            } else {
                transform.rotation = currentRotation;
            }
        }
    }

    void OnDrawGizmos() 
    {
        if (_isLookAtPressed) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(_currentTargetPosition, _currentTargetPosition + Vector3.up * 1.5f);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactionRange);
    }

    void Update()
    {
        CheckInteractingWithUI();
        HandleRotation();
        _currentState.UpdateStates();
        // Transform the movement vector relative to the camera
        TransformMovementVector();
        _characterController.Move(_appliedMovement * _movementMultiplier * Time.deltaTime);
        if (_isLookAtPressed) {
            Vector3 moveVector = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * _appliedMovement;
            _animator.SetFloat(_animMoveXHash, moveVector.x);
            _animator.SetFloat(_animMoveYHash, moveVector.z);
        } else {
            _animator.SetFloat(_animMoveXHash, 0f);
            _animator.SetFloat(_animMoveYHash, new Vector2(_appliedMovement.x, _appliedMovement.z).magnitude);
        }

        goldCount.text = _activeInventory.gold.ToString();
        XPStatus();
        RegenerateMana();
    }

    void CheckInteractingWithUI()
    {
        _interactingWithUI = EventSystem.current.IsPointerOverGameObject() || InventoryUI.Instance.CurrentItem != null;
    }

    void TransformMovementVector()
    {
        if (_virtualCamera != null) {
            Vector3 cameraForward = _virtualCamera.transform.forward;
            Vector3 cameraRight = _virtualCamera.transform.right;

            // Ignore the y components
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            // Normalize the vectors to get the direction
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction based on the orientation
            _appliedMovement = cameraForward * _appliedMovement.z + cameraRight * _appliedMovement.x;
            if (_appliedMovement != Vector3.zero) {
                _animator.ResetTrigger(_animPickUpHash);
            }
            _appliedMovement.y -= 9.8f;
        }
    }

    void OnEnable() 
    {
        _controls.Player.Enable();
    }

    void OnDisable()
    {
        _controls.Player.Disable();
    }

    // TODO: Move take damage method to PlayerDefaultState
    void TakeDamage(float damage)
    {
        //Debug.Log("ow ow ow");
        _playerHP -= damage;
        healthGlobe.value = _playerHP;
        OnPlayerHealthChange?.Invoke(_playerHP);
        if (_playerHP <= 0) {
            PlayerDead();
        }

        //Debug.Log(_playerHP);
    }

    public void UseMana(int amount)
    {
        _playerMP -= amount;
        OnPlayerManaChange?.Invoke(_playerMP);
    }

    void RegenerateMana()
    {
        if (_playerMP < _maxPlayerMP) {
            _playerMP += _manaRegenRate * Time.deltaTime;
            manaGlobe.value = _playerMP;
            OnPlayerManaChange?.Invoke(_playerMP);
        }
    }

    void XPStatus()
    {
        xpBar.maxValue = _xpSystem.XPToNextLevel;
        xpBar.value = _xpSystem.CurrentXP;
        playerLevel.text = "Level " + _xpSystem.GetCurrentLevel();
    }

    void PlayerDead()
    {
        OnPlayerDead?.Invoke();
        _controls.Disable();
        _characterController.enabled = false;
        _animator.SetTrigger(_animDeathHash);
    }

    void OnPotionConsumed(PotionEffect effectType, int effectValue) 
    {
        _animator.SetTrigger(_animDrinkHash);
        switch (effectType) {
            case PotionEffect.RestoreHealth:
                _playerHP += effectValue;
                if (_playerHP > _maxPlayerHP) _playerHP = _maxPlayerHP;
                break;
            case PotionEffect.RestoreMana:
                _playerMP += effectValue;
                if (_playerMP > _maxPlayerMP) _playerMP = _maxPlayerMP;
                break;
        }
    }

    void OnGoldCollected(int amount) 
    {
        _activeInventory.gold += amount;
    }
}
