using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CustomItems {
    [ApiVersion(2, 1)]
    public class CustomItems : TerrariaPlugin {
        public override string Name => "CustomItems";
        public override string Author => "Johuan";
        public override string Description => "Allows you to spawn custom items";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public CustomItems(Main game) : base(game) {
        }

        public override void Initialize() {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args) {
            Commands.ChatCommands.Add(new Command("customitem", CustomItem, "customitem", "citem") {
                HelpText = "/customitem <id/itemname> <parameters> <#> ... \nParameters: hexcolor, damage, knockback, useanimation, " +
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo. \n" +
                "Random value identifiers: random, rand, and r. \n" +
                "Colors: red, blue, green, yellow, orange, black, white, pink, purple, teal, navy, turquoise, indigo, gray, cyan, brown, and beige"
            });

            Commands.ChatCommands.Add(new Command("customitem.give", GiveCustomItem, "givecustomitem", "gcitem") {
                HelpText = "/givecustomitem <name> <id/itemname> <parameters> <#> ... \nParameters: hexcolor, damage, knockback, useanimation, " +
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo. \n" +
                "Random value identifiers: random, rand, and r. \n" +
                "Colors: red, blue, green, yellow, orange, black, white, pink, purple, teal, navy, turquoise, indigo, gray, cyan, brown, and beige"
            });
        }

        private void CustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Count();

            if (num == 0) {
                args.Player.SendErrorMessage("Invalid Syntax. /customitem <id/itemname> <parameters> <#> ... \nParameters: hexcolor, damage, knockback, useanimation, " +
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo.");
                return;
            }

            List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
            if (items.Count == 0) {
                args.Player.SendErrorMessage($"No item found by the name of {args.Parameters[0]}.");
                return;
            }
            Item item = items[0];

            TSPlayer player = new TSPlayer(args.Player.Index);
            int itemIndex = Item.NewItem((int)player.X, (int)player.Y, item.width, item.height, item.type, item.maxStack);

            Item targetItem = Main.item[itemIndex];
            targetItem.playerIndexTheItemIsReservedFor = args.Player.Index;

            var pairedInputs = SplitIntoPairs<string>(parameters.Skip(1).ToArray());

            foreach (var pair in pairedInputs) {
                string param = pair[0];
                string arg = pair[1];
                switch (param) {
                    case "hexcolor":
                    case "hc":
                    case "color":

                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.color = getRandomColor();
                        }
                        else if (arg.Equals("red"))
                        {
                            targetItem.color = Color.Red;
                        }
                        else if (arg.Equals("blue"))
                        {
                            targetItem.color = Color.Blue;
                        }
                        else if (arg.Equals("green"))
                        {
                            targetItem.color = Color.Green;
                        }
                        else if (arg.Equals("yellow"))
                        {
                            targetItem.color = Color.Yellow;
                        }
                        else if (arg.Equals("orange"))
                        {
                            targetItem.color = Color.Orange;
                        }
                        else if (arg.Equals("black"))
                        {
                            targetItem.color = Color.Black;
                        }
                        else if (arg.Equals("white"))
                        {
                            targetItem.color = Color.White;
                        }
                        else if (arg.Equals("pink"))
                        {
                            targetItem.color = Color.Pink;
                        }
                        else if (arg.Equals("purple"))
                        {
                            targetItem.color = Color.Purple;
                        }
                        else if (arg.Equals("teal"))
                        {
                            targetItem.color = Color.Teal;
                        }
                        else if (arg.Equals("navy"))
                        {
                            targetItem.color = Color.Navy;
                        }
                        else if (arg.Equals("turquoise"))
                        {
                            targetItem.color = Color.Turquoise;
                        }
                        else if (arg.Equals("indigo"))
                        {
                            targetItem.color = Color.Indigo;
                        }
                        else if (arg.Equals("gray"))
                        {
                            targetItem.color = Color.Gray;
                        }
                        else if (arg.Equals("cyan"))
                        {
                            targetItem.color = Color.Cyan;
                        }
                        else if (arg.Equals("brown"))
                        {
                            targetItem.color = Color.Brown;
                        }
                        else if (arg.Equals("beige"))
                        {
                            targetItem.color = Color.Beige;
                        }
                        else
                        {
                            targetItem.color = new Color(int.Parse(arg.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                        }

                        break;
                    case "damage":
                    case "d":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.damage = genRandom(1, 1000); // damage can range from 1 to 1000
                        }
                        else
                        {
                            targetItem.damage = int.Parse(arg);
                        }
                        break;
                    case "knockback":
                    case "kb":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.knockBack = genRandom(1, 100); // knockback can range from 1 to 10
                        }
                        else
                        {
                            targetItem.knockBack = int.Parse(arg);
                        }
                        break;
                    case "useanimation":
                    case "ua":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.useAnimation = genRandom(2, 10); // useanimation can range from 2 to 10
                        }
                        else
                        {
                            targetItem.useAnimation = int.Parse(arg);
                        }
                        break;
                    case "usetime":
                    case "ut":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.useTime = genRandom(1, 40); // usetime can range from 1 to 40
                        }
                        else
                        {
                            targetItem.useTime = int.Parse(arg);
                        }
                        break;
                    case "shoot":
                    case "s":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.shoot = genRandom(1, 955); // 650 for 1.3 | 955 for 1.4
                        }
                        else
                        {
                            targetItem.shoot = int.Parse(arg);
                        }
                        break;
                    case "shootspeed":
                    case "ss":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.shootSpeed = genRandom(1, 50); // shootspeed can range from 1 to 50
                        }
                        else
                        {
                            targetItem.shootSpeed = int.Parse(arg);
                        }
                        break;
                    case "width":
                    case "w":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.width = genRandom(1, 255); // width can range from 1 to 255
                        }
                        else
                        {
                            targetItem.width = int.Parse(arg);
                        }
                        break;
                    case "height":
                    case "h":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.height = genRandom(1, 255); // height can range from 1 to 255
                        }
                        else
                        {
                            targetItem.height = int.Parse(arg);
                        }
                        break;
                    case "scale":
                    case "sc":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.scale = genRandom(1, 10); // scale can range from 1 to 10
                        }
                        else
                        {
                            targetItem.scale = int.Parse(arg);
                        }
                        break;
                    case "ammo":
                    case "a":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.ammo = genRandom(0, 10); // ammo can range from 0 to 10
                        }
                        else
                        {
                            targetItem.ammo = int.Parse(arg);
                        }
                        break;
                    case "useammo":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.useAmmo = genRandom(0, 10);
                        }
                        else
                        {
                            targetItem.useAmmo = int.Parse(arg);
                        }
                        break;
                    case "notammo":
                    case "na":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.notAmmo = Convert.ToBoolean(genRandom(0, 1));
                        }
                        else
                        {
                            targetItem.notAmmo = Boolean.Parse(arg);
                        }
                        break;
                }
            }

            TSPlayer.All.SendData(PacketTypes.UpdateItemDrop, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.ItemOwner, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.TweakItem, null, itemIndex, 255, 63);
        }

        private void GiveCustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Counta();

            if (num == 0) {
                args.Player.SendErrorMessage("Invalid Syntax. /givecustomitem <name> <id/itemname> <parameters> <#> ... \nParameters: hexcolor, damage, knockback, useanimation, " +
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo.");
                return;
            }

            List<TSPlayer> players = TShock.Players.Where(c => c.Name.Contains(args.Parameters[0])).ToList();
            if (players.Count != 1) {
                args.Player.SendErrorMessage("Failed to find player of: " + args.Parameters[0]);
                return;
            }

            if (num == 1) {
                args.Player.SendErrorMessage("Failed to provide arguments to item.");
                return;
            }

            List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (items.Count == 0) {
                args.Player.SendErrorMessage($"No item found by the name of {args.Parameters[1]}.");
                return;
            }
            Item item = items[0];

            TSPlayer player = new TSPlayer(players[0].Index);
            int itemIndex = Item.NewItem((int)player.X, (int)player.Y, item.width, item.height, item.type, item.maxStack);

            Item targetItem = Main.item[itemIndex];
            targetItem.playerIndexTheItemIsReservedFor = args.Player.Index;

            var pairedInputs = SplitIntoPairs<string>(parameters.Skip(2).ToArray());
            foreach (var pair in pairedInputs) {
                string param = pair[0];
                string arg = pair[1];
                switch (param) {
                    case "hexcolor":
                    case "hc":
                    case "color":

                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.color = getRandomColor();
                        }
                        else if (arg.Equals("red"))
                        {
                            targetItem.color = Color.Red;
                        }
                        else if (arg.Equals("blue"))
                        {
                            targetItem.color = Color.Blue;
                        }
                        else if (arg.Equals("green"))
                        {
                            targetItem.color = Color.Green;
                        }
                        else if (arg.Equals("yellow"))
                        {
                            targetItem.color = Color.Yellow;
                        }
                        else if (arg.Equals("orange"))
                        {
                            targetItem.color = Color.Orange;
                        }
                        else if (arg.Equals("black"))
                        {
                            targetItem.color = Color.Black;
                        }
                        else if (arg.Equals("white"))
                        {
                            targetItem.color = Color.White;
                        }
                        else if (arg.Equals("pink"))
                        {
                            targetItem.color = Color.Pink;
                        }
                        else if (arg.Equals("purple"))
                        {
                            targetItem.color = Color.Purple;
                        }
                        else if (arg.Equals("teal"))
                        {
                            targetItem.color = Color.Teal;
                        }
                        else if (arg.Equals("navy"))
                        {
                            targetItem.color = Color.Navy;
                        }
                        else if (arg.Equals("turquoise"))
                        {
                            targetItem.color = Color.Turquoise;
                        }
                        else if (arg.Equals("indigo"))
                        {
                            targetItem.color = Color.Indigo;
                        }
                        else if (arg.Equals("gray"))
                        {
                            targetItem.color = Color.Gray;
                        }
                        else if (arg.Equals("cyan"))
                        {
                            targetItem.color = Color.Cyan;
                        }
                        else if (arg.Equals("brown"))
                        {
                            targetItem.color = Color.Brown;
                        }
                        else if (arg.Equals("beige"))
                        {
                            targetItem.color = Color.Beige;
                        }
                        else
                        {
                            targetItem.color = new Color(int.Parse(arg.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                        }

                        break;
                    case "damage":
                    case "d":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.damage = genRandom(1, 1000); // damage can range from 1 to 1000
                        }
                        else
                        {
                            targetItem.damage = int.Parse(arg);
                        }
                        break;
                    case "knockback":
                    case "kb":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.knockBack = genRandom(1, 100); // knockback can range from 1 to 10
                        }
                        else
                        {
                            targetItem.knockBack = int.Parse(arg);
                        }
                        break;
                    case "useanimation":
                    case "ua":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.useAnimation = genRandom(2, 10); // useanimation can range from 2 to 10
                        }
                        else
                        {
                            targetItem.useAnimation = int.Parse(arg);
                        }
                        break;
                    case "usetime":
                    case "ut":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.useTime = genRandom(1, 40); // usetime can range from 1 to 40
                        }
                        else
                        {
                            targetItem.useTime = int.Parse(arg);
                        }
                        break;
                    case "shoot":
                    case "s":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.shoot = genRandom(1, 955); // 650 for 1.3 | 955 for 1.4
                        }
                        else
                        {
                            targetItem.shoot = int.Parse(arg);
                        }
                        break;
                    case "shootspeed":
                    case "ss":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.shootSpeed = genRandom(1, 50); // shootspeed can range from 1 to 50
                        }
                        else
                        {
                            targetItem.shootSpeed = int.Parse(arg);
                        }
                        break;
                    case "width":
                    case "w":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.width = genRandom(1, 255); // width can range from 1 to 255
                        }
                        else
                        {
                            targetItem.width = int.Parse(arg);
                        }
                        break;
                    case "height":
                    case "h":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.height = genRandom(1, 255); // height can range from 1 to 255
                        }
                        else
                        {
                            targetItem.height = int.Parse(arg);
                        }
                        break;
                    case "scale":
                    case "sc":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.scale = genRandom(1, 10); // scale can range from 1 to 10
                        }
                        else
                        {
                            targetItem.scale = int.Parse(arg);
                        }
                        break;
                    case "ammo":
                    case "a":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.ammo = genRandom(0, 10); // ammo can range from 0 to 10
                        }
                        else
                        {
                            targetItem.ammo = int.Parse(arg);
                        }
                        break;
                    case "useammo":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.useAmmo = genRandom(0, 10);
                        }
                        else
                        {
                            targetItem.useAmmo = int.Parse(arg);
                        }
                        break;
                    case "notammo":
                    case "na":
                        if (arg.Equals("random") || arg.Equals("rand") || arg.Equals("r"))
                        {
                            targetItem.notAmmo = Convert.ToBoolean(genRandom(0, 1));
                        }
                        else
                        {
                            targetItem.notAmmo = Boolean.Parse(arg);
                        }
                        break;
                }
            }

            TSPlayer.All.SendData(PacketTypes.UpdateItemDrop, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.ItemOwner, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.TweakItem, null, itemIndex, 255, 63);
        }

        public static T[][] SplitIntoPairs<T>(T[] input) {
            T[][] split = new T[input.Length / 2][];

            for (int x = 0; x < input.Length / 2; x++) {
                split[x] = new[] { input[x * 2], input[x * 2 + 1] };
            }

            return split;
        }
        public Color getRandomColor()
        {
            Color color = Color.Transparent;

            Random rnd = new Random();
            int num = rnd.Next(1, 17);

            switch (num)
            {
                default:
                    break;
                case 1:
                    color = Color.Red;
                    break;
                case 2:
                    color = Color.Blue;
                    break;
                case 3:
                    color = Color.Green;
                    break;
                case 4:
                    color = Color.Yellow;
                    break;
                case 5:
                    color = Color.Orange;
                    break;
                case 6:
                    color = Color.Black;
                    break;
                case 7:
                    color = Color.White;
                    break;
                case 8:
                    color = Color.Pink;
                    break;
                case 9:
                    color = Color.Purple;
                    break;
                case 10:
                    color = Color.Teal;
                    break;
                case 11:
                    color = Color.Navy;
                    break;
                case 12:
                    color = Color.Turquoise;
                    break;
                case 13:
                    color = Color.Indigo;
                    break;
                case 14:
                    color = Color.Gray;
                    break;
                case 15:
                    color = Color.Cyan;
                    break;
                case 16:
                    color = Color.Brown;
                    break;
                case 17:
                    color = Color.Beige;
                    break;
            }

            return color;
        }

        public int genRandom(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }
    }
}
