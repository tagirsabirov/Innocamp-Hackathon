using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Rigidbody rb;
	public float moveSpeed;
	private float xInput;
	private float zInput;
	public float jumpHeight;
	float jump;
	
	void awake()
	{
		rb = GetComponent<Rigidbody>();
	}
	void Start()
	{
		
	}	

	// Update is called once per frame
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
	}
	private void Move()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			rb.AddForce(new Vector3(0f, 25f, 0f));
		}
		rb.AddForce(new Vector3(xInput, 0f, zInput) * moveSpeed);




	}
}

	
