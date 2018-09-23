using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CustomItems {
    [ApiVersion (2, 1)]
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
                HelpText = "/customitem <id> <parameters> <#> ... \nParameters: hexcolor, damage, knockback, useanimation, " +
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo."
            });
        }


        private void CustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Count();

            if (num == 0) {
                args.Player.SendErrorMessage("Invalid Syntax. /customitem <id> <parameters> <#> ... \nParameters: hexcolor, damage, knockback, useanimation, " +
                "usetime, shoot, shootspeed, width, height, scale, ammo, useammo, notammo.");
                return;
            }

            List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
            Item item = items[0];

            TSPlayer player = new TSPlayer(args.Player.Index);
            int itemIndex = Item.NewItem((int)player.X, (int)player.Y, item.width, item.height, item.type, item.maxStack);

            Item targetItem = Main.item[itemIndex];

            for (int index = 1; index < num; ++index) {
                string lower = parameters[index].ToLower();
                if (lower.Equals("hexcolor") && index + 1 < num) {
                    targetItem.color = new Color(int.Parse(args.Parameters[index + 1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                        int.Parse(args.Parameters[index + 1].Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                        int.Parse(args.Parameters[index + 1].Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                } else if ((lower.Equals("damage") || lower.Equals("d")) && index + 1 < num) {
                    targetItem.damage = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("knockback") || lower.Equals("kb")) && index + 1 < num) {
                    targetItem.knockBack = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("useanimation") || lower.Equals("ua")) && index + 1 < num) {
                    targetItem.useAnimation = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("usetime") || lower.Equals("ut")) && index + 1 < num) {
                    targetItem.useTime = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("shoot") || lower.Equals("s")) && index + 1 < num) {
                    targetItem.shoot = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("shootspeed") || lower.Equals("ss")) && index + 1 < num) {
                    targetItem.shootSpeed = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("width") || lower.Equals("w")) && index + 1 < num) {
                    targetItem.width = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("height") || lower.Equals("h")) && index + 1 < num) {
                    targetItem.height = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("scale") || lower.Equals("sc")) && index + 1 < num) {
                    targetItem.scale = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("ammo") || lower.Equals("a")) && index + 1 < num) {
                    targetItem.ammo = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("useammo") || lower.Equals("ua")) && index + 1 < num) {
                    targetItem.useAmmo = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("notammo") || lower.Equals("na")) && index + 1 < num) {
                    targetItem.notAmmo = Boolean.Parse(args.Parameters[index + 1]);
                }
                ++index;
            }

            TSPlayer.All.SendData(PacketTypes.TweakItem, "", itemIndex, 255, 63);
        }
    }
}
