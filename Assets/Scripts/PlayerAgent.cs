using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;

public class PlayerAgent : Agent
{
    // Esse é o script do playeragent usado para o treino competitivo
    //ele funciona como o bossagent só que para o player, com as mudanças
    //da movimentação e forma como o player ataca
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
    //mesma coisa do boss ele coleta informações da propria posição e do boss
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(bossTransform.localPosition);
    }

    // duas ações uma para o eixo x e outra para o eixo y para evitar que o player fique parado
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

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    //recompensas e colisões para o player
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
        // penaliza o player quando ele encosta nas paredes pelo mesmo motivo do boss
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
