using System.Numerics;
using Meitou.Data.Characters;
using Meitou.Simulation;

namespace Meitou.Transport;

// Schema 3 measures from the saddle to the rider's pelvis. DrawAnchor places the rider's node.
internal sealed class RiderAnchor(CharacterRigs rigs, CharacterCold rider, Quaternion inverseBind)
{
    internal const string PoseName = "sitting chair";
    internal const float PoseTime = 1;
    private CharacterAppearance? appearance;
    private BodyBuild? bodyBuild;
    private Vector3 pelvisFromNode;
    private DrawAnchor? written;

    internal static RiderAnchor? For(CharacterRigs rigs, CharacterCold rider, CharacterCold mount, string bone)
    {
        if (mount.Appearance is not { } look || rigs.For(look)?.Rig is not { } rig || rig.Handle(bone) is not int h)
            return null;
        return new RiderAnchor(rigs, rider, rig.BindRotationInverse[h]);
    }

    internal bool Update(CharacterId carrier, MountDefinition fit, float carrierScale)
    {
        if (rider.Appearance is not { } look || carrierScale <= 0) return false;
        var build = rider.ShapedBuild;
        bool sameBuild = build is { } next && bodyBuild is { } previous ? previous.Near(next) : build == bodyBuild;
        if (!ReferenceEquals(appearance, look) || !sameBuild)
        {
            if (rigs.For(look) is not { } setup || setup.Rig.Handle("Bip01 Pelvis") is not int h
                || setup.Rig.Layer(rigs.Database, PoseName) is not { } seat) return false;
            var reshaped = build is { } current ? rigs.Reshape(look, current) : null;
            var layers = new List<ActiveLayer> { new(seat, PoseTime, 1) };
            foreach (var (layer, time) in setup.Fixed) layers.Add(new ActiveLayer(layer, time, 1));
            var scratch = setup.Rig.Rent();
            try
            {
                scratch.PoseDerived(reshaped?.Shape ?? setup.Shape, System.Runtime.InteropServices.CollectionsMarshal.AsSpan(layers));
                // WorldPublish uses node scale 1 for humanoids. Their height comes from the shaped rig.
                pelvisFromNode = scratch.DerivedPosition[h] - Vector3.UnitY * (reshaped?.GroundOffset ?? setup.GroundOffset);
            }
            finally { setup.Rig.Return(scratch); }
            appearance = look;
            bodyBuild = build;
        }

        written = rider.DrawAnchor = Calculate(carrier, fit, inverseBind, pelvisFromNode, carrierScale);
        return true;
    }

    internal static DrawAnchor Calculate(CharacterId carrier, MountDefinition fit, Quaternion inverseBind,
        Vector3 pelvisFromNode, float carrierScale)
    {
        var facing = fit.SaddleRotation * Quaternion.CreateFromAxisAngle(Vector3.UnitY, fit.SeatYaw);
        var seat = fit.SaddleOffset + Vector3.Transform(fit.SeatOffset, fit.SaddleRotation);
        // The renderer scales the entire offset by the animal's size. Undo that scale for the rider correction only.
        var node = seat - Vector3.Transform(pelvisFromNode, facing) / carrierScale;
        return new DrawAnchor(carrier, fit.Bone, Vector3.Transform(node, inverseBind), Quaternion.Normalize(inverseBind * facing));
    }

    internal void Clear()
    {
        if (rider.DrawAnchor == written) rider.DrawAnchor = null;
        written = null;
    }
}
