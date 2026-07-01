using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackUIMovement : MonoBehaviour
{
    public float moveSpeed = .6f;

    private void Update()
    {
        this.transform.position = this.transform.position + Vector3.up * this.moveSpeed * Time.deltaTime;
    }
}
