using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Player : Agent
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public int playerHP = 5; // Player hit points
    public GameObject attackPrefab; // Prefab do ataque
    public Transform attackPoint; // Ponto de origem do ataque
    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 initialPosition;
    private int initialPlayerHP;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze rotation to keep the player upright
        initialPosition = transform.localPosition;
        initialPlayerHP = playerHP;
    }

    public override void OnEpisodeBegin()
    {
        // Reset player position and health
        transform.localPosition = initialPosition;
        playerHP = initialPlayerHP;
        rb.velocity = Vector2.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(isGrounded);
        sensor.AddObservation(playerHP);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 3
        float move = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        float jump = Mathf.Clamp(actionBuffers.ContinuousActions[1], 0f, 1f);
        float attack = Mathf.Clamp(actionBuffers.ContinuousActions[2], 0f, 1f);

        Move(move);
        if (jump > 0.5f && isGrounded)
        {
            Jump();
        }
        if (attack > 0.5f)
        {
            Attack();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
{
    var continuousActionsOut = actionsOut.ContinuousActions;
    // Geração de ações aleatórias
    continuousActionsOut[0] = Random.Range(-1f, 1f); // Movimento para frente e para trás
    continuousActionsOut[1] = Random.value > 0.8f ? 1f : 0f;  // Salto (20% de chance de pular)
    continuousActionsOut[2] = Random.value > 0.7f ? 1f : 0f;  // Ataque (30% de chance de atacar)
}

    void Move(float direction)
    {
        Vector2 move = new Vector2(direction * speed, rb.velocity.y);
        rb.velocity = move;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    void Attack()
    {
        // Instantiate attack prefab at attack point
        if (attackPrefab != null && attackPoint != null)
        {
            Instantiate(attackPrefab, attackPoint.position, attackPoint.rotation);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Beam") || other.gameObject.CompareTag("Boss"))
        {
            playerHP--;
            Debug.Log("Player hit! Current HP: " + playerHP);

            if (playerHP <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        EndEpisode();
    }
}
