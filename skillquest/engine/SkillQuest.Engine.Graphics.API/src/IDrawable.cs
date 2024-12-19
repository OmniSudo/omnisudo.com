namespace SkillQuest.Engine.Graphics.API;

public interface IDrawable : IDisposable {
    public void Draw(DateTime now, TimeSpan delta);
}
