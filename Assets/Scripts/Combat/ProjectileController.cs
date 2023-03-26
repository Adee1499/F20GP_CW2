using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject HitEffectPrefab;
    public GameObject FlashEffectPrefab;
    public int Damage;
    public float Speed;
    public float MaxLifetime;

    float _destroyTime;
    Rigidbody rb;

    public static Action<int> OnProjectileCollision; // Pass damage amount when invoking this event

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _destroyTime = Time.time + MaxLifetime;

        if (FlashEffectPrefab != null) {
            GameObject flashEffect = Instantiate(FlashEffectPrefab, transform);
            flashEffect.transform.SetParent(null);

            DestroyGameObjectWhenParticleSystemFinished(flashEffect);
        }
    }

    void Update()
    {
        if (_destroyTime < Time.time) 
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Speed != 0)
            rb.velocity = transform.forward * Speed;
    }

    void OnTriggerEnter(Collider other) 
    {
        if (!other.name.Equals(gameObject.name) && !other.CompareTag("Merchant") && !other.CompareTag("Player")) {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Speed = 0;

            // Play the hit effect
            if (HitEffectPrefab != null) {
                Vector3 contactPoint = other.ClosestPoint(transform.position);
                Vector3 contactNormal = transform.position - contactPoint;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contactNormal);
                GameObject hitEffect = Instantiate(HitEffectPrefab, contactPoint + contactNormal * 0f, rotation);

                DestroyGameObjectWhenParticleSystemFinished(hitEffect);
            }

            // Deal damage
            if (other.CompareTag("Enemy")) {
                OnProjectileCollision?.Invoke(Damage);
            }

            Destroy(gameObject);
        }
    }

    void DestroyGameObjectWhenParticleSystemFinished(GameObject target)
    {
        ParticleSystem targetParticles = target.GetComponent<ParticleSystem>();
        if (targetParticles != null) {
            Destroy(target, targetParticles.main.duration);
        } else {
            Destroy(target, target.transform.GetChild(0).GetComponent<ParticleSystem>().main.duration);
        }
    }
}
