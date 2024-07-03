using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject frontalBeamPrefab;
    public GameObject skyBeamPrefab;
    public float attackInterval = 5f; // Time between attacks
    public float beamDuration = 2f; // Time the beams stay active
    public float skyBeamStartHeight = 10f; // Starting height of the sky beam
    public float skyBeamSpeed = 5f; // Speed at which the sky beam descends
    public Vector2 skyBeamSpawnRange = new Vector2(-7f, -1f); // Range for sky beam spawn x position
    public float frontalBeamSpeed = 5f; // Speed at which the frontal beam moves forward
    public int bossHP = 20; // Boss health points

    private void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Sword"))
        {
            bossHP--;
            Debug.Log("Boss hit! Current HP: " + bossHP);

            if (bossHP <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            int attackChoice = Random.Range(0, 2); // 0 for frontal beam, 1 for sky beam

            if (attackChoice == 0)
            {
                StartCoroutine(FrontalBeamAttack());
            }
            else
            {
                StartCoroutine(SkyBeamAttack());
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    private IEnumerator FrontalBeamAttack()
    {
        Vector3 startPosition = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 2);
        GameObject frontalBeam = Instantiate(frontalBeamPrefab, startPosition, Quaternion.identity);
        // Adjust the position and rotation as needed
        frontalBeam.transform.parent = transform; // Optional: parent the beam to the boss
        
        Vector3 endPosition = new Vector3(transform.position.x - 9, transform.position.y, transform.position.z + 2);

        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (Time.time - startTime < beamDuration)
        {
            float distCovered = (Time.time - startTime) * frontalBeamSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            frontalBeam.transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
            yield return null;
        }

        Destroy(frontalBeam);
    }

    private IEnumerator SkyBeamAttack()
    {
        float randomX = Random.Range(skyBeamSpawnRange.x, skyBeamSpawnRange.y);
        Vector3 startPosition = new Vector3(transform.position.x + randomX, transform.position.y + skyBeamStartHeight, transform.position.z + 2);
        GameObject skyBeam = Instantiate(skyBeamPrefab, startPosition, Quaternion.identity);
        // Adjust the position and rotation as needed
        skyBeam.transform.parent = transform; // Optional: parent the beam to the boss

        float elapsedTime = 0f;
        Vector3 endPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + 2);

        while (elapsedTime < beamDuration)
        {
            skyBeam.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / beamDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(skyBeam);
    }
}
