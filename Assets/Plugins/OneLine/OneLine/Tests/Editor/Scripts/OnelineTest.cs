using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OneLine;

public class OnelineTest : ScriptableObject {

	[SerializeField]
	private NestedClass root;

	[Serializable]
	public abstract class BaseClass {
		[OneLine]
		public List<Vector2> list;
	}

	[Serializable]
	public class NestedClass : BaseClass {
	}
}
