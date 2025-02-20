using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    [SerializeField] private float minSpeed = 0f;
    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float boostDuration = 2f;//esta avriable se divide en 3 
    [SerializeField] private float collisionSlowDuration = 2f;
    [SerializeField] private float minChangeInterval = 5f;
    [SerializeField] private float maxChangeInterval = 15f;
    [SerializeField] private float lifetime;

    private float speed;
    private float initialSpeed;
    private float boostTimer;
    private float speedChangeTimer;
    private float directionChangeTimer;
    private Rigidbody rb;
    private Vector3 direction;
    private bool isBoosted = false;
    private int boostChangeCounter = 0;//ste ews nuevo el nuevo tiempo
    private float nextBoostChangeTime;//siguiente 
    private bool isColliding = false;
    private float collisionTimer;
    private int frogType;
    private Animator animator;
    private bool initialMovement = true; // Controla el primer movimiento hacia -z

    public void SetLifetime(float time)
    {
        lifetime = time;
        Destroy(gameObject, lifetime);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        SetRandomSpeed();
        direction = Vector3.back; // Inicia con dirección hacia -z
        initialSpeed = speed;
        ResetTimers();
    }

    public void SetRandomSpeed()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        initialSpeed = speed;
    }

    private void SetRandomDirection()
    {
        float xDir = Random.Range(-1f, 1f);
        float zDir = Random.Range(-1f, 1f);
        direction = new Vector3(xDir, 0, zDir).normalized;
    }

    private void ResetTimers()
    {
        speedChangeTimer = Random.Range(minChangeInterval, maxChangeInterval);
        directionChangeTimer = Random.Range(minChangeInterval, maxChangeInterval);
    }

    void Update()
    {
        HandleMovement();
        HandleTimers();
        UpdateAnimationSpeed();
    }

    private void HandleMovement()
    {
        if (isColliding)
    {
        rb.linearVelocity = direction * 0.2f;
        collisionTimer -= Time.deltaTime;

        if (collisionTimer <= 0f)
        {
            isColliding = false;
            speed = initialSpeed;
        }
    }
    else
    {
        rb.linearVelocity = direction * speed;
    }

    if (initialMovement)
    {
        if (speedChangeTimer <= 0f)
        {
            initialMovement = false;
            SetRandomDirection();
        }
    }
    
    if (direction != Vector3.zero)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    if (isBoosted)
    {
        boostTimer -= Time.deltaTime;

        // Cambiar dirección 3 veces durante el boost
        if (boostChangeCounter < 3 && boostTimer <= nextBoostChangeTime)
        {
            SetRandomDirection();
            boostChangeCounter++;
            nextBoostChangeTime = boostTimer - (boostDuration / 3f);
        }

        if (boostTimer <= 0f)
        {
            isBoosted = false;
            speed = initialSpeed;
            boostChangeCounter = 0; // Reiniciar contador
        }
    }
    }

    private void HandleTimers()
    {
        if (!initialMovement)
        {
            speedChangeTimer -= Time.deltaTime;
            if (speedChangeTimer <= 0f && !isBoosted && !isColliding)
            {
                SetRandomSpeed();
                speedChangeTimer = Random.Range(minChangeInterval, maxChangeInterval);
            }
        }

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0f)
        {
            SetRandomDirection();
            directionChangeTimer = Random.Range(minChangeInterval, maxChangeInterval);
        }
    }

    private void UpdateAnimationSpeed()
    {
        if (animator != null)
        {
            float normalizedSpeed = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
            animator.SetFloat("speed", normalizedSpeed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Frog") || collision.gameObject.CompareTag("Piscina"))
        {
            ReflectDirection(collision.contacts[0].normal);
            isColliding = true;
            collisionTimer = collisionSlowDuration;
        }
    }

    private void ReflectDirection(Vector3 collisionNormal)
    {
        direction = Vector3.Reflect(direction, collisionNormal);
        float randomAngle = Random.Range(-20f, 20f);
        Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
        direction = rotation * direction;
    }

    private void OnMouseDown()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        speed = 1.5f;
        isBoosted = true;
        boostTimer = boostDuration;
    
        boostChangeCounter = 0;
        nextBoostChangeTime = boostTimer - (boostDuration / 3f); // Primer cambio de dirección
    
        float randomEscapeAngle = Random.Range(120f, 240f);
        Quaternion escapeRotation = Quaternion.Euler(0, randomEscapeAngle, 0);
        direction = escapeRotation * direction;
    }

    public void Flee(Vector3 fleeOrigin)
    {
        speed = 1f;
        isBoosted = true;
        boostTimer = boostDuration;
        Vector3 fleeDirection = (transform.position - fleeOrigin).normalized;
        float randomEscapeAngle = Random.Range(-30f, 30f);
        Quaternion escapeRotation = Quaternion.Euler(0, randomEscapeAngle, 0);
        direction = escapeRotation * fleeDirection;
    }

    public int GetFrogType()
    {
        return frogType;
    }

    public void SetFrogType(int type)
    {
        frogType = type;
    }
}
