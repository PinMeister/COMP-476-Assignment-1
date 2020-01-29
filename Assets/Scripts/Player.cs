using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    Animator animator;
    
    public Vector3 destination;
    public float distance;
    public float speed;
    public float maxSpeed;
    public float stopRadius;
    public float slowRadius;
    public float rotationDegreesPerSecond;

    public bool arrive;
    public bool flee;

    void Start()
    {
		animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat ("Blend", speed / maxSpeed);

        //Appear on other side of arena when crossing a boundary
        if (transform.position.z > 50)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -50);
        }
        if (transform.position.z < -50)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 50);
        }
        if (transform.position.x > 50)
        {
            transform.position = new Vector3(-50, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -50)
        {
            transform.position = new Vector3(50, transform.position.y, transform.position.z);
        }

        // Set target and change character colour based on current tag
        if (this.tag == "Tagged")
        {
            Target("Not Tagged");
            maxSpeed = 3;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else if (this.tag == "Frozen")
        {
            speed = 0;
            arrive = false;
            flee = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            Target("Frozen");
            maxSpeed = 1.5f;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }

        if (arrive)
        {
            Vector3 Direction = destination - this.transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, Direction);
            float CurrentRotation = Vector3.Dot(perp, Vector3.up);
            Kinematic(Direction, CurrentRotation);
            speed = Mathf.Clamp(speed, 0, maxSpeed);
            this.transform.Translate(Vector3.forward * Time.deltaTime * 0.5f * 1.75f * speed);
        }

        if (flee)
        {
            Vector3 direction = transform.position - destination;
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationDegreesPerSecond * Time.deltaTime);

            Vector3 perp = Vector3.Cross(transform.forward, direction);
            float CurrentRotation = Vector3.Dot(perp, Vector3.up);
            Kinematic(direction, CurrentRotation);
            this.transform.Translate(Vector3.forward * Time.deltaTime * 0.5f * 1.105f * speed);
        }
    }

    protected void Kinematic(Vector3 Direction, float CurrentRotation)
    {
        if (speed < maxSpeed)
        {
            if (distance <= stopRadius)
            {
                this.transform.Rotate(Vector3.up, CurrentRotation);
                speed = maxSpeed;
            }
            else if (distance < slowRadius)
            {
                speed = 0;
                if (CurrentRotation > 1)
                    this.transform.Rotate(Vector3.up, 1);
                else if (CurrentRotation < -1)
                    this.transform.Rotate(Vector3.up, -1);
                else
                    speed = maxSpeed;
            }
        }
        else
        {
            if (Vector3.Angle(Direction, transform.forward) <= 30)
            {
                if (CurrentRotation > 0)
                    this.transform.Rotate(Vector3.up, 1);
                else if (CurrentRotation < 0)
                    this.transform.Rotate(Vector3.up, -1);
            }
            else
            {
                speed = 0f;
            }
        }
    }

    // Find target depending on tag
    void Target(string target)
    {
        // Use kinematic arrive for chosen target with appropriate tag
        foreach (GameObject opponent in GameObject.FindGameObjectsWithTag(target))
        {
            if (Vector3.Distance(transform.position, opponent.transform.position) < slowRadius)
            {
                distance = Vector3.Distance(transform.position, opponent.transform.position);
                destination = opponent.transform.position;
                arrive = true;
                flee = false;
            }
        }

        // Flee from tagged character if they are close enough
        if (this.tag == "Not Tagged")
        {
            GameObject taggedOpponent = GameObject.FindGameObjectWithTag("Tagged");
            if (Vector3.Distance(transform.position, taggedOpponent.transform.position) < slowRadius)
            {
                distance = Vector3.Distance(transform.position, taggedOpponent.transform.position);
                destination = taggedOpponent.transform.position;
                flee = true;
                arrive = false;
            }
        }
    }

    // Change tags on collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Not Tagged" && this.tag == "Tagged")
        {
            Debug.Log("frozen!");
            collision.gameObject.tag = "Frozen";
        }
        if (collision.gameObject.tag == "Frozen" && this.tag == "Not Tagged")
        {
            Debug.Log("saved!");
            collision.gameObject.tag = "Not Tagged";
        }
    }
}
