using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJJ2017StockWave {

	public class BackgroundManager : MonoBehaviour {

		public static BackgroundManager instance;

		// Use this for initialization
		void Start () {
			instance = this;
		}
		
		public void Move(float x, float y) {
		}
	}
}