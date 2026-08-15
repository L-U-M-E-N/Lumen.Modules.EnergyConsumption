from PyP100 import PyP110

import sys

###################
# https://github.com/fishbigger/TapoP100/pull/87
###################
import json
import logging
import time
import configparser
import datetime
import calendar
import requests
import pytz

config = configparser.ConfigParser()
config.read('config.ini');

midnight=(datetime.datetime.now(pytz.timezone("Europe/Paris")) \
            .replace(hour=0, minute=0, second=0, microsecond=0) \
            .astimezone(pytz.utc)
)
midnightTimestamp=calendar.timegm(midnight.timetuple())
print(midnightTimestamp)

_LOGGER = logging.getLogger(__name__)

class P110B(PyP110.P110):
    def getEnergyData(self, startT, endT, intervalT):
        return self.request("get_energy_data", { "end_timestamp": endT, "interval": intervalT, "start_timestamp": startT})

def formatTimestampAsIsoDate(date):
    return datetime.datetime.fromtimestamp(fromDate).astimezone(datetime.timezone.utc).isoformat().replace('+00:00', 'Z')

###################
ips = config['DEFAULT']['Ips'].split(',')
for ip in ips:
    p110 = P110B(ip, config['DEFAULT']['Username'], config['DEFAULT']['Password']) #Creating a P110 plug object

    p110.handshake() #Creates the cookies required for further methods
    p110.login() #Sends credentials to the plug and creates AES Key and IV for further methods

    #PyP110 has all PyP100 functions and additionally allows to query energy usage infos
    #print(p110.getEnergyUsage()) #Returns dict with all the energy usage
    #print(p110.getEnergyData(int(sys.argv[4]), int(sys.argv[5]), int(sys.argv[6])));

    entries=[]

    # Get yesterday's hourly data
    fromDate=midnightTimestamp-86400
    res = p110.getEnergyData(int(fromDate), int(fromDate+3600), int(60))

    for entry in res['data']:
        entries.append({
            'from': formatTimestampAsIsoDate(fromDate),
            'to': formatTimestampAsIsoDate(fromDate + 3600),
            'value': entry,
            'source': ('TAPO P110 - ' + ip)
        })
        fromDate+=3600

    # Get today's hourly data
    fromDate=midnightTimestamp
    res = p110.getEnergyData(int(fromDate), int(fromDate+3600), int(60))
    for entry in res['data']:
        entries.append({
            'from': formatTimestampAsIsoDate(fromDate),
            'to': formatTimestampAsIsoDate(fromDate + 3600),
            'value': entry,
            'source': ('TAPO P110 - ' + ip)
        })
        fromDate+=3600

    print(entries)

    response = requests.post(config['DEFAULT']['LumenUrl'] + '/Energy', json=entries, headers={
        'Content-Type': 'application/json',
        'X-Api-Key': config['DEFAULT']['LumenApiKey']
    })
    print(response)