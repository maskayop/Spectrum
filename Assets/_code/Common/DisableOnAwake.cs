using UnityEngine;

public class DisableOnAwake : MonoBehaviour
{
	[SerializeField]
	bool disableOnAwake = true;

	void Awake()
	{
		gameObject.SetActive(!disableOnAwake);
	}
}
