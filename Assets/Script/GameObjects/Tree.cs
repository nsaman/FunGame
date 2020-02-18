using UnityEngine;
using System.Collections.Generic;

public class Tree : MonoBehaviour {
    
	void Start () {
        // todo, adjust tree resources based on size
        Dictionary<Item, uint> woods = new Dictionary<Item, uint>();
        woods.Add(new Wood(), 100);
        GetComponent<Inventory>().inventory.Add(Tags.Wood, woods);
        GetComponent<Inventory>().setTotalWeight();
    }
	
	void Update () {
	    
	}
}
