# SteamTimelineIntegration

SteamTimelineIntegration is a mod for RUMBLE that automatically adds annotations to Steam's built-in recording feature.

Individual games, rounds, results, and more will shown on the timeline in order to make navigating your recording to save clips much easier.

Your games can also be shown as a list, displaying information like your opponent's name, the result, whether the game was client/host, and even client round wins.
From here you can easily click a game and be brought straight to it on the timeline, or right click and save the whole game as a clip right there!

Optionally, you can also enable markers to show each and every instance of damage on the timeline, so you can see exactly where the action happened!
Though be warned, this will make the timeline very cluttered

## Steam Recording Setup

Even if you have Steam's game recording enabled, it probably isn't recording your rumble.
Here's the full setup to get that working:


1. In Steam, click on Steam in the very upper lefthand corner of the window
2. Click "Settings" from the dropdown
3. In the list on the lefthand side of the settings window, click on "Game Recording".
   It should be near the bottom of the list, in the 3rd and final section
4. Select "Record in Background"
5. Scroll down to "Add game-specific settings here" and click the "Add Game" dropdown
6. Search for "SteamVR" and click it to add it above
7. Click the "Length" dropdown, and select "Do Not Record" in the popup
8. Repeat the above three steps for **ALL** VR overlays you might be using.
   Things like "OVR Toolkit", "OVR Advanced Settings", and others **ALL** need to
   be added to this list and set to "Do Not Record" in order for RUMBLE to be recorded.
9. Done! You may customize the rest of the recording settings in this window to your personal preference

After following these steps the next time you play RUMBLE, it should begin recording automatically!
You should be able to see the recording either:
1. On the RUMBLE page of your Steam library, scroll down and see the "Recordings and Screenshots" section on the right
2. By clicking "View" and then "Recordings & Screenshots" at the top of the Steam window
