using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerHP = 5;

    private Vector2 movementDirection;
    private Rigidbody2D rb;
    private float changeDirectionTimer = 0f;
    private const float directionChangeInterval = 1.5f;

    // Limites de movimento do jogador
    private Vector2 spawnAreaMin = new Vector2(-8.34f, -1.42f);
    private Vector2 spawnAreaMax = new Vector2(0.06f, 3.68f);

    // Configurações do projétil
    public GameObject projectilePrefab; // Prefab do projétil
    private float shootTimer = 0f;
    private const float shootInterval = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChangeDirection();
    }

    private void Update()
    {
        changeDirectionTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;

        if (changeDirectionTimer >= directionChangeInterval)
        {
            ChangeDirection();
            changeDirectionTimer = 0f;
        }

        if (shootTimer >= shootInterval)
        {
            ShootProjectile();  // Lança um projétil a cada 1 segundo
            shootTimer = 0f;
        }

        MovePlayer();

        if (IsOutsideSpawnArea())
        {
            TeleportBackToArea();
        }
    }

    private void MovePlayer()
    {
        rb.velocity = movementDirection * 2f; // Ajuste a velocidade conforme necessário
    }

    private void ChangeDirection()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void ShootProjectile()
    {
        // Instancia o projétil na posição e rotação atual do jogador
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

        // Define as direções básicas (cima, baixo, esquerda, direita)
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,    // Cima
            Vector2.down,  // Baixo
            Vector2.left,  // Esquerda
            Vector2.right  // Direita
        };

        // Escolhe uma direção aleatória entre as quatro
        Vector2 shootDirection = directions[Random.Range(0, directions.Length)];

        // Aplica uma força ao projétil para lançá-lo na direção escolhida
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = shootDirection * 5f; // Ajuste a velocidade do projétil conforme necessário

        // Destrói o projétil após 2 segundos
        Destroy(projectile, 2f);
    }

    private bool IsOutsideSpawnArea()
    {
        return transform.localPosition.x < spawnAreaMin.x || transform.localPosition.x > spawnAreaMax.x ||
               transform.localPosition.y < spawnAreaMin.y || transform.localPosition.y > spawnAreaMax.y;
    }

    private void TeleportBackToArea()
    {
        transform.localPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            0
        );
    }

    public void TakeDamage()
    {
        playerHP--;
        FindObjectOfType<BossAgent>().UpdateHealthUI(); // Atualiza a UI ao receber dano

        Debug.Log("Player hit! Current HP: " + playerHP);

        if (playerHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player defeated!");
        FindObjectOfType<BossAgent>().CheckEndCondition(); // Verifica se o episódio deve terminar
    }
}
