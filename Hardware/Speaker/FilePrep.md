
## The speaker
The usage of the speaker is not difficult. The tricky part is the preperation.
*You can make use of two methods:*
1. `PlayAsync()`, which plays the preloaded file asynchronous to not disturb any other processes.
2. `Stop()`, which stops the async playing of the file.
**NOTE:** There is no pause option.
The files get preloaded on initialisation, this means it may take a while for the bot to continue. The preloading enables immediate playback.

### Preperation of the files. 
I suggest using a tool to convert audio files into 16-bit audio files. I personally use the tool ffmpeg which can be downloaded by typing in a command prompt `winget install ffmpeg`. (The tool might need to be added to the environment path.)

In the case of ffmpeg you can convert files with `ffmpeg -i [input filepath] -ac 1 -sample_fmt s16 [output.wav]`
*The used flags are follows:*
- `-i` is the input file
- `-ac` is the number of audiochannels
- `-sample_fmt` is the format of the files. in this case is s16, signed 16 bits
- There is no flag needed for the output file

### When the file is prepped. 
you have put the file on the bot, this can be done via file transfer or a usb stick.
I personally use a usb. Because its linux you'll have to manually mount the usb. 
*Please follow the following steps to mount your usb:*
1. Plug the usb into the rasperry pi
2. Launch powershell
3. Connect to the bot through ssh via the following command `ssh rompi@[The IP of your bot on the shared network, this can be found in the bot console] 
4. When connected type `lsblk` to find the location of the usb on the bot, in most cases its `sda1`
5. Make a map under the directory `/mnt/` named `usb`
6. Mount the usb with the command `sudo mount /dev/sda1 /mnt/usb
7. The location of the files is `/mnt/usb/[File path to the file on your usb]

### Final
When the usb is mounted and the 16-bit wav file is located you can use the class's start and stop function. 
Again: The files get preloaded on initialisation. This means it takes a while to continue.