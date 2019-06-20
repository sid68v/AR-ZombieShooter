using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour, IDragHandler
{

    public GameObject ball;
    public GameObject target;
    public Vector2 xLimits, yLimits, zLimits;
    public float power = 10f;
    public float focus = 10f;
    public float yControlFactor = .02f;
    public float xControlFactor = 10f;
    Rigidbody rb;
    float angle = 0f;
    bool isMoving;

    Vector3 forceVector = Vector3.one;


    // Start is called before the first frame update
    void Start()
    {

        rb = ball.GetComponent<Rigidbody>();
    }


    public void OnDrag(PointerEventData eventData)
    {

        Vector3 deltaPos = eventData.position - eventData.pressPosition;


        float strength = Mathf.Clamp(deltaPos.y, 0, 1000);

        float spin = Mathf.Clamp(deltaPos.x, -1000, 1000);

        forceVector = new Vector3(spin * xControlFactor, strength * yControlFactor, strength * power);

        rb.AddForceAtPosition(forceVector, ball.transform.GetChild(0).position, ForceMode.Impulse);

        Vector3 changedForceVector = target.transform.position - ball.transform.position;

        rb.AddForceAtPosition(changedForceVector, ball.transform.GetChild(0).position, ForceMode.Acceleration);

        Debug.Log("Force :" + forceVector);

        //rb.AddForce(Mathf.Clamp01(forceVector.magnitude) * forceVector* controlFactor + ball.transform.forward * power,ForceMode.Force);
        //Debug.Log("Force :" + Mathf.Clamp01(forceVector.magnitude) * forceVector * controlFactor + ball.transform.forward * power);


    }


    // Update is called once per frame
    void Update()
    {
        ball.transform.position = new Vector3(
            Mathf.Clamp(ball.transform.position.x, xLimits.x, xLimits.y),
            Mathf.Clamp(ball.transform.position.y, yLimits.x, yLimits.y),
            Mathf.Clamp(ball.transform.position.z, zLimits.x, zLimits.y));
        if (ball.transform.position.z == zLimits.x)
            rb.velocity = Vector3.down;

        if (rb.velocity.z > 0)
        {
            
            Vector3 changedForceVector = target.transform.position - ball.transform.position;
            rb.AddForceAtPosition(changedForceVector*focus, ball.transform.GetChild(0).position, ForceMode.Acceleration);

        }
    }
}
