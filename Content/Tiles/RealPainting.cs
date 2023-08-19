using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace MarioLand.Content.Tiles;
public abstract class RealPainting : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(63, 50, 39));
    }
}

public class RealMarioPainting : RealPainting { }
public class RealLuigiPainting : RealPainting { }