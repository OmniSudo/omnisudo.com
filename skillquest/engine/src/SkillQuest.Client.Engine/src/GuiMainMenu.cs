using SkillQuest.API;
using SkillQuest.Shared.Addon.SkillQuest.Doohickey.GUI;

namespace SkillQuest.Client.Engine;

public class GuiMainMenu : GuiBase {
    public override Uri? Uri { get; set; } = new Uri("cl://gui.skill.quest/main");

    public override string Description { get; } = "Main Menu";

    public override void OnDraw() {
        // Clear the screen
        SH.Screen.Clear();

        // Draw the background
        SH.Screen.DrawBackground(0, 0, SH.Screen.Width, SH.Screen.Height);

        // Center and draw the main menu buttons
        int centerX = (SH.Screen.Width - MainButtonWidth) / 2;
        int centerY = (SH.Screen.Height - MainButtonHeight) / 2;

        for (int i = 0; i < MainMenuButtons.Count; i++) {
            GuiButton button = MainMenuButtons[i];
            SH.Screen.DrawText(button.Text, centerX + (i % 3 * (MainButtonWidth + 10)), centerY + (i / 3 * (MainButtonHeight + 10)));
        }

        // Draw the main menu title
        SH.Screen.DrawText("SkillQuest", SH.Screen.Width / 2 - 50, 20);

        // Update the screen
        SH.Screen.Update();
    }
}
