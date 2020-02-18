using UnityEngine;
using System.Collections.Generic;

public class Berries : MonoBehaviour {

    void Start()
    {
        // todo, adjust resources based on size
        Dictionary<Item, uint> foods = new Dictionary<Item, uint>();
        foods.Add(new Food(), 100);
        GetComponent<Inventory>().inventory.Add(Tags.Food, foods);
        GetComponent<Inventory>().setTotalWeight();
    }
    
    void Update () {
	
	}
}
