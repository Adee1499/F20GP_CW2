using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gold : MonoBehaviour {
    public int amount;

    public static Action<int> OnGoldCollected;

    [SerializeField] private float _minDistance;
    private Transform _target;
    // Set a delay so that when gold is dropped from chests it won't instantly fly towards the player
    private bool _canDetectCollisions; 
    private bool _isFollowing;
    private Vector3 _velocity = Vector3.zero;
    private BoxCollider _boxCollider;
    private Rigidbody _rigidbody;

    void Awake() 
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start() 
    {
        StartCoroutine(SpawnDelay());
    }

    void Update() 
    {
        if (_isFollowing && _canDetectCollisions) {
            Destroy(_boxCollider);
            Destroy(_rigidbody);
            transform.position = Vector3.SmoothDamp(transform.position, _target.position + Vector3.up * 0.25f,
                ref _velocity, Time.deltaTime * Random.Range(5f, 15f));
            if (Vector3.Distance(transform.position, _target.position) < _minDistance) {
                OnGoldCollected?.Invoke(amount);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player")) {
            _isFollowing = true;
        }
    }

    IEnumerator SpawnDelay() 
    {
        yield return new WaitForSeconds(0.5f);
        _canDetectCollisions = true;
    }
}
