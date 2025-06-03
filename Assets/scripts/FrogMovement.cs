using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    // Estos serán asignados por FrogSpawner mediante ConfigureMovement(...)
    private float minSpeed;
    private float maxSpeed;
    private float boostDuration;
    private float minChangeInterval;
    private float maxChangeInterval;

    [SerializeField] private float collisionSlowDuration = 2f; 
    private float lifetime;

    private float speed;
    private float initialSpeed;
    private float boostTimer;
    private float speedChangeTimer;
    private float directionChangeTimer;
    private Rigidbody rb;
    private Vector3 direction;
    private bool isBoosted = false;
    private int boostChangeCounter = 0;
    private float nextBoostChangeTime;
    private bool isColliding = false;
    private float collisionTimer;
    private int frogType;
    private Animator animator;
    private bool initialMovement = true; // Controla el primer movimiento hacia -z

    /// <summary>
    /// Se llama desde FrogSpawner justo después de AddComponent<FrogMovement>().
    /// </summary>
    public void ConfigureMovement(
        float _minSpeed,
        float _maxSpeed,
        float _boostDuration,
        float _minChangeInterval,
        float _maxChangeInterval
    )
    {
        minSpeed = _minSpeed;
        maxSpeed = _maxSpeed;
        boostDuration = _boostDuration;
        minChangeInterval = _minChangeInterval;
        maxChangeInterval = _maxChangeInterval;
    }

    public void SetLifetime(float time)
    {
        lifetime = time;
        Destroy(gameObject, lifetime);
    }

    public void SetFrogType(int type)
    {
        frogType = type;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Inicialmente definimos una velocidad aleatoria dentro del rango recibido
        SetRandomSpeed();

        // Dirección inicial hacia -Z
        direction = Vector3.back;

        initialSpeed = speed;
        ResetTimers();
    }

    /// <summary>
    /// Asigna speed = un valor aleatorio entre [minSpeed, maxSpeed].
    /// </summary>
    public void SetRandomSpeed()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        initialSpeed = speed;
    }

    /// <summary>
    /// Elige una dirección aleatoria normalizada en X-Z.
    /// </summary>
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

        // Si aún está en el primer movimiento (lleva la dirección Vector3.back),
        // solo cambia de dirección cuando speedChangeTimer llegue a cero.
        if (initialMovement)
        {
            if (speedChangeTimer <= 0f)
            {
                initialMovement = false;
                SetRandomDirection();
            }
        }

        // Rotación suave para que "mire" hacia donde va
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
                boostChangeCounter = 0;
            }
        }
    }

    private void HandleTimers()
    {
        if (!initialMovement)
        {
            speedChangeTimer -= Time.deltaTime;
            // Cuando toca cambiar velocidad (y no está en boost ni colisión), elige nueva
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
        speed = initialSpeed * 1.5f;       // Ejemplo: boost = 1.5× velocidad inicial
        isBoosted = true;
        boostTimer = boostDuration;

        boostChangeCounter = 0;
        nextBoostChangeTime = boostTimer - (boostDuration / 3f);

        float randomEscapeAngle = Random.Range(120f, 240f);
        Quaternion escapeRotation = Quaternion.Euler(0, randomEscapeAngle, 0);
        direction = escapeRotation * direction;
    }

    /// <summary>
    /// Se puede llamar desde otro script (ej. CaptureArea) para huir de un punto dado.
    /// </summary>
    public void Flee(Vector3 fleeOrigin)
    {
        speed = initialSpeed * 1f;   // Ejemplo: flee con velocidad igual a initialSpeed
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
}
