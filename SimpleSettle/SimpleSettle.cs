using TerrariaApi.Server;
using Terraria;
using System.Timers;
using TShockAPI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace SimpleSettle;

/// <summary>
/// Saved config values, config writing and reading
/// </summary>
public class ConfigManager {
  public bool Message = false;
  public bool MessageOnlyInConsole = true;
  public bool MaxSettle = true;
  public int MaxSettleAmount = 10000;
  public int SettleTime = 300;
  public string SettleMessage = "Settling liquids.";
  public string SettleHighMessage = "Settling liquids due to high amount of liquids.";
  public void write() {
    File.WriteAllText(SimpleSettle.path, JsonConvert.SerializeObject(this, Formatting.Indented));
  }
  public ConfigManager read() {
    if (!File.Exists(SimpleSettle.path)) {
      return new ConfigManager();
    }
    return JsonConvert.DeserializeObject < ConfigManager > (File.ReadAllText(SimpleSettle.path));
  }
}
[ApiVersion(2, 1)]
public class SimpleSettle: TerrariaPlugin {
  public override string Name => "Simple Settle";
  public override Version Version => new Version(1, 0, 1);
  public override string Author => "Async Void";
  public override string Description => "a simple plugin that settles all the liquid on the server every 5 minutes";
  public static string path = Path.Combine(TShock.SavePath, "SimpleSettle.json");
  public static ConfigManager config = new ConfigManager();
  private System.Threading.Timer liquidTimer;
  private System.Threading.Timer highLiquidTimer;
  public SimpleSettle(Main game): base(game) {}
  /// <summary>
  /// Creates hooks, starts up the config, and starts up the timers
  /// </summary>
  public override void Initialize() {
    GeneralHooks.ReloadEvent += OnReload;
    if (File.Exists(path))
      config = config.read();
    else
      config.write();
    ServerApi.Hooks.GamePostInitialize.Register(this, LoadUpTimers);
  }
  /// <summary>
  /// Reloads config, restarts timers
  /// </summary>
  private void OnReload(ReloadEventArgs args) {
    if (File.Exists(path))
      config = config.read();
    else
      config.write();
    liquidTimer.Dispose();
    highLiquidTimer.Dispose();
    liquidTimer = new System.Threading.Timer(SettleLiquids, null, 1000 * config.SettleTime, 1000 * config.SettleTime);
    if (config.MaxSettle) {
      highLiquidTimer = new System.Threading.Timer(SettleLiquidsHigh, null, 30000, 30000);
    }
  }
  /// <summary>
  /// Destroys timers, unhooks, and cleans up unneeded values
  /// </summary>
  protected override void Dispose(bool disposing) {
    if (disposing) {
      ServerApi.Hooks.GamePostInitialize.Deregister(this, LoadUpTimers);
      liquidTimer.Dispose();
      highLiquidTimer.Dispose();
    }
    base.Dispose(disposing);
  }
  private static bool HasSettledWithNoneOn = false;
  /// <summary>
  /// Settle liquids, Some logic to prevent unneeded settling
  /// </summary>
  private void SettleLiquids(object state) {
    int liquidCount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
    if (TShock.Players[0] == null) {
      HasSettledWithNoneOn = false;
    } else {
      HasSettledWithNoneOn = true;
    }
    if (liquidCount > 100 && !HasSettledWithNoneOn) {
      Liquid.StartPanic();
      if (config.Message && !config.MessageOnlyInConsole) {
        TShockAPI.TSPlayer.All.SendInfoMessage(config.SettleMessage);
      }
      if (config.MessageOnlyInConsole) {
        Console.WriteLine(config.SettleMessage);
      }
    }
  }
  /// <summary>
  /// Check if liquids amount is high And settle if so
  /// </summary>
  private void SettleLiquidsHigh(object state) {
    int LiquidCount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
    if (LiquidCount > config.MaxSettleAmount) {
      Liquid.StartPanic();
      if (config.Message && !config.MessageOnlyInConsole) {
        TShockAPI.TSPlayer.All.SendInfoMessage(config.SettleHighMessage);
      }
      if (config.MessageOnlyInConsole) {
        Console.WriteLine(config.SettleHighMessage);
      }
    }
  }
  /// <summary>
  /// Create Timers
  /// </summary>
  private void LoadUpTimers(EventArgs args) {
    liquidTimer = new System.Threading.Timer(SettleLiquids, null, 1000 * config.SettleTime, 1000 * config.SettleTime);
    if (config.MaxSettle) {
      highLiquidTimer = new System.Threading.Timer(SettleLiquidsHigh, null, 30000, 30000);
    }
  }
}
