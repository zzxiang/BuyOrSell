using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJJ2017StockWave {

	public class RegularStockEvents {

		public static RegularStockEvent[] events;

		static RegularStockEvents() {
			List<RegularStockEvent> eventList = new List<RegularStockEvent> ();

			RegularStockEvent evt;


			evt = new RegularStockEvent ();
			evt.message = "庆祝GGJ成功举办,股价上抬";
			evt.good = true;
			eventList.Add (evt);


			evt = new RegularStockEvent ();
			evt.message = "我国单身狗超过欧洲人口总和，股价大涨";
			evt.good = true;
			eventList.Add (evt);

			evt = new RegularStockEvent ();
			evt.message = "诸多道友渡劫，股价同期飞升";
			evt.good = true;
			eventList.Add (evt);

			evt = new RegularStockEvent ();
			evt.message = "法律承认同性婚姻，股民又相信爱情了";
			evt.good = true;
			eventList.Add (evt);

			evt = new RegularStockEvent ();
			evt.message = "今朝有酒今朝醉，明天股价登高位";
			evt.good = true;
			eventList.Add (evt);



			evt = new RegularStockEvent ();
			evt.message = "国足爆冷出线，足彩纷纷倒闭，股价暴跌";
			evt.good = false;
			eventList.Add (evt);

			evt = new RegularStockEvent ();
			evt.message = "春节将至，小学生放假，股价暴跌";
			evt.good = false;
			eventList.Add (evt);

			evt = new RegularStockEvent ();
			evt.message = "你挖坑来我去跳，恰似股价往下掉";
			evt.good = false;
			eventList.Add (evt);

			evt = new RegularStockEvent ();
			evt.message = "问君能有几多愁，恰似股价要跳楼";
			evt.good = false;
			eventList.Add (evt);

			evt = new RegularStockEvent ();
			evt.message = "林丹出轨李宗伟，股市马上要跳水";
			evt.good = false;
			eventList.Add (evt);



			events = eventList.ToArray ();
		}
	}
}