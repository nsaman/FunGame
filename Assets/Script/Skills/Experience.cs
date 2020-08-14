using System;
using UnityEngine;

public class Experience {

    public const int START_LEVEL_VARIANCE = 10;
    public const double LEVEL_EXPERIENCE_GROWTH_RATE = 1.05f;
    public const int LEVEL_ONE_PROGRESS_TO_LEVEL = 10;

    Skill skill;
    int level;
    int progress;
    int nextLevelCache;

    public Skill Skill { get => skill; }
    public int Level { get => level; }
    public int Progress { get => progress; }

    public Experience(Skill skill)
    {
        this.skill = skill;
        level = RandomSingleton.Instance.Next(START_LEVEL_VARIANCE) + 1;
        progress = 0;
        SetNextLevelCache();
    }

    public Experience(Skill skill, int level, int progress)
    {
        this.skill = skill;
        this.level = level;
        this.progress = progress;
        SetNextLevelCache();
    }

    public void gainXp(int amount)
    {
        progress += amount;
        if(progress >= nextLevelCache)
        {
            progress -= nextLevelCache;
            level++;
            SetNextLevelCache();

            Debug.Log(skill.Title + " leveled to " + level);
        }
    }

    void SetNextLevelCache()
    {
        nextLevelCache = (int) (LEVEL_ONE_PROGRESS_TO_LEVEL *  Math.Pow(LEVEL_EXPERIENCE_GROWTH_RATE, level) * skill.Difficulty);
    }
}
