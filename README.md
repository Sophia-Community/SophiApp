# SophiApp. The next chapter of the [Sophia Script](https://github.com/farag2/Sophia-Script-for-Windows) project

<img src="https://raw.githubusercontent.com/Sophia-Community/SophiApp/master/img/sophiapp_big.svg" alt="Sophia Script" width='350' align="right">

<img src="https://upload.wikimedia.org/wikipedia/commons/0/05/Windows_10_Logo.svg" height="30px"/>

<p align="left">
  <a href="https://github.com/Sophia-Community/SophiApp/actions"><img src="https://img.shields.io/github/workflow/status/Sophia-Community/SophiApp/Build Release?label=GitHub%20Actions&logo=GitHub"></a>

  <a href="https://github.com/Sophia-Community/sophiapp/releases/latest"><img src="https://img.shields.io/github/v/release/Sophia-Community/sophiapp"></a>
  <a href="https://github.com/Sophia-Community/sophiapp/releases"><img src="https://img.shields.io/github/v/release/Sophia-Community/SophiApp?include_prereleases&label=pre-release&style=flat"></a>
  <a href="https://github.com/Sophia-Community/sophiapp/releases"><img src="https://img.shields.io/tokei/lines/github/Sophia-Community/SophiApp"></a>

  <a href="https://github.com/Sophia-Community/sophiapp/releases"><img src="https://img.shields.io/github/downloads/Sophia-Community/sophiapp/total?label=downloads%20%28since%20September%202021%29"></a>

  <a href="https://www.linkedin.com/in/vladimir-nameless-132745a1/"><img src="https://img.shields.io/badge/UI/UX%20by-Vladimir%20Nameless-blue?style=flat&logo=linkedin"></a>
  <a href="https://www.linkedin.com/mwlite/in/наталия-гуменюк-ba4a04161"><img src="https://img.shields.io/badge/Logo%20by-Natalia-blue?style=flat&logo=linkedin"></a>
  <img src="https://img.shields.io/badge/Made%20with-149ce2.svg?color=149ce2"><img src="https://github.com/websemantics/bragit/blob/master/demo/img/heart.svg" height="17px"/>

  <a href="https://t.me/SophiaNews"><img src="https://img.shields.io/badge/Sophia%20News-Telegram-blue?style=flat&logo=Telegram"></a>
  <a href="https://t.me/Sophia_Chat"><img src="https://img.shields.io/badge/Sophia%20Chat-Telegram-blue?style=flat&logo=Telegram"></a>
</p>

Available in: <img src="https://upload.wikimedia.org/wikipedia/commons/a/ae/Flag_of_the_United_Kingdom.svg" height="11px"/> <img src="https://upload.wikimedia.org/wikipedia/commons/f/f3/Flag_of_Russia.svg" height="11px"/>


<a href="https://github.com/Sophia-Community/SophiApp/releases/latest"><img src="https://raw.githubusercontent.com/farag2/Sophia-Script-for-Windows/master/img/SSdownloadbutton.svg" width=220px height=55px></a>

***

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Q5Q51QUJC)

<a href="https://yoomoney.ru/to/4100116615568835"><img src="https://yoomoney.ru/i/shop/iomoney_logo_color_example.png" width=220px height=46px></a>

***

<p align="center">
	&bull;
	<a href="#screenshots-helen">Screenshots</a>
	&bull;
	<a href="#core-features">Core features</a>
	&bull;
	<a href="https://github.com/Sophia-Community/SophiApp/blob/master/CHANGELOG.md">Changelog</a>
</p>

***

## System Requirements

* Currently supports `Windows 10 2004/20H2/21H1/21H2 x64` for now. Requires the `1151` build and higher. Windows 11 is in the works;
* To be able to run SophiApp, you must be the only logged-on account with admin rights.

## Installation

SophiApp is fully portable: doesn't have any config (yet) and doesn't save any data into registry. Just extract the `SophiApp` folder and run `SophiApp.exe`

## Core features

* All builds are being compiled in cloud via [GitHub Actions](https://github.com/Sophia-Community/SophiApp/actions)
  * You may compare a zip archive hash sum on the release page with the hash in cloud console in the `Compress Files` category to be sure that the archive wasn't spoofed;
* The app shows the `actual` state of every feature in the UI;
* Has a built-in search engine;
  * If you use the searching feature, it will locate the function you can change. [GIF](#searching-feature)
* Supports dark & light themes;
  * The app can change its' theme instantly when you change your default Windows theme mode for apps. [GIF](#instantly-changing-theme)
* Set up Privacy & Telemetry;
* Turn off diagnostics tracking scheduled tasks;
* Set up UI & Personalization;
* Uninstall OneDrive "correctly";
* Uninstall UWP apps displaying localized packages names;
  * An UWP apps list is being rendered dynamically using local icons
* Disable Windows features displaying friendly packages names;
* Uninstall Windows capabilities displaying friendly packages names;
* Download and install the [HEVC Video Extensions from Device Manufacturer](https://www.microsoft.com/p/hevc-video-extensions-from-device-manufacturer/9n4wgh0z6vhq) from Microsoft server using <https://store.rg-adguard.net> parser to be able to open .heic and .heif formats;
* Create a `Windows Cleanup` and `Windows Cleanup Notification` scheduled tasks for Windows cleaning up unused files and updates;
  * A native toast notification will be displayed where you can choose to snooze, run the cleanup task or [dismiss](#native-interactive-toasts-for-the-windows-cleanup-scheduled-task)
* Create tasks in the Task Scheduler to clear
  * `%SystemRoot%\SoftwareDistribution\Download`
  * `%TEMP%`
* Configure the Windows security;
* The ability to copy functions' description or header
* Many more File Explorer and context menu "deep" tweaks.

## Screenshots [Helen]

![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/1.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/2.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/3.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/4.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/5.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/6.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/7.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/8.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/9.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/10.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/11.png)
![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/12.png)

## Searching feature

![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/search.gif)

## Instantly changing theme

![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/theme.gif)

##  Adendum

* You are able to enable hidden functions in UI by turning on the "Advanced settings" in the Settings;
  * The hidden functions will marked with a gear in UI;
* After closing the SophiApp, it creates a log file that you can attach to an open issue to help us understands the bug. The log file doesn't contain any sensitive personal information.

## Translating

* Feel free to translate the UI into your language by translating the [JSON](https://github.com/Sophia-Community/SophiApp/blob/master/SophiApp/SophiApp/Resources/UIData.json) and creating a [.xml](https://github.com/Sophia-Community/SophiApp/tree/master/SophiApp/SophiApp/Localizations) file.

## The 3rd party libraries used

* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [TaskScheduler](https://github.com/dahall/taskscheduler)
* [ManagedDism](https://github.com/jeffkl/ManagedDism)
* [wix3](https://github.com/wixtoolset/wix3)

## Ask a question on

* [Telegram discussion group](https://t.me/sophia_chat)
* [Telegram channel](https://t.me/sophianews)
* [Ru-Board](https://forum.ru-board.com/topic.cgi?forum=5&topic=50903)
* [Reddit](https://www.reddit.com/user/farag2/)
