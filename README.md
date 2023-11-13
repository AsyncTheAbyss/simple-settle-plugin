# Simple Settle Plugin
A tshock plugin that will settle liquids after a specified amount of time in the config. This plugin will also settle the liquids if there is a high amount of them.

# Configuration creation and where to find
After running the plugin for the first time a config file will be created under the server directory, in tshock and callled SimpleSettle.json. 

# Configuration info
Message means if it will show the message to all players or not.

MessageOnlyInConsole if this is turned on it will override the Message option in the config and will only show the message in the console.

MaxSettle will toggle the settling if the liquids are high or not.

MaxSetttleAmount is the cap at which the high liquid settling will occur at.

SettleTime is how often the settle timer will go off in seconds. (not including the high liquid settle)
