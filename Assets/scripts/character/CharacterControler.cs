using UnityEngine;
using UnityEngine.AI;

public class CharacterClickMove : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayer;
    public float rotationSpeed = 5f;
    public float holdThreshold = 0.15f; // Tiempo mínimo para considerar el clic como "sostenido"

    private NavMeshAgent agent;
    private Animator animator;

    private bool isHoldingClick = false;
    private float clickStartTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickStartTime = Time.time;
            isHoldingClick = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldingClick = false;
        }

        if (Input.GetMouseButton(0))
        {
            float heldTime = Time.time - clickStartTime;

            if (heldTime >= holdThreshold)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
                {
                    agent.SetDestination(hit.point);
                    animator.SetFloat("mov-x", 1f); // Activar animación
                }
            }
        }

        // Rotación hacia la dirección del movimiento
        if (agent.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Detener animación si el personaje ha llegado
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetFloat("mov-x", 0f); // Detener animación
        }
    }
}
