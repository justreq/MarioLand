using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MarioLand.Content.Tiles;
public class BrickBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 16 };
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        TileObjectData.addTile(Type);
        AddMapEntry(new Color(200, 88, 0));
    }

    public static Vector2 TopLeft(int i, int j) => new(i - (Main.tile[i, j].TileFrameX / 18 % 2), j - (Main.tile[i, j].TileFrameY / 18 % 2));

    public static bool IsFull(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;

    public override bool RightClick(int i, int j)
    {
        Vector2 topLeft = TopLeft(i, j);

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Main.tile[(int)topLeft.X + x, (int)topLeft.Y + y].TileFrameX += (short)(IsFull((int)topLeft.X + x, (int)topLeft.Y + y) ? -36 : 36);
            }
        }

        Main.NewText($"Changed block state to {(IsFull(i, j) ? "Full" : "Empty")}");

        return true;
    }

    public override bool CanKillTile(int i, int j, ref bool blockDamaged)
    {
        return !IsFull(i, j);
    }

    public override bool CreateDust(int i, int j, ref int type)
    {
        return !IsFull(i, j) && base.CreateDust(i, j, ref type);
    }
}
