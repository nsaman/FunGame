using UnityEngine;
using System.Collections.Generic;

public class Tree : MonoBehaviour {
    
	void Start () {
        // todo, adjust tree resources based on size
        Dictionary<Item, uint> woods = new Dictionary<Item, uint>();
        GetComponent<Inventory>().add(Items.getItemByTag(Tags.Wood), 100);
        GetComponent<Inventory>().setTotalWeight();
    }
	
	void Update () {
	    
	}
}
