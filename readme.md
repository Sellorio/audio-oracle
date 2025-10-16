# Audio Oracle

A hosted app used to add albums to a local music library from YouTube, MusicBrainz (metadata only) and SoundCloud
(download source only).

## What is in the box

The main app is an all-inclusive docker image which provides a web interface, database management and any other
prerequsites except for the `cookies-youtube.txt` file. This file contains an export of YouTube cookies needed
to interact with the API and download most tracks.

This is where AudioOracleCompanion comes in. This is a windows desktop application which hosts a browser window.
If you log in to YouTube or YouTube Music and press the start button, this app will export YouTube cookies and
save them to the folder defined in `config.yml`. The app will re-export cookies every 30 minutes to ensure
cookies remain valid and up-to-date.

## How to use

1. Run the `publish.ps1` script. This will compile AudioOracleCompanion into `./publish/AudioOracleCompanion` and
build and export the AudioOracle docker image to `./publish/audio-oracle.tar`.

You will need the appropriate dotnet sdks installed to compile AudioOracleCompanion.

2. Then copy the docker image to your server, run `docker load -i audio-oracle.yar` to import the image

3. Update your docker-compose file to something like this:

```yaml
audio-oracle:
  image: audio-oracle:latest
  container_name: audio-oracle
  user: root
  ports:
    - 30123:80
  environment:
    - ASPNETCORE_HTTP_PORT=80
    - AO_PASSWORD=<yourpasswordhere>
  volumes:
    - ./Music:/music
    - ./data/audio-oracle:/data
  restart: unless-stopped
```

4. Update the config.yml in AudioOracleCompanion to point to your Audio Oracle data folder
5. Run AudioOracleCompanion, log in to YouTube, select your account (if applicable) and press the start button

Note, you will need a Premium subscription for many YouTube downloads and your metadata may auto redirect to
the video version of some music if you are not a subscriber.
