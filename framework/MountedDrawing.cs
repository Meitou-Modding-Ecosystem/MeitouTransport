using System.Numerics;
using HarmonyLib;
using Meitou.Data.Characters;
using Meitou.Game.Rendering.Objects;

namespace Meitou.Transport;

// Release-specific bridge to the posed characters. PoseLive has joined all its workers at this boundary.
internal static class MountedDrawing
{
    private delegate void Collect(object renderer, Presentation.Frame frame);
    private static Collect collect = null!;

    internal static void Install(Harmony harmony, Type renderer)
    {
        var live = AccessTools.Inner(renderer, "Live");
        var adapter = typeof(PosedFrame<,,,>).MakeGenericType(renderer, live,
            AccessTools.Field(live, "Asset").FieldType, AccessTools.Field(live, "State").FieldType);
        collect = AccessTools.Method(adapter, "CollectSaddles").CreateDelegate<Collect>();
        harmony.Patch(AccessTools.Method(renderer, "PoseLive"),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(MountedDrawing), nameof(AfterPose))));
    }

    private static void AfterPose(object __instance)
    {
        if (Presentation.FrameFor(__instance) is { Mounts.Count: > 0 } frame) collect(__instance, frame);
    }

    // Typed field delegates let the per-frame pass read the client's private struct array in place.
    private static class PosedFrame<TRenderer, TLive, TAsset, TState>
        where TRenderer : class where TLive : struct where TAsset : class where TState : class
    {
        private static readonly AccessTools.FieldRef<TRenderer, TLive[]> Entries = AccessTools.FieldRefAccess<TRenderer, TLive[]>("_live");
        private static readonly AccessTools.FieldRef<TRenderer, int> Count = AccessTools.FieldRefAccess<TRenderer, int>("_liveCount");
        private static readonly AccessTools.StructFieldRef<TLive, long> Key = AccessTools.StructFieldRefAccess<TLive, long>("Key");
        private static readonly AccessTools.StructFieldRef<TLive, bool> Portrait = AccessTools.StructFieldRefAccess<TLive, bool>("PortraitOnly");
        private static readonly AccessTools.StructFieldRef<TLive, Matrix4x4> World = AccessTools.StructFieldRefAccess<TLive, Matrix4x4>("World");
        private static readonly AccessTools.StructFieldRef<TLive, TAsset> Asset = AccessTools.StructFieldRefAccess<TLive, TAsset>("Asset");
        private static readonly AccessTools.StructFieldRef<TLive, TState> State = AccessTools.StructFieldRefAccess<TLive, TState>("State");
        private static readonly AccessTools.FieldRef<TAsset, SkeletonRig> Rig = AccessTools.FieldRefAccess<TAsset, SkeletonRig>("Rig");
        private static readonly AccessTools.FieldRef<TState, Matrix4x4[]> Bones = AccessTools.FieldRefAccess<TState, Matrix4x4[]>("Bones");

        internal static void CollectSaddles(object instance, Presentation.Frame frame)
        {
            var renderer = (TRenderer)instance;
            var entries = Entries(renderer);
            for (int i = 0; i < Count(renderer); i++)
            {
                ref var mount = ref entries[i];
                if (Portrait(ref mount) || !frame.Mounts.TryGetValue(Key(ref mount), out var fit)
                    || fit.Mesh.Length == 0 || BoneFrame(ref mount, fit.Bone) is not { } anchor) continue;
                var saddle = Matrix4x4.CreateFromQuaternion(fit.SaddleRotation) * Matrix4x4.CreateTranslation(fit.SaddleOffset)
                    * anchor * World(ref mount);
                if (Matrix4x4.Decompose(saddle, out var scale, out var rotation, out var origin))
                    frame.Meshes.Add(new DebrisPiece(frame.MeshKey(fit.Mesh, fit.Texture), scale.X, rotation, origin));
            }
        }

        // The skin palette maps the rest model to its animated pose. Starting with model-space axes at the bind
        // bone origin lets authors fit their art facing +Z without knowing the exporter-specific bone orientation.
        private static Matrix4x4? BoneFrame(ref TLive entry, string name)
        {
            var rig = Rig(Asset(ref entry));
            var bones = Bones(State(ref entry));
            return rig?.Handle(name) is int h && h < bones.Length
                ? Matrix4x4.CreateTranslation(rig.BindPosition[h]) * bones[h] : null;
        }
    }
}
