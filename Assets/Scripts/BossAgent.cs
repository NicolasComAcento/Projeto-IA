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
    float frontalBeamPositionChange = actionBuffers.ContinuousActions[0];
    float skyBeamPositionChange = actionBuffers.ContinuousActions[1];

  boss.frontalBeamTargetPosition += new Vector2(frontalBeamPositionChange, 0);
    boss.skyBeamTargetPosition += new Vector2(skyBeamPositionChange, 0);

    // Garantir que a nova posição esteja dentro dos limites aceitáveis
    boss.frontalBeamTargetPosition = new Vector2(Mathf.Clamp(boss.frontalBeamTargetPosition.x, -9f, -1f), boss.frontalBeamTargetPosition.y);
    boss.skyBeamTargetPosition = new Vector2(Mathf.Clamp(boss.skyBeamTargetPosition.x, -7f, -1f), boss.skyBeamTargetPosition.y);
  }

  public override void Heuristic(in ActionBuffers actionsOut)
  {
    var continuousActionsOut = actionsOut.ContinuousActions;
    continuousActionsOut[0] = 0; // Sem mudanças por padrão
    continuousActionsOut[1] = 0; // Sem mudanças por padrão
  }

  public override void OnEpisodeBegin()
  {
    player.playerHP = 5;
    player.transform.localPosition = new Vector3(0, 0.5f, -5);
  }

}