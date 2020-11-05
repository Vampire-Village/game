using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    // Start is called before the first frame update
    public void completeTask(GameObject taskCanvas)
    {
        GameObject.Find("Player").GetComponent<Controller>().endTask();
        Debug.Log("Task ended!");
        Destroy(taskCanvas, 2.0f);
    }
}
