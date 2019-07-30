# Neon.HomeControl
Similar to HomeAssistant, but made with .net core and ❤️


# Help request!
I'm looking for people to help me with the project, please contact me!


# Actual implemented plugins
- Weather (Dark sky API)
- Spotify API
- Sonoff-Tasmoda
- Philip Hue
- Panasonic Air Conditioner API
- Own tracks (via MQTT)
- MQTT Client
- ~~Nest Thermo (disabled, because the api are changing)~~
- Chromecast and SSDP media player (thanks @kakone)

# Features
 - .NET Core 2.2 
 - Scripts Engine (for make rules): *LUA*
 ~~- Events Database: *LiteDB*~~
 - Events Database: NoSQL Connectors (LiteDB and MongoDb)
 - Classic Database (can change in future): *Sqlite*

# !!! NOTE !!!
Rename config `neon.settings-default.json` to `neon.settings.json` before start application



# Simple event system
You can catch events in two different ways in LUA:

_First Method_

```lua
add_rule("test_rule", "Weather", "entity.Temperature > 30", function(entity) 
     log_info("test", "It's hot!")
end)
```

_Second Method_

```lua
on_event_name("weather", function(entity)
  if entity.Temperature >= 30 then
      log_info("test", "Temperature is {0} and is hot!", entity.Temperature);
  else
      log_info("test", "Temperature is {0} and is mid hot!", entity.Temperature);
  end
end
)
```

# Simple commands system:

```lua
on_event_name("weather", function(entity)

  if entity.Temperature >= 30 then
      log_info("test", "Temperature is {0}", entity.Temperature);
      local airco_entity = cast_entity(get_entity_by_name("airco"))
      send_command(airco, "POWER", "ON")
  else      
end
```

# Alarm system:

```lua
add_alarm("test_alarm", 07,32, function()
  log_info("test", "It's time to wake up!");
end
)
```