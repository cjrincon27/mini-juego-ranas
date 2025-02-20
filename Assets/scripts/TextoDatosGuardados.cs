using UnityEngine;
using TMPro;

public class MostrarDatosGlobales : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoDatos;

    void Start()
    {
        ActualizarTexto();
    }

    public void ActualizarTexto()
    {
        if (textoDatos != null)
        {
            textoDatos.text = DatosGlobales.ObtenerTodosLosDatos();
        }
    }
}
