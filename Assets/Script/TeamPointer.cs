using UnityEngine;
using System.Collections.Generic;

public class TeamPointer : MonoBehaviour {

    public uint team = 0;

    private TeamController teamController;
    public TeamController TeamController
    {
        get{ return teamController; }
        // on setting team, change this gameObjects color to reflect the teams
        set
        {
            if (teamController != null)
                teamController.deregister(this);

            teamController = value;
            value.register(this);

            List<MeshRenderer> meshRenders = new List<MeshRenderer>();

            if (GetComponent<MeshRenderer>() != null)
                meshRenders.Add(GetComponent<MeshRenderer>());

            meshRenders.AddRange(GetComponentsInChildren<MeshRenderer>());

            foreach (MeshRenderer meshRenderer in meshRenders)
                meshRenderer.material.color = value.teamColor;

            team = value.team;
        }
    }

	// Use this for initialization
	void Start ()
    {
       TeamsController teamsController = GameObject.FindWithTag(Tags.Teams).GetComponent<TeamsController>();
        if (team == 0)
            TeamController = teamsController.getDefaultTeam();
        else
            TeamController = teamsController.getOrCreateTeamByTeamNumber(team);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        teamController.deregister(this);
    }
}
