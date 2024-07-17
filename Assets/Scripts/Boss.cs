using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject frontalBeamPrefab;
    public GameObject skyBeamPrefab;
    public float attackInterval = 5f; // Time between attacks
    public float beamDuration = 4f; // Time the beams stay active
    public float skyBeamStartHeight = 10f; // Starting height of the sky beam
    public float skyBeamSpeed = 5f; // Speed at which the sky beam descends
    public float frontalBeamSpeed = 5f; // Speed at which the frontal beam moves forward
    public int bossHP = 20; // Boss health points
    public float escalaDePenalizacao = 1;
    public int countErros = 0;
    public float damageCooldown = 0.2f; // Cooldown duration in seconds

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
        // Verifica se o objeto colidido é a "Sword" e se o Boss pode1 receber dano
        if (other.gameObject.CompareTag("Sword") && canTakeDamage && !this.gameObject.CompareTag("Beam"))
        {
            // Verifica se o objeto colidido não é um "Beam"
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
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
        bossAgent.AddReward(1.0f); // Recompensa ao derrotar o boss
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

    private IEnumerator FrontalBeamAttack()
    {
        Vector3 startPosition = new Vector3(
            transform.position.x - 1,
            transform.position.y,
            transform.position.z + 2
        );
        GameObject frontalBeam = Instantiate(frontalBeamPrefab, startPosition, Quaternion.identity);
        // Não parentear o feixe ao boss
        // frontalBeam.transform.parent = transform; // Removido

        Vector3 endPosition = new Vector3(
            frontalBeamTargetPosition.x,
            transform.position.y,
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
            bossAgent.AddReward(1.0f); // Recompensa ao acertar o jogador
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
    }

    private IEnumerator SkyBeamAttack()
    {
        Vector3 startPosition = new Vector3(
            transform.position.x + skyBeamTargetPosition.x,
            transform.position.y + skyBeamStartHeight,
            transform.position.z + 2
        );
        GameObject skyBeam = Instantiate(skyBeamPrefab, startPosition, Quaternion.identity);
        // Não parentear o feixe ao boss
        // skyBeam.transform.parent = transform; // Removido

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
            bossAgent.AddReward(1.0f); // Recompensa ao acertar o jogador
        }
        else
        {
            bossAgent.AddReward(-1.0f); // Penalidade ao errar
        }
    }
}
