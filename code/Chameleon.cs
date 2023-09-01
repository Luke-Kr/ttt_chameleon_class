using Sandbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrorTown;
using TTT_Classes;

namespace TTT_Classes
{
    public partial class Chameleon : TTT_Class
    {
        public override string Name { get; set; } = "Chameleon";
        public override string Description { get; set; } = "You are a master of camouflage! Use your active ability to turn invisible but harmless.";
        public override float Frequency { get; set; } = 1f;
        public override Color Color { get; set; } = Color.FromRgb(0x107C10);
        public override bool hasActiveAbility { get; set; } = true;
        public override float coolDownTimer { get; set; } =  60f;
        public override float buttonDownDuration { get; set; } = 1f;
        public override bool hasDuration { get; set; } = true;
        public override float Duration { get; set; } = 10f;
        private bool Active { get; set; } = false;
        private RealTimeSince SetActive { get; set; }
        private IList<Entity> Items { get; set; } = new List<Entity>();
        private int MaxItems { get; set; } = 1;


        [ClientRpc]
        public void SetHolsterdClient()
        {
            Entity.Inventory.SetActiveSlot(0);
        }

        public override void ActiveAbility()
        {
            Active = true;
            SetActive = 0;
            Entity.EnableDrawing = false;
           
            foreach (Entity item in new List<Entity>(Entity.Inventory.Items))
            {
                if (!(item.GetType() == typeof(Holstered)))
                {
                    Items.Add(item);
                    Entity.Inventory.Items.Remove(item);
                }
            }
            Entity.Inventory.SetActiveSlot(0);
            SetHolsterdClient();
            (MaxItems, Entity.Inventory.MaxItems) = (Entity.Inventory.MaxItems, MaxItems);
        }

        [GameEvent.Tick.Server]
        public void DoDuration()
        {
            if (Active && SetActive > Duration)
            {
                Active = false;
                Entity.EnableDrawing = true;

                // If someone altered the items of the player, assume it is permanent and respect that
                if (Entity.Inventory.Items.Count > 1 || Entity.Inventory.MaxItems > 1) return;

                (MaxItems, Entity.Inventory.MaxItems) = (Entity.Inventory.MaxItems, MaxItems);
                foreach (Entity item in new List<Entity>(Items))
                {
                    if (!(item.GetType() == typeof(Holstered)))
                    {
                        Items.Remove(item);
                        Entity.Inventory.AddItem(item);
                    }
                }
            }
        }
    }
}
