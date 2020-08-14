public class Skill {
    string title;
    float difficulty;

    public string Title { get => title; }
    public float Difficulty { get => difficulty; }

    public Skill(string title, float difficulty)
    {
        this.title = title;
        this.difficulty = difficulty;
    }

}
