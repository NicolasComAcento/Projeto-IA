using TMPro;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BossAgent : Agent
{
     // os SerializeField são informações que o boss recebe no editor
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

    // aqui temos o começo do episodio, ele é um override para que ele faça um "reset" em tudo que tinha antes
    // sobreescrendo com as informações para um novo ep.
    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        rb.velocity = Vector2.zero;
        timer = 0f;

        // reseta a posição do player nesse range, para ter uma variação e o boss não só "decorar" onde está o player
        player.transform.localPosition = new Vector3(
            Random.Range(-7.96f, -2.396f),
            Random.Range(3.75f, -1.45f),
            0
        );

        player.playerHP = 5;
        bossHP = 10;

        UpdateHealthUI();
    }
    // Sensores utilizados para coletar as informações, basicamente para o boss é um sensor para a sua propria posição e outro para o player
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    // aqui definimos a quantidade de ações continuas, no caso do boss ele precisa 
    // de duas delas uma para se mexer no eixo X e outra no Y, também colocamos um timer
    // para caos ele fique parado ele comece a ser penalizado.
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

    // aqui a função heuristic funciona para testes fora do treino
    // quando iamos testar se alguma mudança estava funcionando mas
    // não queriamos iniciar um novo treino, então é como um teste manual
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }


    //Parte de como as recompensas foram definidas
    private void OnTriggerEnter2D(Collider2D other)
    {
        //no começo chegar ao Goal (Player) dava uma recompensa para o boss
        // depois definimos na outra função que é quando o boss encosta na "sword"
        // para ter um hitbox melhor, então a função do goal ficou apenas para mudar a cor
        // da cena em caso de acertos e erros
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            // SetReward(+0.5f);
            ChangeFloorObjectsColor(Color.green);
            CheckEndCondition();
        }
        //boss perde vida ao bater na parede e toma penalidade, a parede em si não
        // possui fisica para parar o movimento do boss, isso é intencional, para
        // que o boss pare de encostar na parede por que ele aprendeu que é ruim
        // se colocassemos uma fisica lá ele ia parar de encostar pois não conseguia
        // passar, o que não era a ideia do treino
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            bossHP--; // Boss perde 1 de HP ao colidir com uma parede
            ChangeFloorObjectsColor(Color.red); 
            UpdateHealthUI();
            SetReward(-0.4f); 

            if (bossHP <= 0)
            {
                Debug.Log("Boss defeated!");
                SetReward(-1.0f);
                CheckEndCondition();
            }
        }

        //recompensa caso o boss acerte o player o verificação caso ele morra ou mate o player com uma recompensa ou punição maior
        else if (other.gameObject.CompareTag("Sword"))
        {
            player.TakeDamage();
            UpdateHealthUI();
            SetReward(+0.3f);

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
        // é o tiro do player que quando o boss nao desvia ele recebe uma punição e perde vida
        }
        else if (other.gameObject.CompareTag("Beam"))
        {
            bossHP--;
            UpdateHealthUI();
            SetReward(-0.3f);

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

    //mudança de coor do chão para visualização
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
            }
        }
    }
    //verificação de vida na ui
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
