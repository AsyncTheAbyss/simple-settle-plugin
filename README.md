# Simple Settle
A tshock plugin that will settle liquids after a specified amount of time in the config. This plugin will also settle the liquids if there is a high amount of them.

### Where to find configuration
run the plugin for the first time to create the config file.  
under the server directory, in /tshock/SimpleSettle.json. 

### Configuration info
**Message**, shows the message to chat or not.  
**MessageOnlyInConsole**, overrides the message option and will only show the message in the console.  
**MaxSettle**, toggles the settling of the liquids if there is a high amount of them. (separate from the main settle timer)  
**MaxSetttleAmount**, the cap at which the high liquid settling will occur at.  
**SettleTime**, how often the settle timer will go off in seconds. (not including the high liquid settle)  
