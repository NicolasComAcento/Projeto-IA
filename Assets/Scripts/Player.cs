using UnityEngine;

public class Player : MonoBehaviour
{
    public void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-7.96f, -2.396f), Random.Range(3.75f, -1.45f), 0);
    }
}
