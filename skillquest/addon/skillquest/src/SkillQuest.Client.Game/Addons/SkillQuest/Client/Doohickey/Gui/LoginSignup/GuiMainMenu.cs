using System.Net;
using System.Numerics;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Graphics.OpenGL;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Character;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;
using static SkillQuest.Shared.Engine.State;
using static SkillQuest.Client.Engine.State;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;

public class GuiMainMenu : Shared.Engine.ECS.Doohickey, IDrawable{
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/mainmenu");

    string address = "127.0.0.1:3698";
    string email = "omni@skill.quest";
    string password = "";

    IClientConnection? connection;

    public GuiMainMenu(){
        Stuffed += (_, _) => { Authenticator.Instance.LoginSuccess += OpenCharacterSelect; };

        Unstuffed += (_, _) => { Authenticator.Instance.LoginSuccess -= OpenCharacterSelect; };
    }

    void OpenCharacterSelect(IClientConnection clientConnection){
        if (connection is null) return;

        Stuff?.Add(new GuiCharacterSelection(connection));
        Stuff?.Remove(this);
    }

    Task? _connect = null;
    
    public void Draw(DateTime now, TimeSpan delta){
        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
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
                        var addr = new System.Net.Mail.MailAddress(trimmed);

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
