using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace KidAutoPetter
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private ModConfig _config;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            _config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += this.KidAutoPetter;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            this.Monitor.Log($"KidAutoPetter loaded successfully. :)", LogLevel.Info);
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
                if (Game1.player.getChildren()[0].isInCrib())
                {
                    return;
                }

                //this.Monitor.Log($"Main Player has children", LogLevel.Debug);
                if (Game1.player.hasPet() && _config.petCatOrDog)
                {
                    Pet pet = Game1.player.getPet();
                    //this.Monitor.Log($"Player {Game1.player.Name} has a pet {pet.Name}", LogLevel.Debug);
                    pet.grantedFriendshipForPet.Set(newValue: true);
                    pet.friendshipTowardFarmer.Set(Math.Min(1000, pet.friendshipTowardFarmer.Value + 12));
                }
                if (_config.petFarmAnimals)
                {
                    foreach (FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
                    {
                        //this.Monitor.Log("Found Animal", LogLevel.Debug);
                        //this.Monitor.Log($"{animal.Name}", LogLevel.Debug);
                        animal.pet(Game1.player);
                    }
                }

                if (_config.enableMessage && (_config.petFarmAnimals || _config.petCatOrDog))
                {
                    Game1.showGlobalMessage(I18n.GetByKey("petted"));
                }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                //this.Monitor.Log("Config Menu not found", LogLevel.Debug);
                return;
            }
            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Helper.ReadConfig<ModConfig>(),
                save: () => this.Helper.WriteConfig(this._config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Text Message on Day start",
                tooltip: () => "This enables or disables the Info Message popping up on day start.",
                getValue: () => this._config.enableMessage,
                setValue: value => this._config.enableMessage = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Pet Cat or Dog",
                tooltip: () => "This enables or disables the Auto-Petting of the Pet Cat or Dog.",
                getValue: () => this._config.petCatOrDog,
                setValue: value => this._config.petCatOrDog = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Pet Farm Animals",
                tooltip: () => "This enables or disables the Auto-Petting of all Farm Animals.",
                getValue: () => this._config.petFarmAnimals,
                setValue: value => this._config.petFarmAnimals = value
            );

            this.Monitor.Log("Setup and Configured GenericModConfigMenu Integration", LogLevel.Info);
        }
    }
}