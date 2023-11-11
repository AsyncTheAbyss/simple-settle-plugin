using TerrariaApi.Server;
using Terraria;
using System.Timers;
using TShockAPI;
using System.Threading.Tasks;

namespace SimpleSettle;

/// <summary>
/// The main plugin class should always be decorated with an ApiVersion attribute. The current API Version is 2.1
/// </summary>
[ApiVersion(2, 1)]
public class SimpleSettle : TerrariaPlugin
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

    // timer variables
    private System.Threading.Timer liquidTimer;
    private System.Threading.Timer highLiquidTimer;

    /// <summary>
    /// The plugin's constructor
    /// Set your plugin's order (optional) and any other constructor logic here
    /// </summary>
    public SimpleSettle(Main game) : base(game)
    {
    }

    /// <summary>
    /// Performs plugin initialization logic.
    /// Add your hooks, config file read/writes, etc here
    /// </summary>
    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, LoadUpTimer);
    }

    /// <summary>
    /// Performs plugin cleanup logic
    /// Remove your hooks and perform general cleanup here
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // unhook
            ServerApi.Hooks.GamePostInitialize.Deregister(this, LoadUpTimer);
            // dispose child objects
            liquidTimer.Dispose();
            highLiquidTimer.Dispose();
            // set large objects to null
            liquidTimer = null;
            highLiquidTimer = null;
        }
        base.Dispose(disposing);
    }

    // create timer for the liquid
    private static bool hassettledwithnooneon = false;
    public static bool didliquids = false;

    private void SetLiquidTimer()
    {
        liquidTimer = new System.Threading.Timer(SettleLiquids, null, 300000, 300000);
    }

    private void SetHighLiquidTimer()
    {
        highLiquidTimer = new System.Threading.Timer(SettleLiquidsHigh, null, 30000, 30000);
    }

    private void SettleLiquids(object state)
    {
        // calculate liquid count
        int liquidCount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;

        // check if a player is active
        if (TShock.Players[0].Active)
        {
            // check if there is literally anything to settle
            if (liquidCount > 100)
            {
                // settle liquid
                Liquid.StartPanic();
                // show message
                TShockAPI.TSPlayer.All.SendInfoMessage("Settling liquids.");
                // set the bool to false because someone is active on the server
                hassettledwithnooneon = false;
            }
        }
        else
        {
            // check if it has settled with no one on the server
            if (!hassettledwithnooneon)
            {
                // check if there is literally anything to settle
                if (liquidCount > 100)
                {
                    // settle liquid
                    Liquid.StartPanic();
                    // show message
                    TShockAPI.TSPlayer.All.SendInfoMessage("Settling liquids.");
                    // set the bool to true now that liquids have been settled with no one on the server
                    hassettledwithnooneon = true;
                }
            }
        }
    }

    private void SettleLiquidsHigh(object state)
    {
        // calculate liquid count
        int liquidCount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
            if (liquidCount > 10000)
            {
                // settle liquid
                Liquid.StartPanic();
                // show message
                TShockAPI.TSPlayer.All.SendInfoMessage("Settling liquids due to high amount of liquids.");
            }
    }

    private void LoadUpTimer(EventArgs args)
    {
        SetLiquidTimer();
        SetHighLiquidTimer();
    }
}
