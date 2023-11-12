using TerrariaApi.Server;
using Terraria;
using System.Timers;
using TShockAPI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace SimpleSettle;

// set up config manager
public class ConfigManager {
    public bool Message = false;
    public bool MessageOnlyInConsole = true;
    public bool MaxSettle = true;
    public int MaxSetttleAmount = 10000;
    public int SettleTime = 300;
    public string settlemessage = "Settling liquids.";
    public string settlehighmessage = "Settling liquids due to high amount of liquids.";
    public void write()
    {
        File.WriteAllText(SimpleSettle.path,JsonConvert.SerializeObject(this,Formatting.Indented));
    }
    public ConfigManager read() {
        // check if our config does not exist
        if (!File.Exists(SimpleSettle.path))
        {
            return new ConfigManager();
        }
        return JsonConvert.DeserializeObject<ConfigManager>(File.ReadAllText(SimpleSettle.path));
    }
}
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
    // config setup 
    public static string path = Path.Combine(TShock.SavePath, "SimpleSettle.json");
    public static ConfigManager config = new ConfigManager();
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
        // set up config
        GeneralHooks.ReloadEvent += OnReload;
        if (File.Exists(path))
            config = config.read();
        else 
            config.write();
        ServerApi.Hooks.GamePostInitialize.Register(this, LoadUpTimers);
    }
    private void OnReload(ReloadEventArgs args)        
    {
        if (File.Exists(path))
             config = config.read();
        else 
            config.write();
        // restart the timers to apply altered time
        DestroyTimers();
        SetLiquidTimer();
        if (config.MaxSettle)
        {
        SetHighLiquidTimer();
        }
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
            ServerApi.Hooks.GamePostInitialize.Deregister(this, LoadUpTimers);
            // destroy timers
            DestroyTimers();
        }
        base.Dispose(disposing);
    }

    // create timer for the liquid
    private static bool HasSettledWithNoneOn = false;
    private void SetLiquidTimer()
    {
        liquidTimer = new System.Threading.Timer(SettleLiquids, null, 1000 * config.SettleTime, 1000 * config.SettleTime);
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
            // set the bool to false because someone is active on the server
            HasSettledWithNoneOn = false;
        }
        else {
            // set the bool to true now that liquids have been settled with no one on the server
            HasSettledWithNoneOn = true;
        }
            // check if there is literally anything to settle
            if (liquidCount > 100 && !HasSettledWithNoneOn)
            {
                // settle liquid
                Liquid.StartPanic();
                // show message
                if (config.Message && !config.MessageOnlyInConsole)
                {
                TShockAPI.TSPlayer.All.SendInfoMessage(config.settlemessage);
                }
                if (config.MessageOnlyInConsole)
                {
                    Console.WriteLine(config.settlemessage);
                }
                
            }
        
                    
    }

    private void SettleLiquidsHigh(object state)
    {
        // calculate liquid count
        int liquidCount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
            if (liquidCount > config.MaxSetttleAmount)
            {
                // settle liquid
                Liquid.StartPanic();
                // show message
                if (config.Message && !config.MessageOnlyInConsole)
                {
                TShockAPI.TSPlayer.All.SendInfoMessage(config.settlehighmessage);
                }
                if (config.MessageOnlyInConsole)
                {
                Console.WriteLine(config.settlehighmessage);
                }
            }
    }

    private void LoadUpTimers(EventArgs args)
    {
        SetLiquidTimer();
        if (config.MaxSettle)
        {
        SetHighLiquidTimer();
        }
    }
    private void DestroyTimers()
    {
        liquidTimer.Dispose();
        highLiquidTimer.Dispose();
        liquidTimer = null;
        highLiquidTimer = null;
    }
}
