using UnityEngine;

namespace Spectrum
{
	public class App : MonoBehaviour
	{
		public static App Instance;

		[SerializeField] bool useTargetFPS = true;

		private void Awake()
		{
			if (Instance != null)
			{
				Debug.LogWarning("Cannot create App");
				Destroy(gameObject);
				return;
			}

			Instance = this;

			SetTargetFPS(useTargetFPS);
		}

		public void ExitGame()
		{
			Application.Quit();
		}

		public void SetTargetFPS(bool value)
		{
			if (value)
				Application.targetFrameRate = 60;
			else
				Application.targetFrameRate = 30;
		}
	}
}
