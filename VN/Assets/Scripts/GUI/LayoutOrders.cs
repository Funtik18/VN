using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutOrders : MonoBehaviour{
	public static LayoutOrders _instance;

	public Canvas map;
	public Canvas gui;
	public Canvas ui;

	private void Awake() {
		_instance = this;
		MakeMainMap();
	}

	public void MakeMainMap() {
		map.sortingOrder = 1;
		gui.sortingOrder = -1;
		ui.sortingOrder = 10;
	}
	public void MakeMainGUI() {
		map.sortingOrder = -1;
		gui.sortingOrder = 1;
		ui.sortingOrder = 10;
	}
	public void MakeMainUI() {
		map.sortingOrder = -1;
		gui.sortingOrder = 1;
		ui.sortingOrder = 10;
	}
}
