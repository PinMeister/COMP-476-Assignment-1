using UnityEngine;
using System.Collections;

// accepts input and animates the player
public class Player : MonoBehaviour
{

	//the animator component attached to this game object
	Animator _animator;

	//veloctiy upper clamp
	public float m_maxVelocity;

	//acceleration in unity units per second
	public float m_acceleration;

	//deccelaration speed with respect to acceleration speed
	public float m_deccelerationMultiplier;

	//rotation speed
	public float rotationDegreesPerSecond = 360;

	// the current velocity
	float _velocity;

	// the input vector
	Vector2 _input = Vector2.zero;


    void Start ()
    {
		//cache the animator
		_animator = GetComponent<Animator> ();
    }

    void Update ()
    {

        // Obtain input information (See "Horizontal" and "Vertical" in the Input Manager)
        float horizontal = Input.GetAxis ("Horizontal");
        float vertical = Input.GetAxis ("Vertical");
        
		//cache the input
		_input.x = horizontal;
		_input.y = vertical;

		//calculate input magnitude (you may use this to assign the blend parameter in your movement
		// blend tree directly, the acceleration system in this example is given to showcase its
		// potential effect on a PC without a controller)
		float inputMag = _input.magnitude;


        // Check for inputs
		if (!Mathf.Approximately (vertical, 0.0f) || !Mathf.Approximately (horizontal, 0.0f)) {
			Vector3 direction = new Vector3 (horizontal, 0.0f, vertical);
			direction = Vector3.ClampMagnitude (direction, 1.0f);

			// increment velocity
			if (_velocity < m_maxVelocity) {
				_velocity += m_acceleration * Time.deltaTime;
				if (_velocity > m_maxVelocity)
					_velocity = m_maxVelocity;
			}

			// look towards the input direction
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (direction), rotationDegreesPerSecond * Time.deltaTime);


		} else if (_velocity > 0){
			
			//decrement velocity if there is no input
			_velocity -= m_acceleration * m_deccelerationMultiplier * Time.deltaTime;
			if (_velocity < 0)
				_velocity = 0;
		}

		// TODO: Translate the game object in world space
        if (transform.position.y < 0.1)
        {
            transform.position += transform.forward * Time.deltaTime * _velocity;
        }
        else
        {
            Vector3 newPos = new Vector3(transform.position.x, 0, transform.position.z);
            transform.position = newPos;
        }

		// set the blend parameter in your animator's movement blend tree
		_animator.SetFloat ("Blend", _velocity / m_maxVelocity);

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
    }
}
