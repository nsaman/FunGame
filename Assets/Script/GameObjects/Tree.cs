using UnityEngine;
using System.Collections.Generic;

public class Tree : MonoBehaviour {
    
    
	void Start () {
        // todo, adjust tree resources based on size
        List<InventoryEntry> woods = new List<InventoryEntry>();
        woods.Add(new InventoryEntry(new Wood(), 100));
        GetComponent<Inventory>().inventory.Add(Tags.Wood, woods);
        GetComponent<Inventory>().setTotalWeight();
    }
	
	void Update () {
	    
	}
}
