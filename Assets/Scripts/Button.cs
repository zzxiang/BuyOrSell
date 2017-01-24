using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GJJ2017StockWave {

	public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		public GameObject normalText;
		public GameObject pressedText;

		public void OnPointerDown (PointerEventData eventData) {
			normalText.SetActive (false);
			pressedText.SetActive (true);
		}

		public void OnPointerUp (PointerEventData eventData) {
			normalText.SetActive (true);
			pressedText.SetActive (false);
		}
	}
}