using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BossAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject[] floorObjects; // Altere para um array de GameObjects
    [SerializeField] private Player player; // Adicione uma referência ao script Player
    private Rigidbody2D rb;

    private float timer;
    private const float TimeLimit = 20f; // Limite de tempo de 20 segundos

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        // Resetar a posição do boss
        transform.localPosition = Vector3.zero;
        rb.velocity = Vector2.zero; // Resetar a velocidade do Rigidbody2D
        timer = 0f; // Resetar o timer no início do episódio

        // Reposicionar o player em uma posição aleatória dentro do intervalo
        player.transform.localPosition = new Vector3(
            Random.Range(-7.96f, -2.396f),
            Random.Range(3.75f, -1.45f),
            0
        );
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

        Vector2 movement = new Vector2(moveX, moveY) * Time.deltaTime * 100f; // Movimento em 2D
        rb.velocity = movement; // Aplicar movimento ao Rigidbody2D

        // Atualizar o timer
        timer += Time.deltaTime;

        // Penalidade se o tempo limite for alcançado sem acertar o player
        if (timer > TimeLimit)
        {
            SetReward(-0.5f);
            EndEpisode();
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
            SetReward(+1f);
            ChangeFloorObjectsColor(Color.green); // Altere a cor para verde
            EndEpisode(); // Finaliza o episódio apenas para este agente
        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-0.1f); // Penalidade menor ao bater na parede
            ChangeFloorObjectsColor(Color.red); // Altere a cor para vermelho
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
}
