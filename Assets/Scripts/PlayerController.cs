using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    // Limites de movimentação
    public float minX = -8f;
    public float maxX = 1.4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze rotation to keep the player upright
        initialPosition = transform.localPosition;
        initialPlayerHP = playerHP;
        StartCoroutine(RandomMovement());
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        if(isGrounded){
            Move(move);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    IEnumerator RandomMovement()
    {
        while (true)
        {
            float randomMove = Random.Range(-1f, 1f); // Movimento aleatório
            float randomJump = Random.value > 0.8f ? 1f : 0f; // Salto (20% de chance de pular)
            float randomAttack = Random.value > 0.7f ? 1f : 0f; // Ataque (30% de chance de atacar)

            Move(randomMove);

            if (randomJump > 0.5f && isGrounded)
            {
                Jump();
            }

            if (randomAttack > 0.5f)
            {
                Attack();
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void Move(float direction)
    {
        Vector2 move = new Vector2(direction * speed, rb.velocity.y);
        rb.velocity = move;

        // Limitar a posição do jogador nos eixos X e Y
        Vector2 clampedPosition = transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        transform.localPosition = clampedPosition;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    void Attack()
    {
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
        transform.localPosition = initialPosition;
        playerHP = initialPlayerHP;
        rb.velocity = Vector2.zero;
    }
}
