using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public int playerHP = 5; // Player hit points
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze rotation to keep the player upright
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveDirection = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection = 1f;
        }

        Vector2 move = new Vector2(moveDirection * speed, rb.velocity.y);
        rb.velocity = move;
    }

    void Jump()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
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
        if (other.gameObject.CompareTag("Beam"))
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
        // Here you can handle the game over logic. For now, we just stop the game.
        // You can add more game over logic such as displaying a game over screen, restarting the level, etc.
        Time.timeScale = 0; // This stops the game by freezing all actions.
    }
}
