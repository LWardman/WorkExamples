import json

from shaker.arduino.arduino_connect import ArduinoConnect
from dice_license.arduino_id import ArduinoID
from ..status import Status
from shaker.arduino.shaker_configuration_map import ShakerConfigurationFileMap
from shaker.arduino.packets.payloads.payload_base import Payload
from shaker.arduino.packets.payloads.payload_factory import determine_payload_class

import logging
logger = logging.getLogger(__name__)

class ArduinoConnectionCentre:
    def __init__(self):
        Status.add_listener(self.status_switch_delegate)
        self.validate_arduino_id()
        self.arduino_connection = ArduinoConnect(ArduinoID.get())
        self.shaker_configuration_map = ShakerConfigurationFileMap()
        self.load_shaker_configuration("empty")

    def status_switch_delegate(self, status: str):
        self.validate_arduino_id()
        match status:
            case "small shake":
                self.load_shaker_configuration("small")
                Status.update_status("shaking")
                return
            case "medium shake":
                self.load_shaker_configuration("medium")
                Status.update_status("shaking")
                return
            case "big shake":
                self.load_shaker_configuration("big")
                Status.update_status("shaking")
                return
            case "stop":
                self.load_shaker_configuration("empty")
                Status.update_status("idle")
                return

    def load_shaker_configuration(self, shake_type: str):
        config_file = self.shaker_configuration_map.get_path_from_shake_type(shake_type)

        # TODO: It's much better to cache this instead of constantly loading files at runtime
        config = None
        with open(config_file, "r") as file: # config_file is checked in shaker_configuration_map
            config = json.load(file)

        # This autoloads the payload type from the shakerconf variables, then sends the new payload to the arduino connection
        payload_class: type[Payload] = determine_payload_class(config)
        payload = payload_class(config)
        self.arduino_connection.set_payload(payload)

    def validate_arduino_id(self):
        if not ArduinoID.is_valid():
            raise EnvironmentError("Arduino ID not registered")
