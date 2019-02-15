#!/usr/bin/python

from phue import Bridge
import os
import logging

HUEBRIDGEIP = "192.168.178.79"
LIGHTNAME = "OnAir"

# Init hue
logging.basicConfig()

Bridge = Bridge(HUEBRIDGEIP)
Bridge.connect() # If the app is not registered and the button is not pressed, press the button and call connect() (this only needs to be run a single time)
Bridge.get_api() # Get the bridge state (This returns the full dictionary that you can explore)
light_names = Bridge.get_light_objects('name') # Get a dictionary with the light name as the key
LAMP = light_names[LIGHTNAME] # Get light object
LAMP.on = $STATE$ # True # $STATE$
LAMP.brightness = 254