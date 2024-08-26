using UnityEngine;
<<<<<<< Updated upstream
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    public GameObject frontalBeamPrefab;
    public GameObject skyBeamPrefab;
    public float attackInterval = 5f; // Time between attacks
    public float beamDuration = 4f; // Time the beams stay active
    public float skyBeamStartHeight = 10f; // Starting height of the sky beam
    public float skyBeamSpeed = 5f; // Speed at which the sky beam descends
    public float frontalBeamSpeed = 5f; // Speed at which the frontal beam moves forward
    public int bossHP = 50; // Boss health points
    public float escalaDePenalizacao = 1;
    public int countErros = 0;
    public int countAcertos = 0; // Contador de acertos
    public float damageCooldown = 0.2f; // Cooldown duration in seconds
    public float frontalBeamAdjustmentDistance = 0.5f; // Ajuste vertical da posição do feixe

    [HideInInspector]
    public Vector2 frontalBeamTargetPosition = new Vector2(-9, 0); // Posição alvo do ataque frontal

    [HideInInspector]
    public Vector2 skyBeamTargetPosition = new Vector2(-7, 0); // Posição alvo do ataque aéreo

    private BossAgent bossAgent;
    private bool canTakeDamage = true; // Flag to control damage cooldown

    private void Start()
    {
        bossAgent = GetComponent<BossAgent>();
        StartCoroutine(AttackRoutine());
=======

public class Boss : MonoBehaviour
{
    public int bossHP = 10;

    private Vector2 movementDirection;
    private Rigidbody2D rb;
    private float changeDirectionTimer = 0f;
    private const float directionChangeInterval = 2f;

    // Limites de movimento do boss
    private Vector2 spawnAreaMin = new Vector2(-7.23f, -0.92f);
    private Vector2 spawnAreaMax = new Vector2(6.97f, 3.5f);

    // Configurações do projétil
    public GameObject projectilePrefab; // Prefab do projétil
    private float shootTimer = 0f;
    private const float shootInterval = 1.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChangeDirection();
>>>>>>> Stashed changes
    }

    private void Update()
    {
<<<<<<< Updated upstream
        if (other.gameObject.CompareTag("Sword") && canTakeDamage)
        {
            bossHP--;
            Debug.Log("Boss hit! Current HP: " + bossHP);
=======
        changeDirectionTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;
>>>>>>> Stashed changes

        if (changeDirectionTimer >= directionChangeInterval)
        {
            ChangeDirection();
            changeDirectionTimer = 0f;
        }

        if (shootTimer >= shootInterval)
        {
            ShootProjectile();  // Lança um projétil a cada 1.5 segundos
            shootTimer = 0f;
        }

        MoveBoss();

        if (IsOutsideSpawnArea())
        {
            TeleportBackToArea();
        }
    }

    private void MoveBoss()
    {
        rb.velocity = movementDirection * 3f; // Ajuste a velocidade conforme necessário
    }

    private void ChangeDirection()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void ShootProjectile()
    {
        // Instancia o projétil na posição e rotação atual do boss
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
        projectileRb.velocity = shootDirection * 6f; // Ajuste a velocidade do projétil conforme necessário
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
        bossHP--;
        FindObjectOfType<PlayerAgent>().UpdateHealthUI(); // Atualiza a UI ao receber dano

        Debug.Log("Boss hit! Current HP: " + bossHP);

        if (bossHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
<<<<<<< Updated upstream
        Destroy(gameObject);
        bossAgent.AddReward(1.0f); // Recompensa ao derrotar o boss
        IncrementDifficulty(); // Incrementar dificuldade do Boss
        RestartLevel(); // Reiniciar o nível
    }

    private void IncrementDifficulty()
    {
        attackInterval = Mathf.Max(1f, attackInterval - 0.5f); // Diminuir o intervalo entre ataques
        skyBeamSpeed += 0.5f; // Aumentar a velocidade do feixe aéreo
        frontalBeamSpeed += 0.5f; // Aumentar a velocidade do feixe frontal
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            int attackChoice = Random.Range(0, 2); // 0 for frontal beam, 1 for sky beam

            if (attackChoice == 0)
            {
                StartCoroutine(FrontalBeamAttack());
            }
            else
            {
                StartCoroutine(SkyBeamAttack());
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void AdjustBeamTargetPosition(bool hitPlayer)
    {
        float adjustment = hitPlayer ? -frontalBeamAdjustmentDistance : frontalBeamAdjustmentDistance;
        frontalBeamTargetPosition.y += adjustment; // Ajusta a posição Y do ataque frontal
    }

    private IEnumerator FrontalBeamAttack()
    {
        Vector3 startPosition = new Vector3(
            transform.position.x - 1,
            transform.position.y,
            transform.position.z + 2
        );
        GameObject frontalBeam = Instantiate(frontalBeamPrefab, startPosition, Quaternion.identity);

        Vector3 endPosition = new Vector3(
            frontalBeamTargetPosition.x,
            frontalBeamTargetPosition.y, // Usar a posição ajustada
            transform.position.z + 2
        );

        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        bool hitPlayer = false;

        while (Time.time - startTime < beamDuration)
        {
            float distCovered = (Time.time - startTime) * frontalBeamSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            frontalBeam.transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                fractionOfJourney
            );

            if (frontalBeam.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Player")))
            {
                hitPlayer = true;
                break;
            }

            yield return null;
        }

        Destroy(frontalBeam);

        if (hitPlayer)
        {
            countAcertos++; // Incrementar contador de acertos
            bossAgent.AddReward(1.0f); // Recompensa ao acertar o jogador
            
            // Reduzir a penalização a cada 5 acertos
            if (countAcertos >= 5)
            {
                countAcertos = 0;
                escalaDePenalizacao = Mathf.Max(1, escalaDePenalizacao - 0.1f); // Não deixar a penalização abaixo de 1
            }
        }
        else
        {
            countErros++;
            if (countErros == 10)
            {
                countErros = 0;
                escalaDePenalizacao += 0.1f;
            }
            bossAgent.AddReward(-1.0f * escalaDePenalizacao); // Penalidade ao errar
        }

        AdjustBeamTargetPosition(hitPlayer); // Ajustar a posição após cada ataque
    }

    private IEnumerator SkyBeamAttack()
    {
        Vector3 startPosition = new Vector3(
            transform.position.x + skyBeamTargetPosition.x,
            transform.position.y + skyBeamStartHeight,
            transform.position.z + 2
        );
        GameObject skyBeam = Instantiate(skyBeamPrefab, startPosition, Quaternion.identity);

        Vector3 endPosition = new Vector3(
            skyBeamTargetPosition.x,
            transform.position.y,
            transform.position.z + 2
        );

        float elapsedTime = 0f;
        bool hitPlayer = false;

        while (elapsedTime < beamDuration)
        {
            skyBeam.transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                elapsedTime / beamDuration
            );
            elapsedTime += Time.deltaTime;

            if (skyBeam.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Player")))
            {
                hitPlayer = true;
                break;
            }

            yield return null;
        }

        Destroy(skyBeam);

        if (hitPlayer)
        {
            countAcertos++; // Incrementar contador de acertos
            bossAgent.AddReward(1.0f); // Recompensa ao acertar o jogador
            
            // Reduzir a penalização a cada 5 acertos
            if (countAcertos >= 5)
            {
                countAcertos = 0;
                escalaDePenalizacao = Mathf.Max(1, escalaDePenalizacao - 0.1f); // Não deixar a penalização abaixo de 1
            }
        }
        else
        {
            bossAgent.AddReward(-1.0f); // Penalidade ao errar
        }

        // Ajuste a posição do ataque aéreo se necessário
        AdjustBeamTargetPosition(hitPlayer); // Opcional, se você quiser aplicar a mesma lógica
=======
>>>>>>> Stashed changes
    }
}
