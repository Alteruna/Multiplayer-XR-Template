using UnityEngine;
using UnityEngine.InputSystem;

namespace Alteruna
{
	public class SimpleXRMovement : MonoBehaviour
	{
		
		public Transform RelativeTo;

		public float MoveSpeed = 1;
		public float rotateSpeed = 129;
		
		public InputActionReference movement;
		public InputActionReference Rotate;
		
		Vector2 _movementInput;

		void Start()
		{
			movement.action.Enable();
			Rotate.action.Enable();
		}
		
		private void Update()
		{
			_movementInput = movement.action.ReadValue<Vector2>();
			Vector3 translation = RelativeTo.TransformDirection(_movementInput.x, 0, _movementInput.y);
			translation = new Vector3(translation.x, 0, translation.z).normalized * (MoveSpeed * Time.deltaTime);
			transform.Translate(translation);
			
			transform.RotateAround(RelativeTo.position, Vector3.up, Rotate.action.ReadValue<Vector2>().x * rotateSpeed * Time.deltaTime);
		}
	}
}