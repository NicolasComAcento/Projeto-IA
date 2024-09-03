using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void Start()
    {
        // Destroi o projétil 2 segundos após ser instanciado, para evitar ao acumulo de projeteis na tela.
        Destroy(gameObject, 2f);
    }
}
