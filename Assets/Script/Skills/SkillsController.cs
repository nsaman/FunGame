using System.Collections.Generic;
using UnityEngine;

public class SkillsController : MonoBehaviour
{
    Dictionary<Skill, Experience> levels;

    void Start()
    {
        levels = new Dictionary<Skill, Experience>();
    }

    public int getLevel(Skill skill)
    {
        if(!levels.ContainsKey(skill))
        {
            levels.Add(skill, new Experience(skill));
        }

        return levels[skill].Level;
    }

    public void gainXp(Skill skill, int amount)
    {
        if (!levels.ContainsKey(skill))
        {
            levels.Add(skill, new Experience(skill));
        }

        levels[skill].gainXp(amount);
    }
}
