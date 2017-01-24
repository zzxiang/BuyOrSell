using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GJJ2017StockWave {

	public class TipsOnInteraction : MonoBehaviour {

		public float showTime = 2f;  // seconds

		Text text;
		float showTimeAccumulated = 0f;
		bool showForEver = false;

		public void Show (string message) {
			if (!text) {
				text = GetComponent<Text> ();
			}
			text.text = message;
			gameObject.SetActive (true);
			showTimeAccumulated = 0f;
		}

		public void ShowForEver (string message) {
			Show (message);
			showForEver = true;
		}
		
		// Update is called once per frame
		void Update () {
			if (showForEver) {
				return;
			}

			showTimeAccumulated += Time.deltaTime;
			if (showTimeAccumulated > showTime) {
				gameObject.SetActive (false);
			}
		}

		void OnDisable () {
			showForEver = false;
			showTimeAccumulated = 0f;
		}
	}
}
