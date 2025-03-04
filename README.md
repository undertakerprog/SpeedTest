### SpeedTest

## 📌 Content
1. [📖About the project](#about)
2. [🛠 Used technologies](#technologies)
3. [🔗 API Endpoints](#endpoints)
4. [🚀 Installation and launch](#installation)
5. [👤 Author](#author)

## <a id = "about"> 1. 📖 About the project </a>

SpeedTest is an application for checking the download speed to a specified or better server.

#### Functionality(backend)
- Server list management 
- Ping detection on the specified ip address or host
- Determining the location of the server and user
- Determining the download speed from the specified server or the best one

For more details on the functionality, refer to [Endpoints](#endpoints)

#### Functionality(desktop application)
- Determining the download speed (fast or default test)
- Selecting a list of servers
- Change speed unit
- A window for constantly checking the download and upload speed

## <a id = "technologies"> 2. 🛠 Technologies </a>

### Stack-technologies
- ASP .NET
- WPF
- Redis
- Swagger
- MSTest

## <a id = "endpoints"> 3. 🔗 Endpoints </a>

The main API endpoints are listed below.  
The full description is available in the Swagger documentation, which is generated automatically.  

📌 **Swagger Documentation**  
To view a detailed description of the API, go to Swagger:  
**`http://localhost:5252/swagger`** (port 5252 - default port specified in `appsettings.json`, if the program is running on a different port, specify the correct port in the Swagger URL)


### Service Location
- Endpoint to determine your location
```sh
GET http://localhost:5252/api/location/my-location
# Response example
{
  "latitude": your_latitude,
  "longitude": your_longitude,
  "country": "your_country",
  "city": "your_city",
  "query": "your_query"
}
```

- Endpoint to determine location for some host
```sh
GET http://localhost:5252/api/location/host-location?host=8.8.8.8
# Response example
{
  "latitude": 39.03,
  "longitude": -77.5,
  "country": "United States",
  "city": "Ashburn"
}
```

- Endpoint to determine closest provider for you
```sh
GET http://localhost:5252/api/location/closest
# Response example
{
  "message": "Closest server found",
  "server": {
    "country": "provider_country",
    "city": "provider_city",
    "host": "provider_host",
    "provider": "provider_name",
    "latitude": provider_latitude,
    "longitude": provider_longitude
  }
}
```

- Endpoint to determine the best provider for you(determination by range and ping)
```sh
GET http://localhost:5252/api/location/best-server
# Response example
{
  "message": "Best server found",
  "server": {
    "country": "provider_country",
    "city": "provider_city",
    "host": "provider_host",
    "provider": "provider_name",
    "latitude": provider_latitude,
    "longitude": provider_longitude
  }
}
```

- Endpoint for find all providers in your city or in chosen city
```sh
 # For your city
GET http://localhost:5252/api/location/servers-city-list
# For chosen city
GET http://localhost:5252/api/location/servers-city-list?city=city

# Response example for your city
[
  {
    "country": "your_country",
    "city": "your_city",
    "host": "provider_host1",
    "provider": "provider_name1",
    "latitude": provider_latitude1,
    "longitude": provider_longitude1
  },
  {
    "country": "your_country",
    "city": "your_city",
    "host": "provider_host2",
    "provider": "provider_name2",
    "latitude": provider_latitude2,
    "longitude": provider_longitude2
  },
  .......
]

# Sample response for the selected city
[
  {
    "country": "Germany",
    "city": "Berlin",
    "host": "warp.cronon.net",
    "provider": "Cronon AG",
    "latitude": 52.5225,
    "longitude": 13.3246
  },
  {
    "country": "Germany",
    "city": "Berlin",
    "host": "speedcheck-muc.kabeldeutschland.de",
    "provider": "Vodafone Kabel Deutschland",
    "latitude": 52.4651,
    "longitude": 13.3961
  },
  .......
]
```

### Service Ping

- Endpoint for check ping on chosen host
```sh
POST http://localhost:5252/api/ping/host-ping
#Request body
"8.8.8.8" 

# Response example
{
  "host": "8.8.8.8",
  "pingResult": 10
}
```

### Service Server

- Endpoint for get list of all servers
```sh
GET http://localhost:5252/api/server/list

# Response example
[
  {
    "country": "Russia",
    "city": "Sochi",
    "host": "62.182.8.78",
    "provider": "Aquafon GSM",
    "latitude": 43.5994,
    "longitude": 39.7289
  },
  {
    "country": "Russia",
    "city": "Orel-Izumrud",
    "host": "cyxym.net",
    "provider": "Systema, LTD",
    "latitude": 43.4562,
    "longitude": 39.923
  },
  .......
]
```

- Endpoint for add new server
```sh
POST http://localhost:5252/api/server/add

# Request body
"8.8.8.8" - (host or ip)

# Response example
Server added successfully(8.8.8.8).
Country: Ashburn
City: United States
```

- Endpoint for update some server
```sh
PUT http://localhost:5252/api/server/update

# Request body
{
  "oldHost": "8.8.8.8",
  "newHost": "1.1.1.1"
}

# Response example
Old host 8.8.8.8 update from 8.8.8.8 to 1.1.1.1
```

- Endpoint for delete some server by city or host
```sh
# if the deletion takes place by city, then one city with the same name is deleted, if there are several cities, then the host must be specified (deletion by host is recommended)
DELETE by city http://localhost:5252/api/server/delete-server?city=city

DELETE by host http://localhost:5252/api/server/delete-server?host=1.1.1.1

# Response examples
Server deleted successfully for city: city with host: any

Server deleted successfully for city: any with host: 1.1.1.1
```

- Endpoint for delete all servers by country
```sh
DELETE http://localhost:5252/api/server/delete-all

#Request body
"Country"

# Response example
All servers for country: 'Country' successfully deleted
```

### Service SpeedTest

- Endpoint for fast speedtest
```sh
GET http://localhost:5252/api/speedtest/fast-test
# Response example
{
  "speed": 31.667
}
```

- Endpoint for speedtest

```sh
# For best server
GET http://localhost:5252/api/speedtest/download-speed
# For chosen server
GET http://localhost:5252/api/speedtest/download-speed?host=host
# Response example
{
  "server": {
    "country": "your_country",
    "city": "your_city",
    "host": "provider_host",
    "provider": "provider_name",
    "latitude": provider_latitude,
    "longitude": provider_longitude
  },
  "speed": 100.201,
  "unit": "Mbps",
  "ping": 3,
  "source": "SpeedTestService"
}
```


## <a id = "installation"> 4. 🚀 Installation and launch </a>

To ensure proper functioning, Redis must be installed

In the Installation on Windows section, it's better to mention that Microsoft has discontinued support for Redis, and it's recommended to use WSL2 or Docker instead.

### 4.1 Installation on Windows
For Windows, it is recommended to install Redist.msi [GitHub for install Redis](https://github.com/microsoftarchive/redis/releases)

### 4.2 Installation on Linux

Follow the instructions in the terminal described on the [official site](https://redis.io/docs/latest/operate/oss_and_stack/install/install-redis/install-redis-on-linux/)



It is recommended to install the port corresponding to SpeedTest - 6379 in the installer
If port 6379 is busy, then in the file `appsettings.json`(path - `SpeedTest\Web\appsettings.json`) replace the port with the one specified during installation
```sh 
"ConnectionStrings": {
  "Redis": "localhost:YOUR_HOST"
}
```

### Requests to check the cache contents after launching the application

```sh
KEYS server-city:* #Getting all cached cities
```

```sh
GET server-city:your_city #Getting the contents of your_city
```

### 4.3 Configuring the Desktop application

If the Desktop application is not working, it may be due to a port mismatch, check which port the backend application is running on (by default it should work on 5252) and replace it in the Desktop file `Settings.settings` path `SpeedTest\Desktop\Properties\Settings.settings`

```sh

# Replace YOUR_PORT to the backend launch port

<Setting Name="SpeedTestUri" Type="System.String" Scope="Application">
      <Value Profile="(Default)">http://localhost:YOUR_PORT/api/SpeedTest/</Value>
    </Setting>
    <Setting Name="LocationUri" Type="System.String" Scope="Application">
      <Value Profile="(Default)">http://localhost:YOUR_PORT/api/Location/</Value>
    </Setting>
```

## <a id = "author"> 5. 👤 Author </a>
Author - [undertakerprog](https://github.com/undertakerprog)