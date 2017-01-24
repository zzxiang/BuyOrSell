using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TestSimpleRNG;

namespace GJJ2017StockWave {
	
	public class StockEventSystem : MonoBehaviour {

		public float regularEventForcastAfter = 5f;  // seconds
		public float regularEventTriggerAfter = 5f;  // seconds
		public float regularEventDuration = 5f;
		public float regularGoodGMin = 60f;
		//public float regularGoodGMax = 80f;
		//public float regularBadGMin = 10f;
		public float regularBadGMax = 40f;
		public float regularGStep = 3f;
		public float standardDeviation = 1f;

		public float eventMessageFadeOutTime = 2f;

		public Color gooldColor;
		public Color badColor;

		public GameObject eventMessage;

		public static StockEventSystem instance;

		float regularTimeDeltaAccumulated = 0f;
		float eventMessageTimeAccumulated = 0f;

		int eventStartMean;
		bool eventStartMeanSet = false;
		float g;

		RegularStockEvent regularStockEvent;
		bool eventChosen = false;

		Text eventMessageText;

		void Start () {
			instance = this;
			eventMessageText = eventMessage.GetComponent<Text> ();
		}

		public bool IsRegularEventForcast () {
			return regularEventForcastAfter <= regularTimeDeltaAccumulated &&
				regularTimeDeltaAccumulated < regularEventForcastAfter + regularEventTriggerAfter;
		}

		public void ChooseRegularStockEvent () {
			regularStockEvent = RegularStockEvents.events[Random.Range(0, RegularStockEvents.events.Length - 1)];
			eventMessageText.text = regularStockEvent.message;
			eventMessageText.color = regularStockEvent.good ? gooldColor : badColor;
			eventMessage.SetActive (true);
			eventMessageTimeAccumulated = 0f;
			eventChosen = true;
		}

		public bool IsRegularEventTrigger () {
			return regularEventForcastAfter + regularEventTriggerAfter <= regularTimeDeltaAccumulated &&
				regularTimeDeltaAccumulated < regularEventForcastAfter + regularEventTriggerAfter + regularEventDuration;
		}

		public bool IsEventStartMeanSet () {
			return eventStartMeanSet;
		}

		public void SetEventStartMean (int value) {
			eventStartMean = value;
			g = regularStockEvent.good ? regularGoodGMin : regularBadGMax;
		}

		public int GenerateNextStockPrice () {
			//float g = regularStockEvent.good ? 
			//	Random.Range (regularGoodGMin, regularGoodGMax) : Random.Range (regularBadGMin, regularBadGMax);
			if (regularStockEvent.good) {
				g += regularGStep;
			} else {
				g -= regularGStep;
			}
			float tan = Mathf.Tan (g * Mathf.Deg2Rad);
			float mean = eventStartMean * tan;
			int nextStockPrice = (int)SimpleRNG.GetNormal (mean, standardDeviation);
			if (regularStockEvent.good && StockWaveGame.instance.GetCurrentStockPrice () < 10 && nextStockPrice < 10) {
				nextStockPrice += Random.Range (20, 30);
			}
			Debug.Log ("mean: " + mean);
			return nextStockPrice;
		}

		public void Reset () {
			regularTimeDeltaAccumulated = 0f;
			eventMessageTimeAccumulated = 0f;
			eventStartMeanSet = false;
			eventChosen = false;
		}
		
		// Update is called once per frame
		void Update () {
			if (!StockWaveGame.instance) {
				return;
			}

			if (StockWaveGame.instance.IsGameOver ()) {
				return;
			}

			regularTimeDeltaAccumulated += Time.deltaTime;

			if (IsRegularEventForcast ()) {
				if (!eventChosen) {
					ChooseRegularStockEvent ();
				}
			} else if (regularTimeDeltaAccumulated >= regularEventForcastAfter + regularEventTriggerAfter + regularEventDuration) {
				eventChosen = false;
				eventStartMeanSet = false;
				regularTimeDeltaAccumulated = 0f;
			}

			if (eventMessage.activeSelf) {
				eventMessageTimeAccumulated += Time.deltaTime;
				if (regularEventTriggerAfter + 3f - eventMessageFadeOutTime <= eventMessageTimeAccumulated &&
					eventMessageTimeAccumulated < regularEventTriggerAfter + 3f) {
					Color color = eventMessageText.color;
					color.a -= 1f / eventMessageFadeOutTime;
					eventMessageText.color = color;
				} else if (eventMessageTimeAccumulated >= regularEventTriggerAfter + 3f) {
					eventMessage.SetActive (false);
					eventMessageTimeAccumulated = 0f;
				}
			}
		}
	}
}