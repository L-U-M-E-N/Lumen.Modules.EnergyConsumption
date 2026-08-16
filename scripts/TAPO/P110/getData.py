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
config.read('/home/pi/P110/config.ini');

midnight=(datetime.datetime.now(pytz.timezone("Europe/Paris")) \
            .replace(hour=0, minute=0, second=0, microsecond=0) \
)
midnightTimestamp=calendar.timegm(midnight.astimezone(pytz.utc).timetuple())
print(midnightTimestamp)

firstDayOfTheMonth=midnight.replace(day=1, hour=0, minute=0, second=0, microsecond=0)
firstDayOfTheMonthTimestamp=calendar.timegm(firstDayOfTheMonth.astimezone(pytz.utc).timetuple())
print(firstDayOfTheMonthTimestamp)

firstDayOfPreviousTheMonth=(firstDayOfTheMonth - datetime.timedelta(days=10)).replace(day=1, hour=0, minute=0, second=0, microsecond=0)
firstDayOfPreviousTheMonthTimestamp=calendar.timegm(firstDayOfPreviousTheMonth.astimezone(pytz.utc).timetuple())
print(firstDayOfPreviousTheMonthTimestamp)

firstDayOfTheYear=midnight.replace(month=1, day=1, hour=0, minute=0, second=0, microsecond=0)
firstDayOfTheYearTimestamp=calendar.timegm(firstDayOfTheYear.astimezone(pytz.utc).timetuple())
print(firstDayOfTheYearTimestamp)

firstDayOfThePreviousYear=(firstDayOfTheYear - datetime.timedelta(days=10)).replace(month=1, day=1, hour=0, minute=0, second=0, microsecond=0)
firstDayOfThePreviousYearTimestamp=calendar.timegm(firstDayOfThePreviousYear.astimezone(pytz.utc).timetuple())
print(firstDayOfThePreviousYearTimestamp)

_LOGGER = logging.getLogger(__name__)

class P110B(PyP110.P110):
    def getEnergyData(self, startT, endT, intervalT):
        return self.request("get_energy_data", { "end_timestamp": endT, "interval": intervalT, "start_timestamp": startT})

def formatTimestampAsIsoDate(date):
    return datetime.datetime.fromtimestamp(date).astimezone(datetime.timezone.utc).isoformat().replace('+00:00', 'Z')

def mapEntries(data, fromDate, toDate, timeStampInterval):
    for entry in data:
        entries.append({
            'from': formatTimestampAsIsoDate(fromDate),
            'to': formatTimestampAsIsoDate(fromDate + timeStampInterval),
            'value': entry,
            'source': ('TAPO P110 - ' + ip)
        })
        fromDate+=timeStampInterval
        if fromDate >= toDate:
            break

def getEnergyEntries(fromDate, toDate, minutesInerval):
    res = p110.getEnergyData(int(fromDate), int(toDate), int(minutesInerval))
    mapEntries(res['data'], fromDate, toDate, minutesInerval*60) 

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
    getEnergyEntries(
        midnightTimestamp-86400,
        midnightTimestamp-86400+3600*24,
        60
    )

    # Get today's hourly data
    getEnergyEntries(
        midnightTimestamp,
        midnightTimestamp+3600*24,
        60
    )

    # Get the previous month's data
    getEnergyEntries(
        firstDayOfPreviousTheMonthTimestamp,
        firstDayOfPreviousTheMonthTimestamp+3600*24*31,
        60*24
    )

    # Get this month's data
    getEnergyEntries(
        firstDayOfTheMonthTimestamp,
        firstDayOfTheMonthTimestamp+3600*24*31,
        60*24
    )

    # Get last year's data
    #getEnergyEntries(
    #    firstDayOfThePreviousYearTimestamp,
    #    firstDayOfThePreviousYearTimestamp+3600*24*365,
    #    60*24*30
    #)

    # Get this year's data
    #getEnergyEntries(
    #    firstDayOfTheYearTimestamp,
    #    firstDayOfTheYearTimestamp+3600*24*365,
    #    60*24*30
    #)

    print(entries)

    response = requests.post(config['DEFAULT']['LumenUrl'] + '/Energy', json=entries, headers={
        'Content-Type': 'application/json',
        'X-Api-Key': config['DEFAULT']['LumenApiKey']
    })
    print(response)

