using UnityEngine;
using System.Collections.Generic;

public class Berries : MonoBehaviour {

    void Start()
    {
        // todo, adjust resources based on size
        List<InventoryEntry> foods = new List<InventoryEntry>();
        foods.Add(new InventoryEntry(new Food(), 100));
        GetComponent<Inventory>().inventory.Add(Tags.Food, foods);
        GetComponent<Inventory>().setTotalWeight();
    }
    
    void Update () {
	
	}
}
