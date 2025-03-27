using UnityEngine;
using UnityEngine.AI;

public class CharacterClickMove : MonoBehaviour
{
    public Camera mainCamera;        // Cámara principal
    public LayerMask groundLayer;    // Capa del suelo
    public float rotationSpeed = 5f; // Velocidad de giro

    private NavMeshAgent agent;
    private Animator animator; // Referencia al Animator

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Obtiene el Animator

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detecta clic izquierdo
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                agent.SetDestination(hit.point);
                animator.SetFloat("mov-x", 1f); // Activa la animación de caminar
            }
        }

        // Rotar el personaje en la dirección del movimiento
        if (agent.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Si el personaje se ha detenido, poner animación en 0
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetFloat("mov-x", 0f); // Detiene la animación
        }
    }
}
