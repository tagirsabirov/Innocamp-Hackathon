using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Rigidbody rb;
	public float moveSpeed;
	private float xInput;
	private float zInput;
	float jump;
	
	void awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		ProcessInputs();
	}
	
	private void FixedUpdate()
	{
		Move();
	}

	private void ProcessInputs()
	{
		xInput = Input.GetAxis("Horizontal");
		zInput = Input.GetAxis("Vertical");
		if (Input.GetKey(KeyCode.Space) && GetComponent<Rigidbody>().transform.position.y <= 0.4f)
			jump = 10f;
		else
			jump = 0f;
	}
	private void Move()
	{
		rb.AddForce(new Vector3(xInput, jump, zInput) * moveSpeed);
	}
}

	
