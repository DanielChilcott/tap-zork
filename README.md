# tap-zork
Text adventure games are a lot of fun but are awkward to play on mobile phones due to the amount of typing and the fact that on a touch display, the soft keyboard takes up half of the screen.

Tap Zork offers a responsive web UI where users can tap words inside responses (eg. 'troll' or 'sword') as well as select words from a verb palette (eg. 'take' or 'attack') to build up a command. Navigation options are also presented as buttons for faster navigation.

The concept could be adapted to support any text-only ZMachine game (up to v5 I think) and with some more work to allow users to customise their own word palette, this could provide a great way to make these games more playable on a mobile device.


Acknowledgements
----------------

The .NET Z-Machine used in this project is taken from http://zmachine.codeplex.com. Please note that that although Tap Zork carries an MIT license, the ZMachine project has a custom license.


To do's and Known Issues
------------------------

* I've come across three potential  bugs in the ZMachine implementation (although theremay be fewer root causes) and given that it is an entire virtual machine it will take some siginficant effort to understand root cause. The simplest to reproduce is to go to the troll room and then west through the forbidding hole. Then go west again four times and an out of range exception will be thrown. This problem occurs in both minizork and zork.

* Ability to customise word palette by allowing users to type their own words and then optionally add them to the palette

* An always up to date inventory (could be returned by the game API after each command is executed). At the moment the issue is that if you need something in haste, you have to request inventory to be able to tap it.

* An always up to date score to be presented

* Control over 3 or 5 game save slots so you can roll back to them easily

* More intuitive UI for restoring an existing game or starting a new game
