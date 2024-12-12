using System.Net;
using ImGuiNET;
using SkillQuest.API.Network;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Game.Base.Client.System.Gui.Character;
using SkillQuest.Game.Base.Client.System.Users;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Client.System.Gui.LoginSignup;

public class GuiMainMenu : SkillQuest.Shared.Engine.ECS.System, IDrawable{
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/mainmenu");

    string address = "127.0.0.1:3698";
    string email = "omni@skill.quest";
    string password = "";

    IClientConnection? connection;

    public GuiMainMenu(){
        Tracked += (_, _) => { Authenticator.Instance.LoginSuccess += OpenCharacterSelect; };

        Untracked += (_, _) => { Authenticator.Instance.LoginSuccess -= OpenCharacterSelect; };
    }

    void OpenCharacterSelect(IClientConnection connection){
        if (connection is null) return;

        Ledger?.Add(new GuiCharacterSelection(connection));
        Ledger?.Remove(this);
    }

    Task? _connect = null;

    public void Draw(DateTime now, TimeSpan delta){
        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoSavedSettings
            )
        ) {
            ImGui.InputTextWithHint("Address", "address", ref address, 128);
            ImGui.InputTextWithHint("Email", "email", ref email, 128);
            ImGui.InputTextWithHint("Password", "password", ref password, 128, ImGuiInputTextFlags.Password);

            ImGui.Separator();

            if (
                ImGui.Button("Login")
            ) {
                _connect = Task.Run(async () => {
                    var trimmed = email.Trim();

                    if (trimmed.EndsWith(".")) {
                        Console.WriteLine("Invalid Email");
                        return;
                    }

                    try {
                        var addr = new global::System.Net.Mail.MailAddress(trimmed);

                        if (addr.Address != trimmed) {
                            Console.WriteLine("Invalid Email");
                            return;
                        }
                    } catch {
                        Console.WriteLine("Invalid Email");
                        return;
                    }

                    email = trimmed;
                    connection = await SH.Net.Connect(IPEndPoint.Parse(address));

                    Authenticator.Instance.Login(connection, email, password);
                });
            }
            ImGui.End();
        }
    }
}
