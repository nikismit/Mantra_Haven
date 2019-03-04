using UnityEngine;

namespace SoundInput
{
	public class SoundInputObject : MonoBehaviour 
	{
		protected virtual void Start()
		{
			SoundInputController.instance.callback += ProccesSoundInput;
		}

		protected virtual void OnEnable()
		{
			//print(this.gameObject.name);
			SoundInputController.instance.callback += ProccesSoundInput;
		}

		protected virtual void OnDisable()
		{
			SoundInputController.instance.callback -= ProccesSoundInput;
		}

		protected virtual void ProccesSoundInput(InputData data)
		{
			Debug.LogWarning("No implemention for ProccesSoundInput in Object: " + gameObject.name);
		}
	}
}