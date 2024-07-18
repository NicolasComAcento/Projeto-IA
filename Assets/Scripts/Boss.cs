using System.Collections;
using UnityEngine;
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Sword") && canTakeDamage)
        {
            bossHP--;
            Debug.Log("Boss hit! Current HP: " + bossHP);

            if (bossHP <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
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
    }
}
