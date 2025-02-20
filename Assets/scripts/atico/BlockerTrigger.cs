using UnityEngine;

public class BlockerTrigger : MonoBehaviour
{
    public int bloqueadorID; // ID único para cada bloqueador

    void Start()
    {
        // Si la misión correspondiente a este bloqueador ya está completada, lo desactivamos
        if (PlayerPrefs.GetInt("Mision_" + bloqueadorID, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BlockManager.instance.BloqueoActivado(bloqueadorID);
        }
    }
}
