Setup Guide
===========

Before setting up any services, please read a summary of what each services provide and their cost.

<h3><a href="#pushovernet-1">Pushover.net</a></h3>
* Supported Operating Systems: <a href="https://pushover.net/clients">iOS, Android, Desktop</a>
* Allowance: 7,500 notifications/month
* Cost: First 5 days free, After $5 one time fee, per operating system (iOS, Android, Desktop/3rd Party Applications)

<h3><a href="#notifymyandroid-1">NotifyMyAndroid</a></h3>
* Supported Operating Systems: <a href="https://play.google.com/store/apps/details?id=com.usk.app.notifymyandroid">Android</a>
* Allowance: Free (5/day), Paid (800/hour)
* Cost: $5 one time fee

<h3><a href="#growl-1">Growl</a></h3>
* Supported Operating Systems: <a href="https://play.google.com/store/apps/details?id=com.growlforandroid.client&hl=en">Android</a>, <a href="http://www.prowlapp.com/">iOS</a>, <a href="http://www.growl.info">Mac</a>, <a href="http://www.growlforwindows.com/">Windows</a>)
* Allowance: No Limit
* Cost: Depends on application (Android: Free, Windows: Free, iOS: $2.99, Mac: $3.99)
* Note: Local notifications only via IP Address to the devices.

Pushover.net
==================
* Signup for <a href="http://www.pushover.net">Pushover.net</a> if you have yet to do so
* Choosing a client:
  * You can choose a client out of their <a href="https://pushover.net/clients">client list</a>
    * You can also signup via their clients
* Getting your User Token:
  * Your token should be at the top right hand side of the screen when you first login.
* Getting the Application Token:
  * Click on <a href="https://pushover.net/apps/build">Register an Application</a>
  * Fill out the following:
    * Name: Gotta Hunt Em All
    * Type: Plugin
    * Description: FFXIV Hunt Plugin
    * URL: You can choose to link to the Github, or leave it blank
    * Icon: Use this <a href="https://raw.githubusercontent.com/KnightAR/ffxivapp-plugin-knightar-hunt-em-all/master/distribution/Logo.png">Logo</a>
  * Check the accept checkbox, and "create application"
  * Your API Token/Key should be located at the top of the page

NotifyMyAndroid
==================
* Signup for <a href="http://www.notifymyandroid.com">NotifyMyAndroid</a> if you have yet to do so
* Download and install the <a href="https://play.google.com/store/apps/details?id=com.usk.app.notifymyandroid">Android Application</a>
* Login on the website, click on "Generate New Key", Click on "Key Description" and rename it to "Gotta Hunt Em All"
* Enter the key into the plugin.

Growl
==================
Growl is a little more complicated as Growl is a local only protocol. There are two ways the protocol is supported, Install the application on the same machine as FFXIVAPP and there is no advanced configuration. If you wish to notify other things such as Tablets, Phones, Other computerson the local network; you will be required to know the IP address of your clients you want to notify.

Lets start with the local machine, Since FFXIVAPP is a Windows based application we'll do a Windows Guide.
* Install <a href="http://www.growlforwindows.com/">Growl for Windows</a>.
* Once installed, Open Growl and make sure "Require password for local apps" is unchecked in the Security tab.
* Open FFXIV App, go to the Growl settings, and click "Register Application", now click "Send Test Message" to make sure growl is getting your messages.

Growl is setup on Windows!

If there's a need to go more in depth on configuring Growl for network devices, You can try to google the information.