using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Boss : MonoBehaviour
{
    public GameObject frontalBeamPrefab;
    public GameObject skyBeamPrefab;
    public float attackInterval = 5f;
    public float beamDuration = 4f;
    public float skyBeamStartHeight = 10f;
    public float skyBeamSpeed = 5f;
    public float frontalBeamSpeed = 5f;
    public int bossHP = 10; // Atualizado para 10
    public float escalaDePenalizacao = 1;
    public int countErros = 0;
    public int countAcertos = 0;
    public float damageCooldown = 0.2f;
    public float frontalBeamAdjustmentDistance = 0.5f;
    public TextMeshProUGUI bossHealthText; // Referência ao TextMeshPro

    [HideInInspector]
    public Vector2 frontalBeamTargetPosition = new Vector2(-9, 0);

    [HideInInspector]
    public Vector2 skyBeamTargetPosition = new Vector2(-7, 0);

    private BossAgent bossAgent;
    private bool canTakeDamage = true;

    private void Start()
    {
        bossAgent = GetComponent<BossAgent>();
        StartCoroutine(AttackRoutine());
        UpdateBossHealthText(); // Atualiza o texto da vida do Boss no início
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Sword") && canTakeDamage)
        {
            bossHP--;
            Debug.Log("Boss hit! Current HP: " + bossHP);
            UpdateBossHealthText(); // Atualiza o texto da vida do Boss quando ele é atingido

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
        bossAgent.AddReward(1.0f);
        EndEpisode();
    }

    public void EndEpisode()
    {
        Debug.Log("Episode Ended.");
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
            int attackChoice = Random.Range(0, 2);

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
        frontalBeamTargetPosition.y += adjustment;
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
            frontalBeamTargetPosition.y,
            transform.position.z + 2
        );

        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        bool hitPlayer = false;

        while (Time.time - startTime < beamDuration)
        {
            float distCovered = (Time.time - startTime) * frontalBeamSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            frontalBeam.transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

            if (frontalBeam.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Player")))
            {
                hitPlayer = true;
                Player player = frontalBeam.GetComponent<Collider2D>().GetComponent<Player>();
                player.TakeDamage(); // Atinge o player
                break;
            }

            yield return null;
        }

        Destroy(frontalBeam);

        if (hitPlayer)
        {
            countAcertos++;
            bossAgent.AddReward(1.0f);

            if (countAcertos >= 5)
            {
                countAcertos = 0;
                escalaDePenalizacao = Mathf.Max(1, escalaDePenalizacao - 0.1f);
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
            bossAgent.AddReward(-1.0f * escalaDePenalizacao);
        }

        AdjustBeamTargetPosition(hitPlayer);
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
            skyBeam.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / beamDuration);
            elapsedTime += Time.deltaTime;

            if (skyBeam.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Player")))
            {
                hitPlayer = true;
                Player player = skyBeam.GetComponent<Collider2D>().GetComponent<Player>();
                player.TakeDamage(); // Atinge o player
                break;
            }

            yield return null;
        }

        Destroy(skyBeam);

        if (hitPlayer)
        {
            countAcertos++;
            bossAgent.AddReward(1.0f);

            if (countAcertos >= 5)
            {
                countAcertos = 0;
                escalaDePenalizacao = Mathf.Max(1, escalaDePenalizacao - 0.1f);
            }
        }
        else
        {
            bossAgent.AddReward(-1.0f);
        }

        AdjustBeamTargetPosition(hitPlayer);
    }

    private void UpdateBossHealthText()
    {
        if (bossHealthText != null)
        {
            bossHealthText.text = "Boss Health: " + bossHP.ToString();
        }
    }
}
