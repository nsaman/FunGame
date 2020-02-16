using UnityEngine;
using System.Collections.Generic;

public class TeamsController : MonoBehaviour {

    List<TeamController> teams;

	// Use this for initialization
	void Awake () {
        teams = new List<TeamController>();
    }
	
	// Update is called once per frame
	void Update () {
	    foreach(TeamController team in teams)
        {
            team.AI.handleTeam();
        }
	}

    public TeamController createTeam()
    {
       Color teamColor = new Color(Random.value, Random.value, Random.value, 0);
        return createTeam(teamColor);
    }

    public TeamController createTeam(Color color)
    {
        TeamController tc = new TeamController();
        tc.team = teams.Count;
        tc.teamColor = color;
        teams.Add(tc);

        return tc;
    }

    public TeamController getDefaultTeam()
    {
        if (teams.Count == 0)
            createTeam();
        return teams[0];
    }

    public TeamController getTeamByIndex(int index)
    {
        if (index + 1 > teams.Count)
            return teams[index];
        else
            return null;
    }
}
