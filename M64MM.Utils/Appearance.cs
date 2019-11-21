﻿using System.Drawing;
using static M64MM.Utils.Core;
namespace M64MM.Utils
{
    public class Looks
    {

        static readonly int[] shadowAddresses = {
            0x07EC30,
            0x07EC48,
            0x07EC60,
            0x07EC78,
            0x07EC90,
            0x07ECA8
        };

        public enum ShadowParts
        {
            X,
            Y,
            Z
        }

        public enum VanillaModelColor
        {
            PantsShade,
            PantsMain,
            HatShade,
            HatMain,
            GloveShade,
            GloveMain,
            ShoeShade,
            ShoeMain,
            SkinShade,
            SkinMain,
            HairShade,
            HairMain
        }

        public static void WriteColor(VanillaModelColor ModelPart, Color color_)
        {
            long writeToAddress = 0;
            byte[] colors = new byte[4];
            switch (ModelPart)
            {
                case VanillaModelColor.PantsShade:
                    writeToAddress = 0x07EC20;
                    break;
                case VanillaModelColor.PantsMain:
                    writeToAddress = 0x07EC28;
                    break;
                case VanillaModelColor.HatShade:
                    writeToAddress = 0x07EC38;
                    break;
                case VanillaModelColor.HatMain:
                    writeToAddress = 0x07EC40;
                    break;
                case VanillaModelColor.GloveShade:
                    writeToAddress = 0x07EC50;
                    break;
                case VanillaModelColor.GloveMain:
                    writeToAddress = 0x07EC58;
                    break;
                case VanillaModelColor.ShoeShade:
                    writeToAddress = 0x07EC68;
                    break;
                case VanillaModelColor.ShoeMain:
                    writeToAddress = 0x07EC70;
                    break;
                case VanillaModelColor.SkinShade:
                    writeToAddress = 0x07EC80;
                    break;
                case VanillaModelColor.SkinMain:
                    writeToAddress = 0x07EC88;
                    break;
                case VanillaModelColor.HairShade:
                    writeToAddress = 0x07EC98;
                    break;
                case VanillaModelColor.HairMain:
                    writeToAddress = 0x07ECA0;
                    break;
            }
            colors[0] = color_.R;
            colors[1] = color_.G;
            colors[2] = color_.B;
            colors[3] = 0x0;
            if (writeToAddress > 0)
            {
                WriteBytes(BaseAddress + writeToAddress, SwapEndianRet(colors, 4));
            }
        }

        public static void fromColorCode(string code)
        {
            //Trim some data we don't need anymore. Now each line of the color code is represented by 3 bytes.
            byte[] data = StringToByteArray(code.Replace("8107EC", ""));

            //Every 6 bytes of the trimmed data represents one color.
            //The first line holds the red and green values, and the second line holds the blue value.
            for (int i = 0; i < data.Length / 6; i++)
            {
                byte r = data[(i * 6) + 1];
                byte g = data[(i * 6) + 2];
                byte b = data[(i * 6) + 4];

                switch (data[(i * 6)])
                {
                    case 0x38:
                        WriteColor(VanillaModelColor.HatShade, Color.FromArgb(r, g, b));
                        break;
                    case 0x40:
                        WriteColor(VanillaModelColor.HatMain, Color.FromArgb(r, g, b));
                        break;
                    case 0x98:
                        WriteColor(VanillaModelColor.HairShade, Color.FromArgb(r, g, b));
                        break;
                    case 0xA0:
                        WriteColor(VanillaModelColor.HairMain, Color.FromArgb(r, g, b));
                        break;
                    case 0x80:
                        WriteColor(VanillaModelColor.SkinShade, Color.FromArgb(r, g, b));
                        break;
                    case 0x88:
                        WriteColor(VanillaModelColor.SkinMain, Color.FromArgb(r, g, b));
                        break;
                    case 0x50:
                        WriteColor(VanillaModelColor.GloveShade, Color.FromArgb(r, g, b));
                        break;
                    case 0x58:
                        WriteColor(VanillaModelColor.GloveMain, Color.FromArgb(r, g, b));
                        break;
                    case 0x20:
                        WriteColor(VanillaModelColor.PantsShade, Color.FromArgb(r, g, b));
                        break;
                    case 0x28:
                        WriteColor(VanillaModelColor.PantsMain, Color.FromArgb(r, g, b));
                        break;
                    case 0x68:
                        WriteColor(VanillaModelColor.ShoeShade, Color.FromArgb(r, g, b));
                        break;
                    case 0x70:
                        WriteColor(VanillaModelColor.ShoeMain, Color.FromArgb(r, g, b));
                        break;
                }
            }
        }

        public static void changeShadow(int amount, ShadowParts part)
        {
            byte[] data = new byte[1];
            data[0] = (byte)amount;

            foreach (int address in shadowAddresses)
            {
                switch (part)
                {
                    case ShadowParts.X:
                        WriteBytes(BaseAddress + address + 3, data);
                        break;
                    case ShadowParts.Y:
                        WriteBytes(BaseAddress + address + 2, data);
                        break;
                    case ShadowParts.Z:
                        WriteBytes(BaseAddress + address + 1, data);
                        break;
                }
            }
        }
    }
}