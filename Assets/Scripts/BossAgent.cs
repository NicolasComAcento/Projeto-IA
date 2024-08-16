using TMPro;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BossAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject[] floorObjects;
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI bossHealthText;
    [SerializeField] private TextMeshProUGUI playerHealthText;

    private Rigidbody2D rb;
    private float timer;
    private const float TimeLimit = 20f;

    public int bossHP = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateHealthUI();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        rb.velocity = Vector2.zero;
        timer = 0f;

        player.transform.localPosition = new Vector3(
            Random.Range(-7.96f, -2.396f),
            Random.Range(3.75f, -1.45f),
            0
        );

        player.playerHP = 5;
        bossHP = 10;

        UpdateHealthUI();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Vector2 movement = new Vector2(moveX, moveY) * Time.deltaTime * 100f;
        rb.velocity = movement;

        timer += Time.deltaTime;

        if (timer > TimeLimit)
        {
            SetReward(-0.5f);
            CheckEndCondition();
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
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+0.2f);
            ChangeFloorObjectsColor(Color.green);
            CheckEndCondition();
        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            bossHP--; // Boss perde 1 de HP ao colidir com uma parede
            ChangeFloorObjectsColor(Color.red); // A parede e o chão mudam de cor para vermelho
            UpdateHealthUI();
            SetReward(-0.1f); // Penalidade menor ao bater na parede

            if (bossHP <= 0)
            {
                Debug.Log("Boss defeated!");
                SetReward(-1.0f);
                CheckEndCondition();
            }
        }
        else if (other.gameObject.CompareTag("Sword"))
        {
            player.TakeDamage();
            UpdateHealthUI();
            SetReward(+0.2f);

            if (bossHP <= 0)
            {
                Debug.Log("Boss defeated!");
                SetReward(-1.0f);
                CheckEndCondition();
            }else if (player.playerHP <= 0)
            {
                SetReward(1.0f);
                CheckEndCondition();
            }
        }
        else if (other.gameObject.CompareTag("Beam"))
        {
            bossHP--;
            UpdateHealthUI();
            SetReward(-0.2f);

            if (bossHP <= 0)
            {
                Debug.Log("Boss defeated!");
                SetReward(-1.0f);
                CheckEndCondition();
            }else if (player.playerHP <= 0)
            {
                SetReward(1.0f);
                CheckEndCondition();
            }
        }
    }

    private void ChangeFloorObjectsColor(Color color)
    {
        foreach (GameObject floorObject in floorObjects)
        {
            if (floorObject != null)
            {
                SpriteRenderer spriteRenderer = floorObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = color;
                }
                else
                {
                    Debug.LogWarning("SpriteRenderer not found on " + floorObject.name);
                }
            }
        }
    }

    public void UpdateHealthUI()
    {
        if (bossHealthText != null)
        {
            bossHealthText.text = "Boss HP: " + bossHP;
        }

        if (playerHealthText != null)
        {
            playerHealthText.text = "Player HP: " + player.playerHP;
        }
    }

    public void CheckEndCondition()
    {
        if (bossHP <= 0 || player.playerHP <= 0)
        {
            EndEpisode();
        }
    }
}
