using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TeamsController : MonoBehaviour {

    Dictionary<uint, TeamController> teams;
    uint highestTeam=0;

    // Use this for initialization
    void Awake () {
        teams = new Dictionary<uint, TeamController>();
    }
	
	// Update is called once per frame
	void Update () {
	    foreach(KeyValuePair<uint, TeamController> team in teams)
        {
            team.Value.AI.handleTeam();
        }
	}

    public TeamController createTeam()
    {
       Color teamColor = new Color(Random.value, Random.value, Random.value, 0);
       return createTeam(teamColor, 0);
    }

    public TeamController createTeam(uint teamNumber)
    {
        Color teamColor = new Color(Random.value, Random.value, Random.value, 0);
        return createTeam(teamColor, teamNumber);
    }

    public TeamController createTeam(Color color, uint teamNumber)
    {
        if (teams.ContainsKey(teamNumber))
            return teams[teamNumber];

        TeamController tc = new TeamController();
        tc.teamColor = color;
        if(teamNumber == 0)
        {
            highestTeam++;
            tc.team = highestTeam;
            teams.Add(highestTeam, tc);
        }
        else
        {
            tc.team = teamNumber;
            teams.Add(teamNumber, tc);

            if (teamNumber > highestTeam)
                highestTeam = teamNumber;
        }

        return tc;
    }

    public TeamController getDefaultTeam()
    {
        if (teams.Count == 0)
            createTeam();
        return teams.Values.ToList()[0];
    }

    public TeamController getOrCreateTeamByTeamNumber(uint teamNumber)
    {
        if (teams.ContainsKey(teamNumber))
            return teams[teamNumber];
        else
            return createTeam(teamNumber);
    }
}
