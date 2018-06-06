using UnityEngine;

namespace Bolt.DanKVK
{

	public class PlayerCamera : BoltSingletonPrefab<PlayerCamera>
	{
		Vector3 _velocity;
		Transform _target;
		bool _aiming = false;
		float _distance = 0f;
		float _aimingAcc = 0f;

		[SerializeField]
		Transform cam;

		[SerializeField]
		float height = 2.3f;

		[SerializeField]
		float offset = 0.75f;

		[SerializeField]
		float aimingDistance = 1f;

		[SerializeField]
		float runningDistance = 3f;

		[SerializeField]
		float runningSmoothTime = 0.1f;

		[SerializeField]
		Transform dummyRig;

		[SerializeField]
		Transform dummyTarget;

		public Camera myCamera {
			get { return cam.GetComponent<Camera> (); }
		}

		public System.Func<int> getHealth;
		public System.Func<bool> getAiming;
		public System.Func<float> getPitch;

		void Awake ()
		{
			DontDestroyOnLoad (gameObject);
			_distance = runningDistance;
		}

		void LateUpdate ()
		{
			UpdateCamera (true);
		}

		void UpdateCamera (bool allowSmoothing)
		{
			if (_target) {
				//var h = getHealth != null ? getHealth () : 100;
				var a = getAiming != null ? getAiming () : false;
				var p = getPitch != null ? getPitch () : 0f;

				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;

				if (_aiming) {
					if (a == false) {
						_aiming = false;
						_aimingAcc = 0f;
					}
				} else {
					if (a) {
						_aiming = true;
						_aimingAcc = 0f;
					}
				}

				_aimingAcc += Time.deltaTime;

				if (_aiming) {
					_distance = Mathf.Lerp (_distance, aimingDistance, _aimingAcc / 0.4f);
				} else {
					_distance = Mathf.Lerp (_distance, runningDistance, _aimingAcc / 0.4f);
				}

				Vector3 pos;
				Quaternion rot;

				CalculateCameraTransform (_target, p, _distance, out pos, out rot);

				if (!_aiming || allowSmoothing) {
					pos = Vector3.SmoothDamp (transform.position, pos, ref _velocity, runningSmoothTime);
				}

				transform.position = pos;
				transform.rotation = rot;

				cam.transform.localRotation = Quaternion.identity;
				cam.transform.localPosition = Vector3.zero;
			}
		}

		public void SetTarget (BoltEntity entity)
		{
			_target = entity.transform;
			UpdateCamera (false);
		}

		public void CalculateCameraAimTransform (Transform target, float pitch, out Vector3 pos, out Quaternion rot)
		{
			CalculateCameraTransform (target, pitch, aimingDistance, out pos, out rot);
		}

		public void CalculateCameraTransform (Transform target, float pitch, float distance, out Vector3 pos, out Quaternion rot)
		{
			dummyTarget.position = target.position;
			dummyTarget.rotation = target.rotation;
			dummyTarget.position += new Vector3 (0, height, 0);
			dummyTarget.position += dummyTarget.right * offset;
			Quaternion pitchRotation = Quaternion.Euler (pitch, 0, 0);

			pos = dummyTarget.position;
			pos += (-dummyTarget.forward * distance);

			pos = dummyTarget.InverseTransformPoint (pos);
			pos = pitchRotation * pos;
			pos = dummyTarget.TransformPoint (pos);
			dummyRig.position = pos;
			dummyRig.LookAt (dummyTarget.position);

			rot = dummyRig.rotation;
		}
	}

}