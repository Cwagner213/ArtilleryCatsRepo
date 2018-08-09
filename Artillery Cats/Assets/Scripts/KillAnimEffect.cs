using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KillAnimEffect : MonoBehaviour {
    int timer = 0;
	// Update is called once per frame
	void Update () {
		if(timer == 0)
        {
            timer = 1;
            StartCoroutine(killAnim());
        }
	}
    public IEnumerator killAnim()
    {
        yield return new WaitForSeconds(1);
        GameObject.Destroy(gameObject);
    }
}
