# PIA-random-reconnect

## Usage

Download the repository and build the solution. Run `Reconnector.exe`.

## Configuration (`appsettings.json`)

*NOTE*: you can get the list of region names by running `piactl.exe get regions`

- Adjust (if different) your path to private internet access folder
- Adjust your list of servers you wish to use

example:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "PIA": {
    "PiaCtlPath": "C:\\Program Files\\Private Internet Access",
    "ValidServers": [
      "uk-streaming-optimized",
      "uk-london",
      "uk-southampton",
      "uk-manchester",
      "fi-helsinki",
      "fi-streaming-optimized",
      "georgia",
      "de-berlin",
      "de-frankfurt",
      "armenia",
      "switzerland",
      "estonia",
      "belgium",
      "monaco",
      "poland",
      "it-milano",
      "it-streaming-optimized",
      "netherlands",
      "norway",
      "romania",
      "se-streaming-optimized",
      "se-stockholm",
      "slovakia",
      "ukraine",
      "es-madrid",
      "es-valencia",
      "france",
      "bulgaria",
      "isle-of-man",
      "luxembourg",
      "serbia",
      "bosnia-and-herzegovina",
      "lithuania",
      "austria",
      "czech-republic",
      "dk-copenhagen",
      "dk-streaming-optimized",
      "malta",
      "nigeria",
      "kazakhstan",
      "liechtenstein",
      "north-macedonia",
      "algeria",
      "moldova",
      "ireland",
      "montenegro",
      "slovenia",
      "portugal",
      "egypt",
      "albania",
      "hungary",
      "latvia",
      "cyprus",
      "morocco",
      "turkey"
    ]
  }
}
```