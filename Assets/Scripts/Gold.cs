using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gold : MonoBehaviour {
    public int amount;

    public static Action<int> OnGoldCollected;
    
    [SerializeField] private float _minDistance;
    private Transform _target;
    private bool _isFollowing;
    private Vector3 _velocity = Vector3.zero;

    void Awake() {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        if (_isFollowing) {
            transform.position = Vector3.SmoothDamp(transform.position, _target.position,
                ref _velocity, Time.deltaTime * Random.Range(5f, 15f));
            if (Vector3.Distance(transform.position, _target.position) < _minDistance) {
                OnGoldCollected?.Invoke(amount);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _isFollowing = true;
        }
    }
}
