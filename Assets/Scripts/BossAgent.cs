using TMPro;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BossAgent : Agent
{
    [SerializeField] private Transform targetTransform; // Referência ao player
    [SerializeField] private GameObject[] floorObjects; // Objetos do chão
    [SerializeField] private PlayerAgent playerAgent; // Referência ao PlayerAgent
    [SerializeField] private TextMeshProUGUI bossHealthText; // UI de vida do boss
    [SerializeField] private TextMeshProUGUI playerHealthText; // UI de vida do player

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

        // Posicionar o player em uma posição inicial aleatória
        playerAgent.transform.localPosition = new Vector3(
            Random.Range(-7.96f, -2.396f),
            Random.Range(3.75f, -1.45f),
            0
        );

        // Resetar a vida do player e do boss
        playerAgent.playerHP = 5;
        bossHP = 10;

        UpdateHealthUI();
    }

    public override void CollectObservations(VectorSensor sensor)
{
    // O número de observações aqui deve ser consistente com o esperado.
    // Supondo que o sensor espere 4 observações: 2 para a posição do boss e 2 para a posição do player.

    sensor.AddObservation(transform.localPosition); // 2 observações (x, y)
    sensor.AddObservation(targetTransform.localPosition); // 2 observações (x, y)
}


    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Vector2 movement = new Vector2(moveX, moveY) * Time.deltaTime * 100f;
        rb.velocity = movement;

        timer += Time.deltaTime;

        // Penalidade por tempo limite
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
            SetReward(-0.4f); // Penalidade menor ao bater na parede

            if (bossHP <= 0)
            {
                Debug.Log("Boss defeated!");
                SetReward(-1.0f);
                CheckEndCondition();
            }
        }
        else if (other.gameObject.CompareTag("Player")) // Colisão com o player
        {
            playerAgent.TakeDamage(2); // Causa 2 de dano ao player
            UpdateHealthUI();
            SetReward(+0.5f);

            if (playerAgent.playerHP <= 0)
            {
                SetReward(2.0f);
                CheckEndCondition();
            }
        }
        else if (other.gameObject.CompareTag("Beam")) // Colisão com o feixe
        {
            bossHP--; // Boss perde 1 de HP ao ser atingido
            UpdateHealthUI();
            SetReward(-0.2f);
            Debug.Log("Hit by beam!");

            if (bossHP <= 0)
            {
                Debug.Log("Boss defeated!");
                SetReward(-1.0f);
                CheckEndCondition();
            }
        }
    }

    public void CheckEndCondition()
    {
        if (playerAgent.playerHP <= 0 || bossHP <= 0)
        {
            EndEpisode();
        }
    }

    private void ChangeFloorObjectsColor(Color color)
    {
        foreach (var obj in floorObjects)
        {
            obj.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void UpdateHealthUI()
    {
        bossHealthText.text = $"Boss HP: {bossHP}";
        playerHealthText.text = $"Player HP: {playerAgent.playerHP}";
    }
}


