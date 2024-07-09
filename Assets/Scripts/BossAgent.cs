using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BossAgent : Agent
{
  private Rigidbody2D bossRb;
  private Boss boss;
  private Player player;
  private float initialAttackInterval;
  private int initialPlayerHP;

  public override void Initialize()
  {
    boss = GetComponent<Boss>();
    player = FindObjectOfType<Player>();
    initialAttackInterval = boss.attackInterval;
    initialPlayerHP = player.playerHP;
  }

  // variáveis que vão ser usadas
  public override void CollectObservations(VectorSensor sensor)
  {
    // ? Boss
    sensor.AddObservation(boss.attackInterval);
    sensor.AddObservation(boss.bossHP);

    // ? Player
    sensor.AddObservation(player.playerHP);
    sensor.AddObservation(player.transform.localPosition);
  }

  public override void OnActionReceived(ActionBuffers actionBuffers)
  {
    float attackIntervalChange = actionBuffers.ContinuousActions[0];
    boss.attackInterval = Mathf.Clamp(initialAttackInterval + attackIntervalChange, 1f, 10f);
  }

  public override void Heuristic(in ActionBuffers actionsOut)
  {
    var continuousActionsOut = actionsOut.ContinuousActions;
    continuousActionsOut[0] = 0; // Sem mudanças por padrão
  }

  public override void OnEpisodeBegin()
  {
    boss.attackInterval = initialAttackInterval;
    player.playerHP = initialPlayerHP;
    player.transform.localPosition = new Vector3(0, 0.5f, -5);
  }

}