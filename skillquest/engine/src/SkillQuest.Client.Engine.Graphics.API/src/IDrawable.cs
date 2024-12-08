namespace SkillQuest.Client.Engine.Graphics.API;

public interface IDrawable : IDisposable {
    public void Draw(DateTime now, TimeSpan delta);
}
