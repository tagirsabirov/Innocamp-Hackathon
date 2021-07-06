using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Rigidbody rb;
	public float moveSpeed = 50f;
	private float xInput;
	private float zInput;
	
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
		rb.AddForce(new Vector3(xInput, 0f, zInput));
	}
}

	
