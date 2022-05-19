using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballSpowner : MonoBehaviour
{
    [SerializeField] private GameObject snowBall;
    [SerializeField] private Transform spownPosition;
    [SerializeField] private Vector2 minMaxAngle;
	[SerializeField] private float spawnDelay = 1f;
	[HideInInspector] public bool IsSpowing = true;
    private IEnumerator repeatSpownCoroutine;
    void Start()
    {
        repeatSpownCoroutine = RepeatSpown();
        StartCoroutine(repeatSpownCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private Quaternion GetRandomAngle()
	{
        float angle = Random.Range(minMaxAngle.x, minMaxAngle.y);
        return Quaternion.Euler(0, angle, 0);
	}
    private void Spown()
	{
        Instantiate(snowBall, spownPosition.position, GetRandomAngle());
	}
    IEnumerator RepeatSpown()
	{
        while (IsSpowing)
        {
            yield return new WaitForSeconds(spawnDelay);
            Spown();
        }
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward, new Vector3(1,1,1));
	}
}
