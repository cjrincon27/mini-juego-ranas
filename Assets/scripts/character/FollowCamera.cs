using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // Personaje a seguir
    public Vector3 offset = new Vector3(0, 5, -7); // Posición relativa a mantener
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(target); // La cámara siempre mira al personaje
        }
    }
}
