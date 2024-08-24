using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    public GameObject bulletPrefab;
    public Transform firePoint; // Transform para definir o ponto de disparo local ao player
    public float moveSpeed = 5f;
    public float bulletSpeed = 10f;
    public Transform boss;
    public List<GameObject> walls; // Lista das paredes
    public int playerHP = 5; // Vida do player
    public float fireRate = 1f; // Taxa de disparo (1 tiro por segundo)

    private Rigidbody2D rb;
    private float lastFireTime; // Tempo do último disparo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastFireTime = -fireRate; // Garantir que o player possa atirar imediatamente no início
    }

    public override void OnEpisodeBegin()
    {
        // Resetar posição do player e do boss ao início de cada episódio
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        boss.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);

        // Resetar a vida do player
        playerHP = 5;

        // Resetar o tempo do último disparo
        lastFireTime = -fireRate;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Adicionando observações ao sensor
        sensor.AddObservation(transform.localPosition); // 2 observações (x, y)
        sensor.AddObservation(boss.localPosition); // 2 observações (x, y)

        foreach (var wall in walls)
        {
            sensor.AddObservation(wall.transform.localPosition); // 2 observações por parede (x, y)
        }

        sensor.AddObservation(playerHP); // 1 observação
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Ações de movimento
        float moveX = actionBuffers.ContinuousActions[0];
        float moveY = actionBuffers.ContinuousActions[1];

        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;
        rb.velocity = movement; // Usar velocity para movimento contínuo

        // Disparo automático
        if (Time.time > lastFireTime + fireRate)
        {
            Shoot();
            lastFireTime = Time.time; // Atualizar o tempo do último disparo
        }

        // Penalizar o player por colidir com as paredes
        foreach (var wall in walls)
        {
            if (GetComponent<Collider2D>().IsTouching(wall.GetComponent<Collider2D>()))
            {
                AddReward(-0.1f);
                TakeDamage(1); // Player perde vida ao colidir com a parede
            }
        }

        // Penalizar o player por colidir com o boss
        if (GetComponent<Collider2D>().IsTouching(boss.GetComponent<Collider2D>()))
        {
            AddReward(-0.5f);
            TakeDamage(2); // Player perde mais vida ao colidir com o boss
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Controle manual para testes
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private void Shoot()
    {
        // Instanciar a bala na posição e rotação locais do player
        GameObject bullet = Instantiate(bulletPrefab, transform.localPosition, transform.localRotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        // Definir a velocidade da bala na direção em que o player está apontando
        rbBullet.velocity = transform.right * bulletSpeed;

        // Destruir a bala após 2,5 segundos
        Destroy(bullet, 2.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Recompensar o player ao acertar o boss com um tiro
        if (collision.gameObject.CompareTag("Boss"))
        {
            AddReward(1.0f);
        }
    }

    // Método para reduzir a vida do player
    public void TakeDamage(int damage)
    {
        playerHP -= damage;

        // Checar se o player foi derrotado
        if (playerHP <= 0)
        {
            // Penalizar fortemente e finalizar o episódio
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}
