# Client-side event tracking for Sitecore 9.x

XdbTracker is a simple API that allows you to trigger events / goals and outcomes through Javascript. The events are automatically associated to the user's current session and current page.

## Usage

``` 
// Trigger built-in "login" goal
xdbTracker.triggerEvent('{66722F52-2D13-4DCC-90FC-EA7117CF2298}');  

// Trigger built-in search event with search term as data
xdbTracker.triggerEvent('{0C179613-2073-41AB-992E-027D03D523BF}', 'my search term');

// Trigger the "Product Purchase" outcome with a monetary value
xdbTracker.triggerOutcome('{9016E456-95CB-42E9-AD58-997D6D77AE83}', 'USD', '9.99');
```
Usage is simple:

 - Create and deploy a PageEvent or Goal in Sitecore and copy it's ID
 - Trigger the event through JS using the ID as seen above

## Setup

- Reference track.min.js in your layout (its only ~380bytes) 
	```<script src="/sitecore modules/web/xdbtracker/track.min.js"></script> ```
- Copy binaries and files from the [Release ZIP](https://github.com/lowedown/XdbTracker/releases/latest) folder to your webroot or integrate them into your build


## Technical details
The tiny track.min.js will POST events to the track handler /sitecore modules/web/xdbtracker/track.ashx as they are triggered. Track.ashx is session-aware and will automatically assign the events and outcomes to the current interaction and page.
Check the source code for more details.

## Compatibility
XdbTracker has been tested with Sitecore 9.0, 9.1 and 9.2. It might also work on 8.x but that has not been tested.

## FAQ
### tracker.ashx is returning "Please provide a valid EventId"
You have probably not deployed your goal/event or you have used a wrong ID.
