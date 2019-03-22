using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CM/Essentials/FloatVariable")]
public class Variable<T> : ScriptableObject, ISerializationCallbackReceiver
{
	public T initialValue;

	[NonSerialized]
	public T value;

	public void OnAfterDeserialize()
	{
		value = initialValue;
	}

	public void OnBeforeSerialize()
	{
		
	}
}