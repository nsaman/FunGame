using UnityEngine;
using System.Collections.Generic;

public class Berries : MonoBehaviour {

    void Start()
    {
        // todo, adjust resources based on size
        Dictionary<Item, uint> foods = new Dictionary<Item, uint>();
        GetComponent<Inventory>().add(Items.getItemByTag(Tags.Food), 100);
        GetComponent<Inventory>().setTotalWeight();
    }
    
    void Update () {
	
	}
}
