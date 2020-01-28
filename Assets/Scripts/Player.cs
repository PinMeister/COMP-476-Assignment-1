using UnityEngine;
using System.Collections;

// implement AI movement and animate NPCs
public class Player : MonoBehaviour
{
    Animator animator;

	public float maxVelocity;
    public float acceleration;
	public float deccelerationMultiplier;
	public float rotationDegreesPerSecond = 360;
	float velocity;

	Vector2 input = Vector2.zero;
    public bool tagged = false;
    public bool target = false;

    void Start()
    {
		animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis ("Horizontal");
        float vertical = Input.GetAxis ("Vertical");
        
		input.x = horizontal;
		input.y = vertical;

		float inputMag = input.magnitude;

		if (!Mathf.Approximately (vertical, 0.0f) || !Mathf.Approximately (horizontal, 0.0f)) {
			Vector3 direction = new Vector3 (horizontal, 0.0f, vertical);
			direction = Vector3.ClampMagnitude (direction, 1.0f);

			if (velocity < maxVelocity) {
				velocity += acceleration * Time.deltaTime;
				if (velocity > maxVelocity)
					velocity = maxVelocity;
			}

			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (direction), rotationDegreesPerSecond * Time.deltaTime);
            
		} else if (velocity > 0){
			
			velocity -= acceleration * deccelerationMultiplier * Time.deltaTime;
			if (velocity < 0)
				velocity = 0;
		}

        if (transform.position.y < 0.1)
        {
            transform.position += transform.forward * Time.deltaTime * velocity;
        }
        else
        {
            Vector3 newPos = new Vector3(transform.position.x, 0, transform.position.z);
            transform.position = newPos;
        }

		animator.SetFloat ("Blend", velocity / maxVelocity);

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

        if (tagged)
        {
            this.tag = "Tagged";
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            this.tag = "Not Tagged";
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            velocity = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Not Tagged")
        {
            tagged = false;
            other.GetComponent<Player>().tagged = true;
        }
    }
}
