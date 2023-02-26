using TerrariaApi.Server;
using Terraria;
using System.Timers;
using TShockAPI;

namespace PluginTemplate;

/// <summary>
/// The main plugin class should always be decorated with an ApiVersion attribute. The current API Version is 2.1
/// </summary>
[ApiVersion(2, 1)]
public class PluginTemplate : TerrariaPlugin
{
    /// <summary>
    /// The name of the plugin.
    /// </summary>
    public override string Name => "Simple Settle Plugin";

    /// <summary>
    /// The version of the plugin in its current state.
    /// </summary>
    public override Version Version => new Version(1, 0, 0);

    /// <summary>
    /// The author(s) of the plugin.
    /// </summary>
    public override string Author => "Donut313";

    /// <summary>
    /// A short, one-line, description of the plugin's purpose.
    /// </summary>
    public override string Description => "a simple plugin that settles all the liquid on the server every 5 minutes";
// timer variable
public static System.Timers.Timer aTimer;
    /// <summary>
    /// The plugin's constructor
    /// Set your plugin's order (optional) and any other constructor logic here
    /// </summary>
    public PluginTemplate(Main game) : base(game)
    {

    }

    /// <summary>
    /// Performs plugin initialization logic.
    /// Add your hooks, config file read/writes, etc here
    /// </summary>
    public override void Initialize()
    {
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
    }
    // boot up timer
        private void OnUpdate(EventArgs args)
        {
SetTimer();
        }
    /// <summary>
    /// Performs plugin cleanup logic
    /// Remove your hooks and perform general cleanup here
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            //unhook
            //dispose child objects
            //set large objects to null
        }
        base.Dispose(disposing);
    }
    // create timer for the liquid
    private static void SetTimer()
        {
            aTimer = new System.Timers.Timer(300000.0);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
                private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // settle liquid
Liquid.StartPanic();
// show message
TShock.Utils.Broadcast("Settling liquids.", 252, 211, 7);
        }
}
