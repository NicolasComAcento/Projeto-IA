using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;

public class PlayerAgent : Agent
{
    [SerializeField] private Transform bossTransform;
    [SerializeField] private GameObject[] walls;
    [SerializeField] private TextMeshProUGUI playerHealthText;

    private Rigidbody2D rb;
    private float safeTime;
    private const float safeTimeRewardInterval = 2f;

    public int playerHP = 5;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateHealthUI();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(
            Random.Range(-7.96f, -2.396f),
            Random.Range(3.75f, -1.45f),
            0
        );
        rb.velocity = Vector2.zero;

        playerHP = 5;
        safeTime = 0f;

        UpdateHealthUI();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(bossTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Vector2 movement = new Vector2(moveX, moveY) * Time.deltaTime * 100f;
        rb.velocity = movement;

        safeTime += Time.deltaTime;

        // Recompensa a cada 2 segundos sem tomar dano
        if (safeTime >= safeTimeRewardInterval)
        {
            SetReward(0.2f);
            safeTime = 0f;
        }

        if (IsOutsideSpawnArea())
        {
            TeleportBackToArea();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BossAgent>(out BossAgent boss))
        {
            TakeDamage();
            UpdateHealthUI();
            SetReward(-1.0f);
            if (playerHP <= 0)
            {
                EndEpisode();
            }
        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            TakeDamage();
            UpdateHealthUI();
            SetReward(-0.5f);
            if (playerHP <= 0)
            {
                EndEpisode();
            }
        }
    }

    private bool IsOutsideSpawnArea()
    {
        return transform.localPosition.x < -7.96f || transform.localPosition.x > -2.396f ||
               transform.localPosition.y < -1.45f || transform.localPosition.y > 3.75f;
    }

    private void TeleportBackToArea()
    {
        transform.localPosition = new Vector3(
            Random.Range(-7.96f, -2.396f),
            Random.Range(-1.45f, 3.75f),
            0
        );
    }

    public void TakeDamage()
    {
        playerHP--;
        Debug.Log("Player hit! Current HP: " + playerHP);
    }

    public void UpdateHealthUI()
    {
        if (playerHealthText != null)
        {
            playerHealthText.text = "Player HP: " + playerHP;
        }
    }
}
