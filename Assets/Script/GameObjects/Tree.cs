using UnityEngine;

public class Tree : MonoBehaviour {
    
    
	void Start () {
        // todo, adjust tree resources based on size
        GetComponent<Inventory>().inventory.Add(new InventoryEntry(Tags.Wood, 100));
    }
	
	void Update () {
	    
	}
}
