using UnityEngine;
using System;

namespace UnitySensors.Attribute
{
	public class InterfaceAttribute : PropertyAttribute
	{
		public Type type;

		public InterfaceAttribute(Type type)
		{
			this.type = type;
		}
	}
}