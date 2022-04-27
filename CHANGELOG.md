# Full Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.0.0.56 — 27.04.2022

Diff from v1.0.0.53
[1.0.0.53...1.0.0.56](https://github.com/Sophia-Community/SophiApp/compare/1.0.0.53...1.0.0.56)

* Added Italian translation <img src="https://upload.wikimedia.org/wikipedia/commons/0/03/Flag_of_Italy.svg" height="11px"/> 
  * Thanks to @THEBOSSMAGNUS;
* Added board to the app window;
* The Start menu functions were moved to the Personalization;
* Brought to the similar designs functions
  * PC Health Check app;
  * The latest Microsoft Visual C++ Redistributable Packages 2015–2022 x64;
  * OneDrive.
* Fixed a bug when the app didn't detect correctly whether C++ Redistributable packages' version was installed higher than SophiApp offered.

![Снимок экрана (1)](https://user-images.githubusercontent.com/10544660/165491772-c0c4d344-f890-4253-8819-51925f5de39f.png)

## 1.0.0.53 — 11.04.2022

* Startup checkings updated;
  * Now SophiApp checks whether Microsoft Defender was disabled via policies.
* Updated the German translation. Huge thanks to [Henry2o1o](https://github.com/Henry2o1o);
* Added the ability to download the latest stable build of SophiApp via PowerShell. Just type the command below, and it will download SophiaApp.zip from GitHub to the Downloads folder, expand the archive, and remove it. Elevated privileges are not needed. The app won't be launched.

  ```powershell
  irm app.sophi.app | iex
  ```

https://imgur.com/a/wCcs0Xi

## 1.0.0.50 — 08.04.2022

* UI render engine updated;
  * It is now possible to choose where to put the new ID in the JSON config without breaking the sequence of the IDs.
* Startup checkings updated;
* Added Windows 11 & Windows 11 Insider Preview support (22509+). Closes [#40](https://github.com/Sophia-Community/SophiApp/issues/40);
* UI/UX updated;
* Updated and expanded localizations;
* Added 🇩🇪 German and 🇺🇦 Ukrainian translations. Closes [#45](https://github.com/Sophia-Community/SophiApp/issues/45);
* Updated functions' descriptions;
* Huge code refactoring;
* Many bugs fixed.
* [Post](https://t.me/SophiaNews/746) on the Telegram channel about the announcement.

## 1.0.0.13 beta 2 — 31.01.2022

* Uploaded the most up-to-date screenshots that reflects the current state of development;
  * [Screeshots](https://github.com/Sophia-Community/SophiApp#screenshots-helen)
* The `Security` category completed;
* Added description to the `UWP applications` category;
  * Remind you that you may copy any header and description by right-clicking header.
* Removed the frame from the app window;
* Added the ability to search by functions headers and descriptions;
  * [GIF](https://github.com/Sophia-Community/SophiApp#searching-feature)
* Fixed button animation on the warning page if SophiApp is running on unsupported Windows;
* Added a new startup checking: you can't run multiple copies of SophiApp at the same time;
* Added description in the `SophiApp.exe` file properties;
* The states of the OneDrive install and uninstall buttons now look more clear;
* The `For All Users` switch on the `UWP Apps` category was replaced by a checkbox;
* The "RadioButtons Group" element was added a lower bound;
  * Closed #23.
* Add the ability to maximize window;
  * Closed #24.
* Window title bar now displays the app version;
* Minor UI changes;
* Minor bug fixed.

## 1.0.0.0 beta 1 — 30.12.2021

* Initial release after many pre-releases.
