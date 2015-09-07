using UnityEngine;
using System.Collections;

namespace Platformer
{
	public class CameraFollow : MonoBehaviour
	{
		[Tooltip("GameObject to follow")]
		[SerializeField]
		private GameObject
			target;

		void Awake ()
		{
		}

		void Update ()
		{
			transform.position = new Vector3 (target.transform.position.x, target.transform.position.y, -10.0f);
		}
	}
}