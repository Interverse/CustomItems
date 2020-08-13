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
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo."
            });

            Commands.ChatCommands.Add(new Command("customitem.give", GiveCustomItem, "givecustomitem", "gcitem") {
                HelpText = "/givecustomitem <name> <id/itemname> <parameters> <#> ... \nParameters: hexcolor, damage, knockback, useanimation, " +
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo."
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
                        targetItem.color = new Color(int.Parse(arg.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                        break;
                    case "damage":
                    case "d":
                        targetItem.damage = int.Parse(arg);
                        break;
                    case "knockback":
                    case "kb":
                        targetItem.knockBack = int.Parse(arg);
                        break;
                    case "useanimation":
                    case "ua":
                        targetItem.useAnimation = int.Parse(arg);
                        break;
                    case "usetime":
                    case "ut":
                        targetItem.useTime = int.Parse(arg);
                        break;
                    case "shoot":
                    case "s":
                        targetItem.shoot = int.Parse(arg);
                        break;
                    case "shootspeed":
                    case "ss":
                        targetItem.shootSpeed = int.Parse(arg);
                        break;
                    case "width":
                    case "w":
                        targetItem.width = int.Parse(arg);
                        break;
                    case "height":
                    case "h":
                        targetItem.height = int.Parse(arg);
                        break;
                    case "scale":
                    case "sc":
                        targetItem.scale = int.Parse(arg);
                        break;
                    case "ammo":
                    case "a":
                        targetItem.ammo = int.Parse(arg);
                        break;
                    case "useammo":
                        targetItem.useAmmo = int.Parse(arg);
                        break;
                    case "notammo":
                    case "na":
                        targetItem.notAmmo = Boolean.Parse(arg);
                        break;
                }
            }

            TSPlayer.All.SendData(PacketTypes.UpdateItemDrop, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.ItemOwner, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.TweakItem, null, itemIndex, 255, 63);
        }

        private void GiveCustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Count();

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
                        targetItem.color = new Color(int.Parse(arg.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                            int.Parse(arg.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                        break;
                    case "damage":
                    case "d":
                        targetItem.damage = int.Parse(arg);
                        break;
                    case "knockback":
                    case "kb":
                        targetItem.knockBack = int.Parse(arg);
                        break;
                    case "useanimation":
                    case "ua":
                        targetItem.useAnimation = int.Parse(arg);
                        break;
                    case "usetime":
                    case "ut":
                        targetItem.useTime = int.Parse(arg);
                        break;
                    case "shoot":
                    case "s":
                        targetItem.shoot = int.Parse(arg);
                        break;
                    case "shootspeed":
                    case "ss":
                        targetItem.shootSpeed = int.Parse(arg);
                        break;
                    case "width":
                    case "w":
                        targetItem.width = int.Parse(arg);
                        break;
                    case "height":
                    case "h":
                        targetItem.height = int.Parse(arg);
                        break;
                    case "scale":
                    case "sc":
                        targetItem.scale = int.Parse(arg);
                        break;
                    case "ammo":
                    case "a":
                        targetItem.ammo = int.Parse(arg);
                        break;
                    case "useammo":
                        targetItem.useAmmo = int.Parse(arg);
                        break;
                    case "notammo":
                    case "na":
                        targetItem.notAmmo = Boolean.Parse(arg);
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
    }
}
