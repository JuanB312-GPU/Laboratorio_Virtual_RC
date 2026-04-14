using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CableCollider : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int segmentos = 10;
    [SerializeField] private float radio = 0.02f;

    private GameObject[] collidersGO;

    void Start() => GenerarColisiones();

    public void GenerarColisiones()
    {
        // Limpiar anteriores
        if (collidersGO != null)
            foreach (var go in collidersGO) Destroy(go);

        // Obtener vértices del mesh (puntos a lo largo del cable)
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;

        // Tomar puntos distribuidos uniformemente
        collidersGO = new GameObject[segmentos];
        int step = Mathf.Max(1, vertices.Length / segmentos);

        for (int i = 0; i < segmentos; i++)
        {
            int idxA = i * step;
            int idxB = Mathf.Min(idxA + step, vertices.Length - 1);

            Vector3 posA = transform.TransformPoint(vertices[idxA]);
            Vector3 posB = transform.TransformPoint(vertices[idxB]);

            Vector3 centro    = (posA + posB) / 2f;
            Vector3 direccion = posB - posA;
            float   longitud  = direccion.magnitude;

            GameObject go = new GameObject($"CableCol_{i}");
            go.transform.SetParent(transform);
            go.transform.position = centro;
            go.transform.up = direccion.normalized; // orienta la cápsula

            var cap = go.AddComponent<CapsuleCollider>();
            cap.radius    = radio;
            cap.height    = longitud + radio * 2f;
            cap.direction = 1; // Y axis, alineado con transform.up

            collidersGO[i] = go;
        }
    }

    // Visualización en editor
    void OnDrawGizmosSelected()
    {
        if (collidersGO == null) return;
        Gizmos.color = Color.cyan;
        foreach (var go in collidersGO)
            if (go != null) Gizmos.DrawWireSphere(go.transform.position, radio);
    }
}
