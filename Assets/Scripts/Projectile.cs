using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void Start()
    {
        // Destroi o projétil 2 segundos após ser instanciado
        Destroy(gameObject, 2f);
    }
}
