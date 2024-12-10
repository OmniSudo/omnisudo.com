using System.Net;
using System.Text;
using ImGuiNET;
using SkillQuest.Addon.Base.Client.System.Gui.Character;
using SkillQuest.Addon.Base.Client.System.Users;
using SkillQuest.API.Network;
using SkillQuest.Client.Engine.Graphics.API;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Client.System.Gui.LoginSignup;

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

        SH.Assets.Open(connection, "./game/SkillQuest.Game.Base.Shared/assets/Component/Item/Mining/Ore.xml")
            .ContinueWith(
                task => { Console.WriteLine(Encoding.UTF8.GetString(task.Result)); }
            );

        Entities?.Add(new GuiCharacterSelection(connection));
        Entities?.Remove(this);
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
