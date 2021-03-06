using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingController : MonoBehaviour
{
    public float duration;
    // Start is called before the first frame update
    void Start()
    {
        // start duration, then destroy
        StartCoroutine(WaitForDuration());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
