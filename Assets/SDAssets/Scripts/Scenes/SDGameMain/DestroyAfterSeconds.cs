using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float secondsUntilSelfDestruct = 3.0f;

	void Start ()
    {
        StartCoroutine(SerlfDestructAfterSeconds());
	}

    private IEnumerator SerlfDestructAfterSeconds()
    {
        yield return new WaitForSeconds(secondsUntilSelfDestruct);

        Destroy(this.gameObject);
    }
}
