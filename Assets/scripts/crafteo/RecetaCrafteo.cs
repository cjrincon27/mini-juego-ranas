using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemRequerido
{
    public DraggableItem item; // Referencia al prefab del item
    public int cantidad = 1;   // Cantidad necesaria
}

[CreateAssetMenu(fileName = "Nueva Receta", menuName = "Crafteo/Receta")]
public class RecetaCrafteo : ScriptableObject
{
    public string nombreReceta;
    public List<ItemRequerido> itemsRequeridos = new List<ItemRequerido>();
    public DraggableItem itemResultado;
    public int cantidadResultado = 1;
}