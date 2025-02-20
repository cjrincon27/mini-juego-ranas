using UnityEngine;

public class ClickController : MonoBehaviour
{
    public LayerMask frogLayer; // Layer asignada a las ranas para detectarlas

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Verifica si el objeto clicado es una rana
                if (!hit.collider.CompareTag("Frog"))
                {
                    // Si no es una rana, aplica el efecto en un radio de 1 metro
                    TriggerFleeEffect(hit.point, 1f);
                }
            }
        }
    }

    void TriggerFleeEffect(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius, frogLayer);

        foreach (Collider collider in colliders)
        {
            FrogMovement frogMovement = collider.GetComponent<FrogMovement>();
            if (frogMovement != null)
            {
                // Llama a Flee y envía la posición de origen del clic
                frogMovement.Flee(position);
            }
        }
    }
}
