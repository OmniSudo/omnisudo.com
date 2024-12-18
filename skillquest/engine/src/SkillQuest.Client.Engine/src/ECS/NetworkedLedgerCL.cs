using System.Collections.Concurrent;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Packet.Entity;
using DateTime = System.DateTime;

namespace SkillQuest.Client.Engine.ECS;

public class NetworkedLedgerCL : EntityLedger{
    IChannel _channel;

    public NetworkedLedgerCL(){
        _channel = Shared.Engine.State.SH.Net.CreateChannel(new Uri("packet://skill.quest/ledger"));

        _channel.Subscribe<EntityUploadPacket>(OnEntityUploadPacket);
    }

    ConcurrentDictionary<Uri, Dictionary<DateTime, TaskCompletionSource<IEntity?>>> _tasks;

    public override async Task<IEntity?> Download(Uri uri, IClientConnection source, DateTime? after){
        after ??= DateTime.Now;
        var timed_tasks = _tasks.GetOrAdd(uri, _ => new Dictionary<DateTime, TaskCompletionSource<IEntity?>>());

        var tcs = timed_tasks.Where(pair => pair.Key > after)
            .OrderBy(pair => pair.Key)
            .Select(pair => pair.Value)
            .FirstOrDefault();

        if (tcs is null) {
            timed_tasks[after.Value] = tcs = new TaskCompletionSource<IEntity?>();
        }

        _channel.Send(source, new EntityDownloadRequestPacket() { Uri = uri, MinTime = after.Value });

        return await tcs.Task;
    }

    void OnEntityUploadPacket(IClientConnection connection, EntityUploadPacket packet){
        var type = Type.GetType(packet.Data["$type"].ToString());

        var timed_tasks = _tasks.GetValueOrDefault(packet.Uri);

        if (!type?.IsAssignableTo(typeof(IEntity)) ?? true) {
            if (timed_tasks is not null) {
                var times = new List<DateTime>();

                foreach (var tcs in timed_tasks.Where(pair => pair.Key < packet.MinTime)) {
                    tcs.Value.SetResult(null);
                    times.Add(tcs.Key);
                }

                foreach (var time in times) {
                    timed_tasks.Remove(time);
                }
            }
            return;
        }

        var current = this[packet.Uri];

        if (current is null) {
            if (Activator.CreateInstance(type) is not IEntity ent) return;
            ent.Uri = packet.Uri;

            Task<IEntity?>? parent = null;

            if (Uri.TryCreate(packet.Data["parent"]?.ToString(), UriKind.Absolute, out var parentUri)) {
                parent = Download( parentUri, connection, packet.MinTime );
            }

            var children = packet.Data["children"]?.AsObject().ToDictionary(
                pair => new Uri(pair.Key),
                pair => {
                    if (Uri.TryCreate(pair.Value?.ToString(), UriKind.Absolute, out var uri)) {
                        return Download(uri, connection, packet.MinTime);
                    }
                    return null;
                }
            );

            ent.Ledger = this;

            ent.Parent = parent?.Result;

            foreach (var tasks in children ?? []) {
                ent[tasks.Key] = tasks.Value?.Result;
            }

            current = ent;
        } else {
            if (type != current.GetType()) {
                if (Activator.CreateInstance(type) is not IEntity ent) return;
                ent.Uri = packet.Uri;
                var components = current.Components;

                foreach (var component in components) {
                    ent[component.Key] = component.Value;
                    component.Value.Entity = ent;
                }

                ent.Parent = current.Parent;

                foreach (var child in current.Children) {
                    ent[child.Key] = child.Value;
                }

                ent.Ledger = this;

                current = ent;
            }
        }

        current.FromJson(packet.Data);

        var times_processed = new List<DateTime>();
        foreach (var pair in timed_tasks.Where(pair => pair.Key < packet.MinTime)) {
            times_processed.Add(pair.Key);
            pair.Value.SetResult(current);
        }

        foreach (var time in times_processed) {
            timed_tasks.Remove(time);
        }
    }

    public override Task<IEntity?> Upload(IEntity entity, IClientConnection destination){
        throw new NotImplementedException();
    }
}
