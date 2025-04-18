using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace KidAutoPetter
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            this.Monitor.Log($"KidAutoPetter loaded successfully. :)", LogLevel.Info);
            helper.Events.GameLoop.DayStarted += this.KidAutoPetter;
        }


        /******
         ** Private Methods
         ******/

        /// <summary>
        /// This function is called upon starting a new Day.
        /// It checks if the player has any children and if so, uses them to Auto-Pet all FarmAnimals including the Pet Dog or Cat.
        /// The mods capability is disabled in Multiplayer Sessions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KidAutoPetter(object? sender, DayStartedEventArgs e)
        { 
            if (Game1.IsMultiplayer) return;
            if (Game1.player.getChildrenCount() > 0 && Game1.player.IsMainPlayer)
            {
                this.Monitor.Log($"Main Player has children", LogLevel.Debug);
                if (Game1.player.hasPet())
                {
                    Pet pet = Game1.player.getPet();
                    //this.Monitor.Log($"Player {Game1.player.Name} has a pet {pet.Name}", LogLevel.Debug);
                    pet.grantedFriendshipForPet.Set(newValue: true);
                    pet.friendshipTowardFarmer.Set(Math.Min(1000, pet.friendshipTowardFarmer.Value + 12));
                }

                foreach (FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
                {
                    //this.Monitor.Log("Found Animal", LogLevel.Debug);
                    //this.Monitor.Log($"{animal.Name}", LogLevel.Debug);
                    animal.pet(Game1.player);
                }

                Game1.showGlobalMessage(I18n.GetByKey("petted"));
            }
        }
    }
}